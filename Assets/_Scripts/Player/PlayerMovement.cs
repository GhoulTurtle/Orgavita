using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour{
	[Header("Required References")]
	[SerializeField] private PlayerEquippedItemHandler playerEquippedItemHandler;
	[SerializeField] private CharacterController characterController;
	[SerializeField] private Transform playerOrientation;
	[SerializeField] private Transform cameraTransform;
	[SerializeField] private Transform groundCheckTransform;
	[SerializeField] private LayerMask groundLayers;

	[Header("Base Movement Variables")]
	[SerializeField] private float walkSpeed;
	[SerializeField] private float crouchSpeed;
	[SerializeField] private float sprintSpeed;
	[SerializeField] private float aimingSpeed;
	[SerializeField] private float sprintCooldown;

	[Header("Crouch Variables")]
	[SerializeField] private float crouchTimeInSeconds;
	[SerializeField] private float crouchCooldown;
	[SerializeField] private float crouchHeight = 0.5f;

	[Header("Gravity Variables")]
	[SerializeField] private float gravity = -9.31f;
	[SerializeField] private float groundedRadius = 0.5f;

	[Header("Debug Variables")]
	[SerializeField] private bool drawGizmos = false;

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
	private const float crouchSnapDistance = 0.1f;
	
	private float xInput;
	private float yInput;
	private float verticalVelocity;
	private float movementSpeed;
	private float standingHeight;

	private bool grounded;
	private bool isAiming = false;
	private bool isMoving = false;

	private PlayerMovementState currentPlayerMovementState = PlayerMovementState.Walking;

	private TerrainType currentTerrainType = TerrainType.None;

	private Vector2 previousMoveInput;
	private Vector2 playerMoveInput;
	private Vector3 moveDirection;
	private Vector3 initialCameraPosition;
	private Vector3 initialGroundCheckPosition;

	private IEnumerator currentSprintCooldown;
	private IEnumerator currentCrouchCooldown;
	private IEnumerator currentStandingCheck;
	private IEnumerator currentCrouchLerpAnimation;

    private void Awake() {
		if(playerEquippedItemHandler != null){
			playerEquippedItemHandler.OnWeaponItemBehaviourSpawned += SubscribeToWeaponStateEvent;
			playerEquippedItemHandler.OnWeaponItemBehaviourDespawned += UnsubscribeToWeaponStateEvent;
		}
	}

    private void Start() {
		UpdateMoveSpeedOnCurrentMovementState();

		standingHeight = characterController.height;
		initialCameraPosition = cameraTransform.localPosition;
		initialGroundCheckPosition = groundCheckTransform.localPosition;
	}

	private void OnDestroy() {
		StopAllCoroutines();
		
		if(playerEquippedItemHandler != null){
			playerEquippedItemHandler.OnWeaponItemBehaviourSpawned -= SubscribeToWeaponStateEvent;
			playerEquippedItemHandler.OnWeaponItemBehaviourDespawned -= UnsubscribeToWeaponStateEvent;
		}
	}

	private void Update(){
        GroundCheck();
        Gravity();
        if (currentPlayerMovementState == PlayerMovementState.Sprinting) CheckValidSprint();
        Move();
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
			UpdatePlayerMovementState(PlayerMovementState.Walking);
			return;
		}

		if(currentSprintCooldown != null) return;

		UpdatePlayerMovementState(PlayerMovementState.Sprinting);
		
		currentSprintCooldown = SprintCooldownCoroutine();
		StartCoroutine(currentSprintCooldown);
	}

	public void CrouchInput(InputAction.CallbackContext context){
		if(currentPlayerMovementState == PlayerMovementState.Sprinting) return;

		var state = context.canceled ? PlayerMovementState.Walking : PlayerMovementState.Crouching;

		if(state == PlayerMovementState.Crouching){
			if(currentCrouchCooldown != null) return;
			StartCrouch();
		}
		else{
			StopCrouch();
		}
	}

	private void StartCrouch(){
		StopCrouchCoroutines();

		currentCrouchLerpAnimation = CrouchLerpAnimationCoroutine(crouchHeight);
		StartCoroutine(currentCrouchLerpAnimation);
		UpdatePlayerMovementState(PlayerMovementState.Crouching);
		
		currentCrouchCooldown = CrouchCooldownCoroutine();
		StartCoroutine(currentCrouchCooldown);
	}

	private void StopCrouch(){
		StopCrouchCoroutines();

		if(!CanStand()){
			currentStandingCheck = ValidStandingCheckCoroutine();
			StartCoroutine(currentStandingCheck);
			return;
		}

		currentCrouchLerpAnimation = CrouchLerpAnimationCoroutine(standingHeight);
		StartCoroutine(currentCrouchLerpAnimation);

		UpdatePlayerMovementState(PlayerMovementState.Walking);
	}

	private void StopCrouchCoroutines(){
		if(currentCrouchLerpAnimation != null){
			StopCoroutine(currentCrouchLerpAnimation);
			currentCrouchLerpAnimation = null;
		} 
			
		if(currentStandingCheck != null){
			StopCoroutine(currentStandingCheck);
			currentStandingCheck = null;
		}
	}

	public void StartAiming(){
		if(currentPlayerMovementState != PlayerMovementState.Crouching){
			movementSpeed = aimingSpeed;
		}

		isAiming = true;
	}

	public void StopAiming(){
		isAiming = false;

		UpdateMoveSpeedOnCurrentMovementState();
		OnPlayerMovementStateChanged?.Invoke(this, new PlayerMovementStateChangedEventArgs(currentPlayerMovementState));
	}

	private void Move(){
        moveDirection = playerOrientation.forward * yInput + playerOrientation.right * xInput;

		if(moveDirection == Vector3.zero){
			OnPlayerMovementStopped?.Invoke(this, EventArgs.Empty);
			isMoving = false;
		}

		Vector3 preMovePosition = characterController.transform.position;
		characterController.Move(movementSpeed * Time.deltaTime * moveDirection.normalized + Time.deltaTime * verticalVelocity * Vector3.up);

		Vector3 postMovePosition = characterController.transform.position;

		Vector3 movementDelta = postMovePosition - preMovePosition;

		if(movementDelta == Vector3.zero){
			isMoving = false;
			OnPlayerMovementStopped?.Invoke(this, EventArgs.Empty);
		}
		else{
			isMoving = true;
		}
		
		if(playerMoveInput != previousMoveInput){
			OnPlayerMovementDirectionChanged?.Invoke(this, new PlayerMovementDirectionChangedEventArgs(playerMoveInput));
		}
		
		previousMoveInput = playerMoveInput;
    }

	private void GroundCheck(){
		grounded = Physics.CheckSphere(groundCheckTransform.position, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
		
		if(!grounded){
			currentTerrainType = TerrainType.None;
			return;
		}

		if(!Physics.Raycast(groundCheckTransform.position, Vector3.down, out RaycastHit hitInfo, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore)) return;
		
		if(!hitInfo.collider.TryGetComponent(out Terrain terrain)){
			currentTerrainType = TerrainType.None;
			return;
		}
		
		currentTerrainType = terrain.GetTerrainType();
	}

	private void Gravity(){
		if(grounded){
			verticalVelocity = -5f;
			return;
		}

		if(verticalVelocity > terminalVelocity){
			verticalVelocity += gravity * Time.deltaTime;
		}
	}

	private void CheckValidSprint(){
		if(yInput < validSprintAngle) {
			UpdatePlayerMovementState(PlayerMovementState.Walking);
		}
	}

	private void UpdatePlayerMovementState(PlayerMovementState state){
		if(currentPlayerMovementState == state) return;
		currentPlayerMovementState = state;

		if(isAiming){
			if(currentPlayerMovementState != PlayerMovementState.Crouching){
				movementSpeed = aimingSpeed;
			}
			else{
				movementSpeed = crouchSpeed;
			}
			return;
		}
		
		UpdateMoveSpeedOnCurrentMovementState();
		OnPlayerMovementStateChanged?.Invoke(this, new PlayerMovementStateChangedEventArgs(currentPlayerMovementState));
    }

	private void UpdateMoveSpeedOnCurrentMovementState(){
		switch (currentPlayerMovementState){
		case PlayerMovementState.Walking: 
			movementSpeed = walkSpeed;
			break;
		case PlayerMovementState.Sprinting: 
			movementSpeed = sprintSpeed;
			break;
		case PlayerMovementState.Crouching: 
			movementSpeed = crouchSpeed;
			break;
        }
	}

    private void EvaulateCurrentWeaponState(object sender, EquippedItemBehaviour.WeaponStateChangedEventArgs e){
		if(isAiming && e.weaponState != WeaponState.Aiming){
			StopAiming();
		}
		else if(e.weaponState == WeaponState.Aiming){
			StartAiming();
		}
    }

	private void SubscribeToWeaponStateEvent(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
		e.equippedItemBehaviour.OnWeaponStateChanged += EvaulateCurrentWeaponState;
    }

    private void UnsubscribeToWeaponStateEvent(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
		e.equippedItemBehaviour.OnWeaponStateChanged -= EvaulateCurrentWeaponState;
	}

	private IEnumerator CrouchLerpAnimationCoroutine(float desiredHeight){
		float current = 0;

		while(Mathf.Abs(characterController.height - desiredHeight) > crouchSnapDistance){
			characterController.height = Mathf.Lerp(characterController.height, desiredHeight, current / crouchTimeInSeconds);

			var halfHeightDifference = new Vector3(0, (standingHeight - characterController.height) / 2, 0);
			var newCameraPos = initialCameraPosition - halfHeightDifference;

			cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newCameraPos, current / crouchTimeInSeconds) ;

			var newGroundCheckPos = initialGroundCheckPosition + halfHeightDifference;
			groundCheckTransform.localPosition = newGroundCheckPos;

			current += Time.deltaTime;
			yield return null;
		}

		characterController.height = desiredHeight;
	}

	private IEnumerator ValidStandingCheckCoroutine(){
		while(Physics.SphereCast(transform.position, characterController.radius,  Vector3.up, out RaycastHit hitInfo, standingHeight)){
			yield return null;
		}

		currentCrouchLerpAnimation = CrouchLerpAnimationCoroutine(standingHeight);
		StartCoroutine(currentCrouchLerpAnimation);
		
		UpdatePlayerMovementState(PlayerMovementState.Walking);
	}

	private IEnumerator CrouchCooldownCoroutine(){
		yield return new WaitForSeconds(crouchCooldown);
		currentCrouchCooldown = null;
	}

	private IEnumerator SprintCooldownCoroutine(){
		yield return new WaitForSeconds(sprintCooldown);
		currentSprintCooldown = null;
	}

	public bool CanStand(){
		return !Physics.Raycast(transform.position, Vector3.up, standingHeight);
	}

	public bool IsMoving(){
		return isMoving;
	}

	public TerrainType GetCurrentTerrainType(){
		return currentTerrainType;
	}

		private void OnDrawGizmosSelected() {
		if(!drawGizmos) return;

		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		Gizmos.DrawSphere(groundCheckTransform.position, groundedRadius);
		Gizmos.DrawRay(transform.position, Vector3.up * standingHeight);
	}
}