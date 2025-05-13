using UnityEngine;

public class InspectInteractable : DialogueInteractable{
    public override string InteractionPrompt => interactionPrompt;
    
    [Header("Inspect Variables")]
    [SerializeField] private DialogueSO inspectDialogue;
    [SerializeField] private string interactionPrompt = "Inspect";

    [Header("Inspect Interactable Settings")]
    [SerializeField] private ItemLevelMechanic connectedItemLevelMechanic;

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public override bool Interact(PlayerInteract player){
        if(connectedItemLevelMechanic != null){
            //If we have a connected level mechanic attempt to unlock it to sortcut having the player needing to open their inventory
            if(connectedItemLevelMechanic.AttemptLevelMechanicInteractionOnInteract(out string resultText)){
                PopupUI.Instance.PrintText(resultText);
                return false;
            }
        }   
        
        return base.Interact(player);
    }

    public override void StartDialogue(){

        if(inspectDialogue == null) return;

        base.StartDialogue();

        textBoxUI.SetDialogueEventHandlers(dialogueEventHandlers);
        textBoxUI.StartDialogue(inspectDialogue);
    }

    public override void EndDialogue(){
        base.EndDialogue();
    }

    public override void ContinueDialogue(object sender, InputEventArgs e){
        base.ContinueDialogue(sender, e);
    }

    public override void CancelDialogue(object sender, InputEventArgs e){
        base.CancelDialogue(sender, e);
    }

    public void SetInspectDialogue(DialogueSO dialogueSO){
        if(inspectDialogue == dialogueSO) return;

        inspectDialogue = dialogueSO;
    }
}