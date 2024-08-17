using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    [Header("Door Events")]
    [SerializeField] private UnityEvent OnDoorOpen;
    [SerializeField] private UnityEvent OnDoorClose;
    [SerializeField] private UnityEvent OnFailedOpen;

    private Vector3 startRotation;

    private IEnumerator currentDoorAnimation;

    private bool isOpen;

    private void Awake() {
        startRotation = transform.rotation.eulerAngles;
    }

    private void OnDestroy() {
        StopDoorAnimation();
    }

    public bool Interact(PlayerInteract player){
        if(isLocked){
            OnFailedOpen?.Invoke();
            return false;
        }

        Open();
        return true;
    }

    public void Open(){
        StopDoorAnimation();

        currentDoorAnimation = DoorAnimation(!isOpen);
        StartCoroutine(currentDoorAnimation);
    }

    public void UnlockDoor(){
        isLocked = false;
    }

    private IEnumerator DoorAnimation(bool openingDoor){
        Quaternion doorStartRotation = transform.rotation;
        Quaternion endRotation;

        if(!openingDoor){
            OnDoorClose?.Invoke();
            endRotation = Quaternion.Euler(startRotation);
        }
        else{
            OnDoorOpen?.Invoke();
            endRotation = Quaternion.Euler(new Vector3(0, doorStartRotation.y + rotationAmount, 0));
        }

        isOpen = openingDoor;

        float current = 0;

        while(current < doorRotationSpeedInSeconds){
            if(Time.timeScale != 0){
                transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, current / doorRotationSpeedInSeconds);
                current += Time.deltaTime;
            }

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
