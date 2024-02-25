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

	private PlayerMovementEnum currentPlayerMovement = PlayerMovementEnum.Walking;

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
        if (movementSpeed == runSpeed) CheckValidSprint();
        Move();
    }

    private void Move(){
        moveDirection = playerOrientation.forward * yInput + playerOrientation.right * xInput;
        characterController.Move(movementSpeed * Time.deltaTime * moveDirection.normalized + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }

    public void MoveInput(InputAction.CallbackContext context){
		xInput = context.ReadValue<Vector2>().x;
		yInput = context.ReadValue<Vector2>().y;
	}

	public void SprintInput(InputAction.CallbackContext context){
		if(currentPlayerMovement == PlayerMovementEnum.Crouching) return;

		if(context.canceled){
			movementSpeed = walkSpeed;
			currentPlayerMovement = PlayerMovementEnum.Walking;
			return;
		}

		if(!canSprint) return;
		currentPlayerMovement = PlayerMovementEnum.Sprinting;
		movementSpeed = runSpeed;
		StartCoroutine(SprintCooldownCoroutine());
	}

	public void CrouchInput(InputAction.CallbackContext context){
		if(currentPlayerMovement == PlayerMovementEnum.Sprinting) return;

		var heightTarget = context.canceled ? standingHeight : crouchHeight;
		var speed = heightTarget == standingHeight ? walkSpeed : crouchMovementSpeed;
		var state = heightTarget == standingHeight ? PlayerMovementEnum.Walking : PlayerMovementEnum.Crouching;

		if(state == PlayerMovementEnum.Crouching && !canCrouch) return;

		// Attempting to uncrouch but hit a ceiling;
		if(state == PlayerMovementEnum.Walking && Physics.Raycast(transform.position, Vector3.up, uncrouchRayDistance)){
			return;
		}

		if(currentCrouchJob != null) StopCoroutine(currentCrouchJob);
		currentCrouchJob = CrouchJobCoroutine(heightTarget);
		StartCoroutine(currentCrouchJob);
		movementSpeed = speed;
		currentPlayerMovement = state;
		if(state == PlayerMovementEnum.Crouching) StartCoroutine(CrouchCooldownCoroutine());
	}

	private void CheckValidSprint(){
		if(yInput < validSprintAngle) movementSpeed = walkSpeed;
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