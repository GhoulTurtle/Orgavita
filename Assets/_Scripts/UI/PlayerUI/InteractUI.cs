using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour{
	[Header("Required References")]
	[SerializeField] private TextMeshProUGUI interactionToolTip;
	[SerializeField] private PlayerInteract playerInteract;

	private void Start() {
		interactionToolTip.text = "";
		playerInteract.OnInteractHover += InteractHover;
	}

	private void OnDestroy() {
		playerInteract.OnInteractHover -= InteractHover;		
	}

    private void InteractHover(object sender, PlayerInteract.InteractableEventArgs e){
		if(e.Interactable == null){
			interactionToolTip.text = "";
			return;
		}

		interactionToolTip.text = e.Interactable.InteractionPrompt;
    }
}
