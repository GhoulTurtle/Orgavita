using UnityEngine;

public class NoteInteractable : MonoBehaviour, IInteractable{
    [Header("Required References")]
    [SerializeField] private NoteSO noteSO;

    public string InteractionPrompt => "Read";

    private PlayerNoteHandler playerNoteHandler;

    public bool Interact(PlayerInteract player){
        if(noteSO == null) return false;

        if(playerNoteHandler == null){
            if(!player.TryGetComponent(out playerNoteHandler)) return false;
        }
        
        //Display the note, and add it to the player note list
        playerNoteHandler.DisplayNote(noteSO);
        playerNoteHandler.AttemptAddNewNoteToDataList(noteSO);
        return true;
    }
}