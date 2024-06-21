using UnityEngine;

[System.Serializable]
public class ChildBehaviourData{
    public Transform lockPos;
    public bool keepOffset;
    public Vector3 offsetPos;
    public Vector3 offsetRotation;
    public bool delayPosUpdate;
    public float followSpeed;
}