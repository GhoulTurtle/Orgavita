using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conversation Dialogue", fileName = "NewConversationDialogue")]
public class ConversationDialogueSO : DialogueSO{
    public string conversationInspectNameText = "Name";
    public ConversationDialogue[] Conversation;
}