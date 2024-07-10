using Cinemachine;
using UnityEngine;

public static class CameraShake{
    public static void TriggerCameraShake(CinemachineImpulseSource cameraShakeSource, CinemachineImpulseDefinition.ImpulseShapes shakeShape, float shakeDuration, float shakeAmplitude, Vector3 shakeDirection){
        CinemachineImpulseDefinition impulseDefinition = cameraShakeSource.m_ImpulseDefinition;
        
        impulseDefinition.m_ImpulseShape = shakeShape;
        impulseDefinition.m_ImpulseDuration = shakeDuration;
        cameraShakeSource.m_DefaultVelocity = shakeDirection;

        cameraShakeSource.GenerateImpulse(shakeAmplitude);
    }
}
