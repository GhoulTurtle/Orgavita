using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPause : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Required References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private void Awake() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }

    private void OnDestroy() {
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= UnPausedInput; 
        }
    }

    public void PausedInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed || GameManager.CurrentState != GameState.Game) return;

        GameManager.UpdateGameState(GameState.UI);
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput += UnPausedInput; 
        }
    }

    public void UnPausedInput(object sender, PlayerInputHandler.InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed || GameManager.CurrentState != GameState.UI) return;

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= UnPausedInput; 
        }

        UnPauseGame();
    }

    public void UnPauseGame(){
        GameManager.UpdateGameState(GameState.Game);
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }
}