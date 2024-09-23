using UnityEngine;

public class NPCInteractable : DialogueInteractable{
    public override string InteractionPrompt => "Talk to " +  nPCName;
    
    [Header("NPC Variables")]
    [SerializeField] private DialogueSO dialogueSO;
    [SerializeField] private string nPCName = "???"; 

    public void UpdateDialogueSO(DialogueSO _dialogueSO){
        if(dialogueSO == _dialogueSO) return;
        
        dialogueSO = _dialogueSO;
    }

    public void UpdateNPCName(string _nPCName){
        nPCName = _nPCName;
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public override void StartDialogue(){
        if(dialogueSO == null) return;

        base.StartDialogue();

        textBoxUI.SetDialogueEventHandlers(dialogueEventHandlers);
        textBoxUI.StartDialogue(dialogueSO);
    }
}
