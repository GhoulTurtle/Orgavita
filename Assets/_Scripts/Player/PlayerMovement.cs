using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour{
	[Header("Required References")]
	[SerializeField] private CharacterController characterController;
	[SerializeField] private Transform playerOrientation;
	[SerializeField] private Transform groundCheckTransform;
	[SerializeField] private LayerMask groundLayers;

	[Header("Base Movement Variables")]
	[SerializeField] private float walkSpeed;
	[SerializeField] private float runSpeed;
	[SerializeField] private float crouchMovementSpeed;
	[SerializeField] private float sprintCooldown;

	[Header("Crouch Variables")]
	[SerializeField] private float crouchSpeed;
	[SerializeField] private float crouchCooldown;
	[SerializeField] private float crouchHeight = 0.5f;

	[Header("Gravity Variables")]
	[SerializeField] private float gravity = -9.31f;
	[SerializeField] private float groundedRadius = 0.5f;

	public EventHandler<PlayerMovementStateChangedEventArgs> OnPlayerMovementStateChanged;
	public class PlayerMovementStateChangedEventArgs : EventArgs{
		public PlayerMovementState playerMovementState;
		public PlayerMovementStateChangedEventArgs(PlayerMovementState _playerMovementState){
			playerMovementState = _playerMovementState;
		}
	}
	public EventHandler<PlayerMovementDirectionChangedEventArgs> OnPlayerMovementDirectionChanged;
	public class PlayerMovementDirectionChangedEventArgs : EventArgs{
		public Vector3 rawDirection;
		public PlayerMovementDirectionChangedEventArgs(Vector3 _rawDirection){
			rawDirection = _rawDirection;
		}
	}
	public EventHandler OnPlayerMovementStopped;

	private const float validSprintAngle = 0.55f;
	private const float terminalVelocity = -53f;
	private const float crouchSnapDistance = 0.01f;
	private const float uncrouchRayDistance = 1.5f;
	
	private float xInput;
	private float yInput;
	private float verticalVelocity;
	private float movementSpeed;
	private float standingHeight;

	private bool canSprint = true;
	private bool canCrouch = true;
	private bool grounded;

	private PlayerMovementState currentPlayerMovementState = PlayerMovementState.Walking;

	private Vector2 previousMoveInput;
	private Vector2 playerMoveInput;
	private Vector3 moveDirection;
	private Vector3 initialCameraPosition;
	private Vector3 initialGroundCheckPosition;

	private WaitForSecondsRealtime sprintCooldownTimer;
	private WaitForSecondsRealtime crouchCooldownTimer;

	private IEnumerator currentCrouchJob;

	private void Awake() {
		sprintCooldownTimer = new WaitForSecondsRealtime(sprintCooldown);
		crouchCooldownTimer = new WaitForSecondsRealtime(crouchCooldown);
	}

	private void Start() {
		movementSpeed = walkSpeed;
		standingHeight = characterController.height;
		initialCameraPosition = playerOrientation.localPosition;
		initialGroundCheckPosition = groundCheckTransform.localPosition;
	}

	private void OnDestroy() {
		StopAllCoroutines();
	}

	private void Update(){
        GroundCheck();
        Gravity();
        if (currentPlayerMovementState == PlayerMovementState.Sprinting) CheckValidSprint();
        Move();
    }

    private void Move(){
        moveDirection = playerOrientation.forward * yInput + playerOrientation.right * xInput;
		if(moveDirection == Vector3.zero){
			OnPlayerMovementStopped?.Invoke(this, EventArgs.Empty);
		}

		if(playerMoveInput != previousMoveInput){
			OnPlayerMovementDirectionChanged?.Invoke(this, new PlayerMovementDirectionChangedEventArgs(playerMoveInput));
		}

        characterController.Move(movementSpeed * Time.deltaTime * moveDirection.normalized + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
		previousMoveInput = playerMoveInput;
    }

    public void MoveInput(InputAction.CallbackContext context){
		xInput = context.ReadValue<Vector2>().x;
		yInput = context.ReadValue<Vector2>().y;

		playerMoveInput.x = xInput;
		playerMoveInput.y = yInput;
	}

	public void SprintInput(InputAction.CallbackContext context){
		if(currentPlayerMovementState == PlayerMovementState.Crouching) return;

		if(context.canceled){
			movementSpeed = walkSpeed;
			UpdatePlayerMovementState(PlayerMovementState.Walking);
			return;
		}

		if(!canSprint) return;

		UpdatePlayerMovementState(PlayerMovementState.Sprinting);
		
		movementSpeed = runSpeed;
		StartCoroutine(SprintCooldownCoroutine());
	}

	public void CrouchInput(InputAction.CallbackContext context){
		if(currentPlayerMovementState == PlayerMovementState.Sprinting) return;

		var heightTarget = context.canceled ? standingHeight : crouchHeight;
		var speed = heightTarget == standingHeight ? walkSpeed : crouchMovementSpeed;
		var state = heightTarget == standingHeight ? PlayerMovementState.Walking : PlayerMovementState.Crouching;

		if(state == PlayerMovementState.Crouching && !canCrouch) return;

		// Attempting to uncrouch but hit a ceiling;
		if(state == PlayerMovementState.Walking && Physics.Raycast(transform.position, Vector3.up, uncrouchRayDistance)){
			return;
		}

		if(currentCrouchJob != null) StopCoroutine(currentCrouchJob);
		currentCrouchJob = CrouchJobCoroutine(heightTarget);
		StartCoroutine(currentCrouchJob);
		movementSpeed = speed;
		
		UpdatePlayerMovementState(state);

		if(state == PlayerMovementState.Crouching) StartCoroutine(CrouchCooldownCoroutine());
	}

	public bool IsMoving(){
		return moveDirection != Vector3.zero;
	}

	private void UpdatePlayerMovementState(PlayerMovementState state){
		if(currentPlayerMovementState == state) return;

		currentPlayerMovementState = state;

		OnPlayerMovementStateChanged?.Invoke(this, new PlayerMovementStateChangedEventArgs(currentPlayerMovementState));
	}

	private void CheckValidSprint(){
		if(yInput < validSprintAngle) {
			movementSpeed = walkSpeed;
			UpdatePlayerMovementState(PlayerMovementState.Walking);
		}
	}

	private void GroundCheck(){
		grounded = Physics.CheckSphere(groundCheckTransform.position, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
	}

	private void Gravity(){
		if(grounded){
			verticalVelocity = 0f;
			return;
		}

		if(verticalVelocity > terminalVelocity){
			verticalVelocity += gravity * Time.deltaTime;
		}
	}

	private IEnumerator CrouchJobCoroutine(float desiredHeight){
		while(characterController.height != desiredHeight){
			characterController.height = Mathf.Lerp(characterController.height, desiredHeight, crouchSpeed * Time.deltaTime);
			
			var halfHeightDifference = new Vector3(0, (standingHeight - characterController.height) / 2, 0);
			var newCameraPos = initialCameraPosition - halfHeightDifference;

			playerOrientation.localPosition = newCameraPos;

			var newGroundCheckPos = initialGroundCheckPosition + halfHeightDifference;
			groundCheckTransform.localPosition = newGroundCheckPos;
			
			if(Mathf.Abs(characterController.height - desiredHeight) < crouchSnapDistance){
				characterController.height = desiredHeight;
			}
			yield return null;
		}
	}

	private IEnumerator CrouchCooldownCoroutine(){
		canCrouch = false;
		yield return crouchCooldownTimer;
		canCrouch = true;
	}

	private IEnumerator SprintCooldownCoroutine(){
		canSprint = false;
		yield return sprintCooldownTimer;
		canSprint = true;
	}

	private void OnDrawGizmosSelected() {
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		Gizmos.DrawSphere(groundCheckTransform.position, groundedRadius);
		Gizmos.DrawRay(transform.position, Vector3.up * uncrouchRayDistance);
	}	
}