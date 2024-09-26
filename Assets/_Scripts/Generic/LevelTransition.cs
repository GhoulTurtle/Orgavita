using UnityEngine;

public class LevelTransition : MonoBehaviour, IInteractable{
    public string InteractionPrompt => interactionPrompt; 

    [Header("Required References")]
    [SerializeField] private RoomDataSO roomToTransitionTo;
    

    private string interactionPrompt = "Transition to ";

    private void Awake() {
        interactionPrompt += roomToTransitionTo.GetSceneName();
    }

    public bool Interact(PlayerInteract player){
        //Trigger our scene level manager
        return true;
    }
}
