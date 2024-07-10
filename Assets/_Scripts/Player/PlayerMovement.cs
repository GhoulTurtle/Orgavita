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
	[SerializeField] private Transform groundCheckTransform;
	[SerializeField] private LayerMask groundLayers;

	[Header("Base Movement Variables")]
	[SerializeField] private float walkSpeed;
	[SerializeField] private float crouchMovementSpeed;
	[SerializeField] private float runSpeed;
	[SerializeField] private float aimingSpeed;
	[SerializeField] private float sprintCooldown;

	[Header("Crouch Variables")]
	[SerializeField] private float crouchTimeInSeconds;
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
	private const float crouchSnapDistance = 0.001f;
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
	private PlayerMovementState nextPlayerMovementState = PlayerMovementState.Walking;

	private TerrainType currentTerrainType = TerrainType.None;

	private Vector2 previousMoveInput;
	private Vector2 playerMoveInput;
	private Vector3 moveDirection;
	private Vector3 initialCameraPosition;
	private Vector3 initialGroundCheckPosition;

	private WaitForSeconds sprintCooldownTimer;
	private WaitForSeconds crouchCooldownTimer;

	private IEnumerator currentUnCrouchJob;
	private IEnumerator currentCrouchJob;

	private void Awake() {
		sprintCooldownTimer = new WaitForSeconds(sprintCooldown);
		crouchCooldownTimer = new WaitForSeconds(crouchCooldown);
		if(playerEquippedItemHandler != null){
			playerEquippedItemHandler.OnWeaponItemBehaviourSpawned += SubscribeToWeaponStateEvent;
			playerEquippedItemHandler.OnWeaponItemBehaviourDespawned += UnsubscribeToWeaponStateEvent;
		}
	}

    private void Start() {
		movementSpeed = walkSpeed;
		standingHeight = characterController.height;
		initialCameraPosition = playerOrientation.localPosition;
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
		if(currentPlayerMovementState == PlayerMovementState.Crouching || currentPlayerMovementState == PlayerMovementState.Aiming) return;

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

	public void StartAiming(){
		if(currentPlayerMovementState != PlayerMovementState.Crouching){
			movementSpeed = aimingSpeed;
		}

		UpdatePlayerMovementState(PlayerMovementState.Aiming);
	}

	public void StopAiming(){
		if(nextPlayerMovementState != PlayerMovementState.Crouching && currentUnCrouchJob == null){
			movementSpeed = walkSpeed;
		}

		UpdatePlayerMovementState(nextPlayerMovementState);
	}

	public void CrouchInput(InputAction.CallbackContext context){
		if(currentPlayerMovementState == PlayerMovementState.Sprinting) return;

		var heightTarget = context.canceled ? standingHeight : crouchHeight;
		
		var speed = heightTarget == standingHeight ? walkSpeed : crouchMovementSpeed;

		if(speed == walkSpeed && currentPlayerMovementState == PlayerMovementState.Aiming){
			speed = aimingSpeed;
		}

		var state = heightTarget == standingHeight ? PlayerMovementState.Walking : PlayerMovementState.Crouching;

		nextPlayerMovementState = state;

		//Attempting to crouch but crouch is on cooldown so return.
		if(state == PlayerMovementState.Crouching && !canCrouch) return;

		// Attempting to uncrouch but hit a ceiling, start a uncrouch job
		if(state == PlayerMovementState.Walking && Physics.Raycast(transform.position, Vector3.up, uncrouchRayDistance)){
			currentUnCrouchJob = UnCrouchJob();
			StartCoroutine(currentUnCrouchJob);
			return;
		}

		if(currentCrouchJob != null) StopCoroutine(currentCrouchJob);
		if(currentUnCrouchJob != null) StopCoroutine(currentUnCrouchJob);
		currentCrouchJob = CrouchJobCoroutine(heightTarget);
		StartCoroutine(currentCrouchJob);
		movementSpeed = speed;
		
		if(currentPlayerMovementState != PlayerMovementState.Aiming){
			UpdatePlayerMovementState(state);
		}

		if(state == PlayerMovementState.Crouching) StartCoroutine(CrouchCooldownCoroutine());
	}

	public bool IsMoving(){
		return moveDirection != Vector3.zero;
	}

	public TerrainType GetCurrentTerrainType(){
		return currentTerrainType;
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
		if(grounded && Physics.Raycast(groundCheckTransform.position, Vector3.down, out RaycastHit hitInfo, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore) && hitInfo.collider.TryGetComponent(out Terrain terrain)){
			currentTerrainType = terrain.GetTerrainType();
		}
		else{
			currentTerrainType = TerrainType.None;
		}
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

	private void SubscribeToWeaponStateEvent(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
		e.equippedItemBehaviour.OnWeaponStateChanged += EvaulateCurrentWeaponState;
    }

    private void UnsubscribeToWeaponStateEvent(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
		e.equippedItemBehaviour.OnWeaponStateChanged -= EvaulateCurrentWeaponState;
	}

    private void EvaulateCurrentWeaponState(object sender, EquippedItemBehaviour.WeaponStateChangedEventArgs e){
		if(currentPlayerMovementState == PlayerMovementState.Aiming && e.weaponState != WeaponState.Aiming){
			StopAiming();
		}
		else if(e.weaponState == WeaponState.Aiming){
			StartAiming();
		}
    }

	private IEnumerator CrouchJobCoroutine(float desiredHeight){
		float current = 0;

		while(Mathf.Abs(characterController.height - desiredHeight) > crouchSnapDistance){
			characterController.height = Mathf.Lerp(characterController.height, desiredHeight, current / crouchTimeInSeconds);

			current += Time.deltaTime;

			var halfHeightDifference = new Vector3(0, (standingHeight - characterController.height) / 2, 0);
			var newCameraPos = initialCameraPosition - halfHeightDifference;

			playerOrientation.localPosition = newCameraPos;

			var newGroundCheckPos = initialGroundCheckPosition + halfHeightDifference;
			groundCheckTransform.localPosition = newGroundCheckPos;
			yield return null;
		}

		characterController.height = desiredHeight;
	}

	private IEnumerator UnCrouchJob(){
		while(Physics.Raycast(transform.position, Vector3.up, uncrouchRayDistance)){
			yield return null;
		}

		currentCrouchJob = CrouchJobCoroutine(standingHeight);
		StartCoroutine(currentCrouchJob);
		
		if(currentPlayerMovementState != PlayerMovementState.Aiming){
			movementSpeed = walkSpeed;
			UpdatePlayerMovementState(PlayerMovementState.Walking);
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