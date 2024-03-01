using UnityEngine;

public class SwitchInteractable : MonoBehaviour, IInteractable{
    public string InteractionPrompt => "Pull";

    public bool Interact(PlayerInteract player){
		Debug.Log("Pulled Switch!");
		return true;
    }
}
