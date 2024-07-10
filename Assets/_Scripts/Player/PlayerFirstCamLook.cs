using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using System.Collections;

public class PlayerFirstCamLook : MonoBehaviour{
	[Header("Cam Variables")]
	[SerializeField] private float camSens;
	[SerializeField] private bool lockCursor;

	[Header("Tilt Variables")]
	[SerializeField] private float tiltTime = 0.25f;
	[SerializeField] private float runningTiltAmount = 2f;
	[SerializeField] private float tiltAmount = 2f;

	[Header("Headbob Variables")]
	[SerializeField, Range(0, 0.1f)] private float bobbingAmplitude = 0.003f;
	[SerializeField, Range(0, 30f)] private float bobbingFrequency = 7.0f;
	[SerializeField, Range(0, 0.1f)] private float crouchingBobbingAmplitude = 0.001f;
	[SerializeField, Range(0, 30f)] private float crouchingBobbingFrequency = 6.5f;
	[SerializeField, Range(0, 0.1f)] private float runningBobbingAmplitude = 0.0055f;
	[SerializeField, Range(0, 30f)] private float runningBobbingFrequency = 15f;
	[SerializeField, Range(0, 0.1f)] private float aimingBobbingAmplitude = 0.001f;
	[SerializeField, Range(0, 30)] private float aimingBobbingFrquency = 7;

	[SerializeField, Range(1f, 5f)] private float headBobResetSpeed = 3f;

	[Header("FOV Variables")]
	[SerializeField] private float cameraAimFOV;
	[SerializeField] private float cameraDefaultFOV;
	[SerializeField] private float cameraFOVAnimationTime;

	[Header("Rquired Reference")]
	[SerializeField] private PlayerEquippedItemHandler playerEquippedItemHandler;
	[SerializeField] private PlayerCameraVisualHandler playerCameraVisualHandler;
	[SerializeField] private Transform cameraRoot;
	[SerializeField] private Transform cameraTransform;
	[SerializeField] private Transform characterOrientation;

	private const float YClamp = 80f;

	private float lastSinValue;
	private bool isMovingUpwards = true;

	private float camX;
	private float camY;
	private float currentAmplitude;
	private float currentFrequency;

	private Vector3 startPos;
	private Vector3 currentMovementVectorNormalized;
	private Vector3 currentTiltVector;

	private PlayerMovement playerMovement;
	private EquippedItemBehaviour equippedWeaponItemBehaviour;

	private IEnumerator currentCameraFOVAnimationCoroutine;

	public EventHandler<TerrainStepEventArgs> OnTerrainStep; 
	public class TerrainStepEventArgs : EventArgs{
		public TerrainType terrainType;
		public TerrainStepEventArgs(TerrainType _terrainType){
			terrainType = _terrainType;
		}
	}

	private void Awake() {
		startPos = cameraTransform.localPosition;
		currentAmplitude = bobbingAmplitude;
		currentFrequency = bobbingFrequency;
	}

    private void Start() {
		if(TryGetComponent(out playerMovement)){
			playerMovement.OnPlayerMovementStateChanged += EvaluatePlayerMovementState;
			playerMovement.OnPlayerMovementDirectionChanged += UpdateCameraTilt;
			playerMovement.OnPlayerMovementStopped += ResetCameraTilt;
		}

		if(playerEquippedItemHandler != null){
			playerEquippedItemHandler.OnWeaponItemBehaviourSpawned += WeaponSpawned;
			playerEquippedItemHandler.OnWeaponItemBehaviourDespawned += WeaponDespawned;
		}

		Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
	}

    private void OnDestroy() {
		if(playerMovement != null){
			playerMovement.OnPlayerMovementDirectionChanged -= UpdateCameraTilt;
			playerMovement.OnPlayerMovementStopped -= ResetCameraTilt;
		}

		if(playerEquippedItemHandler != null){
			playerEquippedItemHandler.OnWeaponItemBehaviourSpawned -= WeaponSpawned;
			playerEquippedItemHandler.OnWeaponItemBehaviourDespawned -= WeaponDespawned;
		}

		UnsubscribeFromEquippedWeaponEvents();
	}

	public void Update(){
		cameraRoot.localRotation = Quaternion.Euler(camY, camX, 0);

		characterOrientation.transform.localRotation = Quaternion.Euler(0, camX, 0);

		CheckMotion();
		ResetPosition();
	}

	public void OnLookInput(InputAction.CallbackContext context){
		var inputVector = context.ReadValue<Vector2>();
		camX += inputVector.x * camSens * Time.deltaTime;
		camY -= inputVector.y * camSens * Time.deltaTime;
		camY = Mathf.Clamp(camY, -YClamp, YClamp);
	}

	public void LookInputInjected(Vector2 inputVector){
		camX += inputVector.x;
		camY -= inputVector.y;
		camY = Mathf.Clamp(camY, -YClamp, YClamp);
	}

	private void CheckMotion(){
		if(!playerMovement.IsMoving()) return;
		PlayMotion(StepMotionCalculation());
	}

	private void PlayMotion(Vector3 motion){
		cameraTransform.localPosition += motion;
	}

	private void ResetPosition(){
		if (cameraTransform.localPosition == startPos) return;
		cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, startPos, headBobResetSpeed * Time.deltaTime);
	}

	private Vector3 StepMotionCalculation(){
		Vector3 pos = Vector3.zero;
		pos.y += Mathf.Sin(Time.time * currentFrequency) * currentAmplitude;
		pos.x += Mathf.Cos(Time.time * currentFrequency / 2) * currentAmplitude * 2;
		
		if ((pos.y > 0 && !isMovingUpwards) || (pos.y < 0 && isMovingUpwards)){
            // Footstep sound should play at the peak of the sin wave
            if (lastSinValue < 0 && pos.y >= 0){
                OnTerrainStep?.Invoke(this, new TerrainStepEventArgs(playerMovement.GetCurrentTerrainType()));
            }
            isMovingUpwards = pos.y >= 0;
        }
		
		lastSinValue = pos.y;
		return pos;
	}

    private void ResetCameraTilt(object sender, EventArgs e){
		cameraTransform.DOLocalRotate(Vector3.zero, tiltTime);
    }

    private void UpdateCameraTilt(object sender, PlayerMovement.PlayerMovementDirectionChangedEventArgs e){
		currentMovementVectorNormalized = e.rawDirection.normalized;

		Vector3 tiltDirection = currentMovementVectorNormalized * tiltAmount;

		currentTiltVector = new Vector3(tiltDirection.y, 0, -tiltDirection.x);

		cameraTransform.DOLocalRotate(currentTiltVector, tiltTime);
    }

	private void EvaluatePlayerMovementState(object sender, PlayerMovement.PlayerMovementStateChangedEventArgs e){
        switch (e.playerMovementState){
            case PlayerMovementState.Sprinting:
				UpdateCameraAnimation(cameraDefaultFOV, runningTiltAmount, runningBobbingAmplitude, runningBobbingFrequency);
                break;
            case PlayerMovementState.Walking:
				UpdateCameraAnimation(cameraDefaultFOV, tiltAmount, bobbingAmplitude, bobbingFrequency);
                break;
            case PlayerMovementState.Crouching:
				UpdateCameraAnimation(cameraDefaultFOV, tiltAmount, crouchingBobbingAmplitude, crouchingBobbingFrequency);
                break;
        }
    }

	private void WeaponAimedIn(){
		UpdateCameraAnimation(cameraAimFOV, tiltAmount, aimingBobbingAmplitude, aimingBobbingFrquency);		
	}

	private void UpdateCameraAnimation(float cameraFOV, float tiltAmount, float bobbingAmplitude, float bobbingFrequency){
		currentAmplitude = bobbingAmplitude;
		currentFrequency = bobbingFrequency;
		
		Vector3 tiltDirection;
		
		tiltDirection = currentMovementVectorNormalized * tiltAmount;
		
		if(playerCameraVisualHandler.GetCameraFOV() != cameraFOV){
			StopFOVAnimationCoroutine();
			currentCameraFOVAnimationCoroutine = playerCameraVisualHandler.CameraFOVAnimationCoroutine(cameraFOV, cameraFOVAnimationTime);
			StartCoroutine(currentCameraFOVAnimationCoroutine);
		}

        currentTiltVector = new Vector3(tiltDirection.y, 0, -tiltDirection.x);
		cameraTransform.DOLocalRotate(currentTiltVector, tiltTime);
	}

    private void WeaponSpawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
		equippedWeaponItemBehaviour = e.equippedItemBehaviour;

		equippedWeaponItemBehaviour.OnWeaponStateChanged += EvaulateWeaponState;
		equippedWeaponItemBehaviour.OnKickbackApplied += ApplyCurrentWeaponKickback;
    }

    private void WeaponDespawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
		UnsubscribeFromEquippedWeaponEvents();
    }

	private void UnsubscribeFromEquippedWeaponEvents(){
		if(equippedWeaponItemBehaviour != null){
			equippedWeaponItemBehaviour.OnKickbackApplied -= ApplyCurrentWeaponKickback;
			equippedWeaponItemBehaviour.OnWeaponStateChanged -= EvaulateWeaponState;

			equippedWeaponItemBehaviour = null;
		}
	}

	private void EvaulateWeaponState(object sender, EquippedItemBehaviour.WeaponStateChangedEventArgs e){
		if(e.weaponState == WeaponState.Aiming){
			WeaponAimedIn();
		}
    }

    private void ApplyCurrentWeaponKickback(object sender, EquippedItemBehaviour.KickbackAppliedEventArgs e){
		Vector2 kickbackVector = new(0, 1 * e.kickbackAmount);
		LookInputInjected(kickbackVector);
    }

    private void StopFOVAnimationCoroutine(){
        if(currentCameraFOVAnimationCoroutine != null){
			StopCoroutine(currentCameraFOVAnimationCoroutine);
			currentCameraFOVAnimationCoroutine = null;
		}
    }
}