using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Basic Dialogue", fileName = "NewBasicDialogue")]
public class BasicDialogueSO : ScriptableObject{
    [Header("Basic Dialouge Variables")]
    public Dialogue[] DialogueSentences;
}
