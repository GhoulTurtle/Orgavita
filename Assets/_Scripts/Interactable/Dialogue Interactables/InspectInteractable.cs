using UnityEngine;
using UnityEngine.Events;

public class InspectInteractable : DialogueInteractable{
    public override string InteractionPrompt => "Inspect";
    
    [Header("Inspect Variables")]
    [SerializeField] private BasicDialogueSO inspectDialogue;

    [Header("Inspect Events")]
    public UnityEvent OnDialogueEnd;

    public override void StartDialogue(){
        if(inspectDialogue == null) return;

        base.StartDialogue();

        textBoxUI.StartDialogue(inspectDialogue.DialogueSentences);
    }

    public override void EndDialogue(){
        base.EndDialogue();
        
        OnDialogueEnd?.Invoke();
    }

    public override void ContinueDialogue(object sender, PlayerInputHandler.InputEventArgs e){
        base.ContinueDialogue(sender, e);
    }

    public override void CancelDialogue(object sender, PlayerInputHandler.InputEventArgs e){
        base.CancelDialogue(sender, e);
    }

    public void SetInspectDialogue(BasicDialogueSO dialogueSO){
        if(inspectDialogue == dialogueSO) return;

        inspectDialogue = dialogueSO;
    }
}
