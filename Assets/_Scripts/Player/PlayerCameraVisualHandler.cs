using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Responsible for handling all visual aspects that effect the camera. Including: Camera Shake, Post Processing, and Camera Noise.
/// </summary>
public class PlayerCameraVisualHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private CinemachineImpulseSource cameraShakeImpulseSource;
    [SerializeField] private CinemachineVirtualCamera mainPlayerVirtualCamera;
    [SerializeField] private VolumeProfile mainPlayerVolumeProfile;
    [SerializeField] private PlayerEquippedItemHandler playerEquippedItemHandler;

    private readonly float cameraFOVAnimationSnapDistance = 0.01f;

    private EquippedItemBehaviour equippedWeaponItemBehaviour;
    private WeaponDataSO equippedWeaponDataSO;

    private void Start() {
        playerEquippedItemHandler.OnWeaponItemBehaviourSpawned += WeaponSpawned;
        playerEquippedItemHandler.OnWeaponItemBehaviourDespawned += WeaponDespawned;
    }

    private void OnDestroy() {
        playerEquippedItemHandler.OnWeaponItemBehaviourSpawned -= WeaponSpawned;
        playerEquippedItemHandler.OnWeaponItemBehaviourDespawned -= WeaponDespawned;

        if(equippedWeaponItemBehaviour != null){
            equippedWeaponItemBehaviour.OnWeaponUse -= TriggerWeaponCameraShake;
        }
    }

    private void WeaponSpawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        equippedWeaponItemBehaviour = e.equippedItemBehaviour;
        equippedWeaponDataSO = equippedWeaponItemBehaviour.GetEquippedWeaponData();

        equippedWeaponItemBehaviour.OnWeaponUse += TriggerWeaponCameraShake;
    }

    private void WeaponDespawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        equippedWeaponItemBehaviour.OnWeaponUse -= TriggerWeaponCameraShake;
        
        equippedWeaponItemBehaviour = null;
        equippedWeaponDataSO = null;
    }

    private void TriggerWeaponCameraShake(object sender, EventArgs e){
        ActivateCameraShake(equippedWeaponDataSO.cameraShakeShapeOnFired, equippedWeaponDataSO.cameraShakeDurationOnFired, equippedWeaponDataSO.cameraShakeAmplitudeOnFired, equippedWeaponDataSO.cameraShakeDirection);
    }

    public void ActivateCameraShake(CinemachineImpulseDefinition.ImpulseShapes shape, float shakeDuration, float shakeAmplitude, Vector3 shakeDirection){
        CameraShake.TriggerCameraShake(cameraShakeImpulseSource, shape, shakeDuration, shakeAmplitude, shakeDirection);
    }

    public IEnumerator CameraFOVAnimationCoroutine(float fovTarget, float cameraFOVAnimationTime){
		float current = 0;

        while(Mathf.Abs(mainPlayerVirtualCamera.m_Lens.FieldOfView - fovTarget) > cameraFOVAnimationSnapDistance){
            if(Time.timeScale != 0){
                mainPlayerVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(mainPlayerVirtualCamera.m_Lens.FieldOfView, fovTarget, current / cameraFOVAnimationTime);
                current += Time.deltaTime;
            }
            
            yield return null;
        }

		mainPlayerVirtualCamera.m_Lens.FieldOfView = fovTarget;
	}

    public float GetCameraFOV(){
        return mainPlayerVirtualCamera.m_Lens.FieldOfView;
    }
}
