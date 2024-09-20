using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable{
    [Header("Interact Object Variables")]
    [SerializeField] private string interactionPrompt;
    [SerializeField] private bool isOneShot = false;

    [Header("Interact Object Events")]
    [SerializeField] private UnityEvent<PlayerInteract> OnInteractEvent;

    public string InteractionPrompt => interactionPrompt;

    private bool hasInteracted = false;

    public bool Interact(PlayerInteract player){
        if(isOneShot && hasInteracted) return false;
        OnInteract(player);

        return true;
    }

    public virtual void OnInteract(PlayerInteract player){
        if(isOneShot){
            hasInteracted = true;
            interactionPrompt = "";
        }

        OnInteractEvent?.Invoke(player);
    }
}
