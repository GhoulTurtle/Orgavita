using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerNoteHandler playerNoteHandler;
    [SerializeField] private Transform noteSpawnPivot;

    [Header("UI References")]
    [SerializeField] private CanvasGroup noteUICanvasGroup;
    [SerializeField] private CanvasGroup noteTextCanvasGroup;
    [SerializeField] private Image leftArrowImage;
    [SerializeField] private Image rightArrowImage;
    [SerializeField] private Transform gameIsPausedTextTransform;
    [SerializeField] private TextMeshProUGUI notePageNumberText;
    [SerializeField] private TextMeshProUGUI noteContentText;

    private Transform currentNoteItemModel;
    private NoteSO currentNoteSO;

    private bool showingNoteContent = false;
    private int contentIndex = 0;

    private void Awake() {
        noteUICanvasGroup.alpha = 0;
        noteTextCanvasGroup.alpha = 0;

        if(playerNoteHandler == null) return;

        playerNoteHandler.OnShowNote += ShowNoteUI;
        playerNoteHandler.OnHideNote += HideNoteUI;
        playerNoteHandler.OnFlipForward += UpdateNextPageUI;
        playerNoteHandler.OnFlipBackward += UpdatePreviousPageUI;
    }

    private void OnDestroy() {
        if(playerNoteHandler == null) return;

        playerNoteHandler.OnShowNote -= ShowNoteUI;
        playerNoteHandler.OnHideNote -= HideNoteUI;
        playerNoteHandler.OnFlipForward -= UpdateNextPageUI;
        playerNoteHandler.OnFlipBackward -= UpdatePreviousPageUI;
    }

    private void ShowNoteUI(NoteSO note){
        currentNoteSO = note;


        //Spawn the note model
        //Start the note fade in animation with unscaled delta time
        //Show only the right arrow
        //Start the COS animation on the right arrow and the SIN animation on the game is paused text
        showingNoteContent = false;
    }

    private void HideNoteUI(){
        DestroyItemModel();
        currentNoteSO = null;

        showingNoteContent = false;
        contentIndex = 0;
    }

    private void DestroyItemModel(){
        if(currentNoteItemModel != null){
            Destroy(currentNoteItemModel.gameObject);
            currentNoteItemModel = null;
        }
    }

    private void UpdateNextPageUI(){
        if(!showingNoteContent){
            //Update note content with the note title
            //Update the note page text with 0/NotePageNumber
            //Start the overlay fade in animation using unscaled delta time
            //Show the left arrow and start the cos animation
            showingNoteContent = true;
            contentIndex = 0;
            return;
        }


    }

    private void UpdatePreviousPageUI(){
    
    }

    private void CreateItemModel(Transform noteModel){
        if (currentNoteItemModel != null){
            DestroyItemModel();
        }

        if (noteModel == null) return;

        currentNoteItemModel = Instantiate(noteModel, noteSpawnPivot);
    }
}
