using System;
using UnityEngine;

public class PlayerNoteHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerNoteDataList playerNoteDataList;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [Header("Text Print Speeds")]
    [SerializeField] private float popupPrintSpeed = 0.01f;
    [SerializeField] private float popupWaitTime = 1.5f;
    [SerializeField] private float popupFadeTime = 1f;

    public static PlayerNoteHandler Instance {get; private set;}

    public Action<NoteSO> OnShowNote;
    public Action OnHideNote;
    public Action OnFlipForward;
    public Action OnFlipBackward;

    private bool addedNote = false;

    private void Awake() {
        if (Instance != null){
            Destroy(gameObject);
        }
        else{
            Instance = this;
        }
    }

    private void OnDestroy() {
        if(playerInputHandler == null || Instance != this) return;

        playerInputHandler.OnCancelInput -= HideNote;
        playerInputHandler.OnNavigateInput -= EvaulateNavigateInput;
    }

    public void AttemptAddNewNoteToDataList(NoteSO noteSO){
        if(noteSO  == null) return;
        if(playerNoteDataList.IsNoteInDataList(noteSO)) return;

        playerNoteDataList.AddNewNoteToDataList(noteSO);
        addedNote = true;
    }

    public void DisplayNote(NoteSO noteSO){
        if(noteSO == null) return;

        GameManager.UpdateGameState(GameState.UI);

        playerInputHandler.OnCancelInput += HideNote;
        playerInputHandler.OnNavigateInput += EvaulateNavigateInput;

        OnShowNote?.Invoke(noteSO);
        Time.timeScale = 0;
    }

    private void HideNote(object sender, InputEventArgs e){
        UnsubscribeFromInputEvents();

        OnHideNote?.Invoke();
    }

    public void HideNoteAnimationFinished(){
        AttemptShowNotePopup();

        GameManager.UpdateGameState(GameState.Game);

        Time.timeScale = 1;
    }

    public void AttemptShowNotePopup(){
        if (addedNote){
            NoteSO noteSO = playerNoteDataList.GetLastNoteAdded();

            if (noteSO != null){
                Dialogue noteAddedDialogue = new(){
                    sentence = playerNoteDataList.IsFirstNote() ? $"<color={ColorHelper.mintGreenHash}>Note has been added to the note tab in the inventory.</color>" : $"<color={ColorHelper.mintGreenHash}>Note has been saved to the note tab.</color>",
                };
                PopupUI.Instance.PrintText(noteAddedDialogue, popupPrintSpeed, true, popupWaitTime, popupFadeTime);
            }
            addedNote = false;
        }
    }

    public void UnsubscribeFromInputEvents(){
        playerInputHandler.OnCancelInput -= HideNote;
        playerInputHandler.OnNavigateInput -= EvaulateNavigateInput;
    }

    private void EvaulateNavigateInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != UnityEngine.InputSystem.InputActionPhase.Performed) return;

        Vector2 inputVector = e.callbackContext.ReadValue<Vector2>();

        if(inputVector.x > 0){
            OnFlipForward?.Invoke();
        }
        else if(inputVector.x < 0){
            OnFlipBackward?.Invoke();
        }
    }
}