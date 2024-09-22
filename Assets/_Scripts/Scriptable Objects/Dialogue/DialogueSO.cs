using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue", fileName = "NewBasicDialogue")]
public class DialogueSO : ScriptableObject{
    [Header("Dialogue Variables")]
    public Dialogue[] dialogueSentences;
    public Response[] responses;

    public bool HasResponses => responses != null && responses.Length > 0;
}