using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerNoteHandler playerNoteHandler;
    [SerializeField] private Transform noteSpawnPivot;

    [Header("UI References")]
    [SerializeField] private CanvasGroup noteUICanvasGroup;
    [SerializeField] private CanvasGroup noteOverlayCanvasGroup;
    [SerializeField] private Image leftArrowImage;
    [SerializeField] private Image rightArrowImage;
    [SerializeField] private Transform gameIsPausedTextTransform;
    [SerializeField] private TextMeshProUGUI notePageNumberText;
    [SerializeField] private TextMeshProUGUI noteContentText;

    [Header("Note Animation Variables")]
    [SerializeField] private float noteUIFadeTimeInSeconds = 0.15f;
    [SerializeField] private float noteOverlayFadeTimeInSeconds = 3.5f;
    [SerializeField] private float arrowPromptAnimationSpeed;
    [SerializeField] private float arrowPromptAnimationMoveDistance;
    [SerializeField] private float gameIsPausedAnimationSpeed;
    [SerializeField] private float gameIsPausedAnimationMoveDistance;

    private float gameIsPausedOriginalPosY;
    private float leftArrowOriginalPosX;
    private float rightArrowOriginalPosX;

    private Transform currentNoteItemModel;
    private NoteSO currentNoteSO;

    private bool showingNoteContent = false;
    private int contentIndex = -1;

    private IEnumerator currentNoteUIFadeCoroutine;
    private IEnumerator currentNoteOverlayFadeCoroutine;
    private IEnumerator currentGameIsPausedSinCoroutine;
    private IEnumerator currentLeftArrowCosCoroutine;
    private IEnumerator currentRightArrowCosCoroutine;

    private void Awake() {
        gameIsPausedOriginalPosY = gameIsPausedTextTransform.localPosition.y;
        leftArrowOriginalPosX = leftArrowImage.transform.localPosition.x;
        rightArrowOriginalPosX = rightArrowImage.transform.localPosition.x;

        noteUICanvasGroup.alpha = 0;
        noteOverlayCanvasGroup.alpha = 0;

        if(playerNoteHandler == null) return;

        playerNoteHandler.OnShowNote += ShowNoteUI;
        playerNoteHandler.OnHideNote += HideNoteUI;
        playerNoteHandler.OnFlipForward += UpdateNextPageUI;
        playerNoteHandler.OnFlipBackward += UpdatePreviousPageUI;
    }

    private void OnDestroy() {
        StopAllCoroutines();

        if(playerNoteHandler == null) return;

        playerNoteHandler.OnShowNote -= ShowNoteUI;
        playerNoteHandler.OnHideNote -= HideNoteUI;
        playerNoteHandler.OnFlipForward -= UpdateNextPageUI;
        playerNoteHandler.OnFlipBackward -= UpdatePreviousPageUI;
    }

    private void ShowNoteUI(NoteSO note){
        currentNoteSO = note;
        
        noteUICanvasGroup.alpha = 0;
        noteOverlayCanvasGroup.alpha = 0;

        //Spawn the note model
        CreateItemModel(currentNoteSO.noteItemModel);

        //Start the note fade in animation with unscaled delta time
        currentNoteUIFadeCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(noteUICanvasGroup, noteUIFadeTimeInSeconds, 0, 1, false);
        StartCoroutine(currentNoteUIFadeCoroutine);

        //Show only the right arrow
        ShowArrows(false, true);

        //Start the SIN animation on the game is paused text
        currentGameIsPausedSinCoroutine = UIAnimator.SinAnimationCoroutine(gameIsPausedTextTransform, gameIsPausedOriginalPosY, gameIsPausedAnimationSpeed, gameIsPausedAnimationMoveDistance, -1, false);
        StartCoroutine(currentGameIsPausedSinCoroutine);

        //Start the COS animations on the arrows
        currentLeftArrowCosCoroutine = UIAnimator.CosAnimationCoroutine(leftArrowImage.transform, leftArrowOriginalPosX, arrowPromptAnimationSpeed, -arrowPromptAnimationMoveDistance, -1, false);
        StartCoroutine(currentLeftArrowCosCoroutine);

        currentRightArrowCosCoroutine = UIAnimator.CosAnimationCoroutine(rightArrowImage.transform, rightArrowOriginalPosX, arrowPromptAnimationSpeed, arrowPromptAnimationMoveDistance, -1, false);
        StartCoroutine(currentRightArrowCosCoroutine);

        showingNoteContent = false;
    }

    private void HideNoteUI(){
        if(currentNoteUIFadeCoroutine != null){
            StopCoroutine(currentNoteUIFadeCoroutine);
            currentNoteUIFadeCoroutine = null;
        }

        currentNoteUIFadeCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(noteUICanvasGroup, noteUIFadeTimeInSeconds, noteUICanvasGroup.alpha, 0, false, ResetNoteUI);
        StartCoroutine(currentNoteUIFadeCoroutine);
    }

    private void ResetNoteUI(){
        DestroyItemModel();
        currentNoteSO = null;

        showingNoteContent = false;
        contentIndex = -1;

        StopAllCoroutines();
        playerNoteHandler.HideNoteAnimationFinished();
    }

    private void UpdateNextPageUI(){
        if(!showingNoteContent){
           UpdateNoteUIText();

            //Start the overlay fade in animation using unscaled delta time
            if(currentNoteOverlayFadeCoroutine != null){
                StopCoroutine(currentNoteOverlayFadeCoroutine);
                currentNoteOverlayFadeCoroutine = null;
            }

            currentNoteOverlayFadeCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(noteOverlayCanvasGroup, noteOverlayFadeTimeInSeconds, 0, 1, false);
            StartCoroutine(currentNoteOverlayFadeCoroutine);

            //Show the left arrow
            ShowArrows(true, true);

            showingNoteContent = true;
            contentIndex = -1;
            return;
        }

        contentIndex++;
        if(contentIndex >= currentNoteSO.notePages.Count){
            //Exit the note
            playerNoteHandler.UnsubscribeFromInputEvents();
            HideNoteUI();
            return;
        }

        //Update the content text
        UpdateNoteUIText();
    }

    private void UpdatePreviousPageUI(){
        if(!showingNoteContent) return;

        if(contentIndex == -1){
            if(currentNoteOverlayFadeCoroutine != null){
                StopCoroutine(currentNoteOverlayFadeCoroutine);
                currentNoteOverlayFadeCoroutine = null;
            }

            //Start the overlay fade out animation using unscaled delta time
            currentNoteOverlayFadeCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(noteOverlayCanvasGroup, noteOverlayFadeTimeInSeconds, 1, 0, false);
            StartCoroutine(currentNoteOverlayFadeCoroutine);

            //Hide the left arrow
            ShowArrows(false, true);

            showingNoteContent = false;
            return;
        }

        contentIndex--;

        UpdateNoteUIText();
    }

    private void CreateItemModel(Transform noteModel){
        if (currentNoteItemModel != null){
            DestroyItemModel();
        }

        if (noteModel == null) return;

        currentNoteItemModel = Instantiate(noteModel, noteSpawnPivot);
    }

    private void DestroyItemModel(){
        if(currentNoteItemModel != null){
            Destroy(currentNoteItemModel.gameObject);
            currentNoteItemModel = null;
        }
    }

    private void ShowArrows(bool showLeft, bool showRight){
        leftArrowImage.enabled = showLeft;
        rightArrowImage.enabled = showRight;
    }

    private void UpdateNoteUIText(){
        if(contentIndex == -1){
            //Update note content with the note title
            noteContentText.text = currentNoteSO.noteTitle;

            //Update the note page text with 0/NotePageNumber
            notePageNumberText.text = "0 / " + currentNoteSO.notePages.Count;
            return;
        }

        if(contentIndex >= currentNoteSO.notePages.Count) return;

        string notePage = currentNoteSO.notePages[contentIndex];

        if(notePage != null){
            //Update the content text
            noteContentText.text = notePage;

            int adjustedPageNumber = contentIndex + 1;

            notePageNumberText.text = adjustedPageNumber + " / " + currentNoteSO.notePages.Count;
        }
    }
}
