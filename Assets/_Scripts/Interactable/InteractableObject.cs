using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable{
    [Header("Interact Object Variables")]
    [SerializeField] protected string interactionPrompt;

    [Header("Interact Object Events")]
    [SerializeField] protected UnityEvent<PlayerInteract> OnInteractEvent;

    public string InteractionPrompt => interactionPrompt;

    public bool Interact(PlayerInteract player){
        OnInteract(player);

        return true;
    }

    public virtual void OnInteract(PlayerInteract player){
        OnInteractEvent?.Invoke(player);
    }
}
