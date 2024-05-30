using UnityEngine;

public class ConversationInteractable : DialogueInteractable{
    public override string InteractionPrompt {get{
        if(conversationDialogueSO != null){
            return "Talk to " + conversationDialogueSO.conversationInspectNameText;
        }
        
        return "";
    }}

    [Header("Conversation Variables")]
    [SerializeField] private ConversationDialogueSO conversationDialogueSO;

    public override void TriggerDialogueFromGameEvent(PlayerInteract player){
        base.TriggerDialogueFromGameEvent(player);
    }

    public override void StartDialogue(){
        base.StartDialogue();

        textBoxUI.StartDialogue(conversationDialogueSO);
    }

    public override void CancelDialogue(object sender, InputEventArgs e){
        base.CancelDialogue(sender, e);
    }

    public override void ContinueDialogue(object sender, InputEventArgs e){
        base.ContinueDialogue(sender, e);
    }
}