using System;
using UnityEngine;

public class PlayerNoteHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerNoteDataList playerNoteDataList;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    public Action<NoteSO> OnShowNote;
    public Action OnHideNote;
    public Action OnFlipForward;
    public Action OnFlipBackward;

    private void OnDestroy() {
        if(playerInputHandler == null) return;

        playerInputHandler.OnCancelInput -= HideNote;
        playerInputHandler.OnNavigateInput -= EvaulateNavigateInput;
    }

    public void AttemptAddNewNoteToDataList(NoteSO noteSO){
        if(playerNoteDataList.IsNoteInDataList(noteSO)) return;

        playerNoteDataList.AddNewNoteToDataList(noteSO);
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
        GameManager.UpdateGameState(GameState.Game);

        Time.timeScale = 1;
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