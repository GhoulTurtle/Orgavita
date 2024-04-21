using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable{
    public string InteractionPrompt {get{
        if(isLocked) return "";
        
        string prompt = isOpen ? "Close" : "Open";
        return prompt;
    }}

    [Header("Door Variables")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private float doorRotationSpeedInSeconds = 1f;
    [SerializeField] private float rotationAmount = 90f;
    [SerializeField] private float forwardDirection = 0f;

    private Vector3 startRotation;
    private Vector3 forward;

    private IEnumerator currentDoorAnimation;

    private bool isOpen;

    private void Awake() {
        startRotation = transform.rotation.eulerAngles;
        forward = transform.forward;
    }

    private void OnDestroy() {
        StopDoorAnimation();
    }

    public bool Interact(PlayerInteract player){
        if(isLocked){
            return false;
        }

        Open(player.transform.position);
        return true;
    }

    public void Open(Vector3 userPosition){
        StopDoorAnimation();

        float dot = Vector3.Dot(forward, (userPosition - transform.position).normalized);
        currentDoorAnimation = DoorAnimation(dot, !isOpen);
        StartCoroutine(currentDoorAnimation);
    }

    public void UnlockDoor(){
        isLocked = false;
    }

    private IEnumerator DoorAnimation(float forwardAmount, bool openingDoor){
        Quaternion doorStartRotation = transform.rotation;
        Quaternion endRotation;

        if(!openingDoor){
            endRotation = Quaternion.Euler(startRotation);
        }
        else if(forwardAmount >= forwardDirection){
            endRotation = Quaternion.Euler(new Vector3(0, doorStartRotation.y - rotationAmount, 0));
        }
        else{
            endRotation = Quaternion.Euler(new Vector3(0, doorStartRotation.y + rotationAmount, 0));
        }

        isOpen = openingDoor;

        float current = 0;

        while(current < 1.5){
            transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, current / doorRotationSpeedInSeconds);
            current += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
    }

    private void StopDoorAnimation(){
        if(currentDoorAnimation != null){
            StopCoroutine(currentDoorAnimation);
            currentDoorAnimation = null;
        }
    }
}
