using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour{
	[Header("Required References")]
	[SerializeField] private GameObject playerHUD;
	[SerializeField] private TextMeshProUGUI interactionToolTip;
	[SerializeField] private PlayerInteract playerInteract;

	private bool showUI = true;

	private void Start() {
		interactionToolTip.text = "";
		playerInteract.OnInteractHover += InteractHover;
		GameManager.OnGameStateChange += GameStateChanged;
	}

    private void OnDestroy() {
		playerInteract.OnInteractHover -= InteractHover;		
		GameManager.OnGameStateChange -= GameStateChanged;
	}

    private void GameStateChanged(object sender, GameManager.GameStateEventArgs e){
		if(e.State != GameState.Game){
			showUI = false;
			playerHUD.SetActive(false);
			interactionToolTip.text = "";
			return;
		}

		showUI = true;
		playerHUD.SetActive(true);
    }

    private void InteractHover(object sender, PlayerInteract.InteractableEventArgs e){
		if(e.Interactable == null || !showUI){
			interactionToolTip.text = "";
			return;
		}

		interactionToolTip.text = e.Interactable.InteractionPrompt;
    }
}
