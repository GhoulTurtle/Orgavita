using UnityEngine;
using UnityEngine.Events;

public class SwitchInteractable : MonoBehaviour, IInteractable{
    public string InteractionPrompt {get{
        string prompt = !currentState ? "Activate" : "Deactivate";
        return prompt;
    }}

    [SerializeField] private bool currentState;

    [Header("Switch Events")]
    [SerializeField] private UnityEvent OnActivateEvent;
    [SerializeField] private UnityEvent OnDeactivateEvent; 

    public bool GetSwitchState(){
        return currentState;
    }

    public bool Interact(PlayerInteract player){
        currentState = !currentState;
        UnityEvent eventToTrigger = currentState ? OnActivateEvent : OnDeactivateEvent;

        eventToTrigger?.Invoke();

        return currentState;
    }
}