using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour{
    private PlayerInput playerInput;

    private const string INGAME = "InGamePlayer";
    private const string UI = "UI";
    private const string CUTSCENE = "Cutscene";

    public event EventHandler OnAcceptInput;
    public event EventHandler OnCancelInput;
    //Need to update navigation
    public event EventHandler OnNavigateInput;
    public event EventHandler OnSkipInput;


    private void Awake() {
        TryGetComponent(out playerInput);
    }

    private void Start() {
        GameManager.OnGameStateChange += GameStateChanged;
    }

    private void GameStateChanged(object sender, GameManager.GameStateEventArgs e){
        switch (e.State){
            case GameState.Game: playerInput.SwitchCurrentActionMap(INGAME);
                break;
            case GameState.UI: playerInput.SwitchCurrentActionMap(UI);
                break;
            case GameState.Cutscene: playerInput.SwitchCurrentActionMap(CUTSCENE);
                break;
        }
    }

    public void AcceptInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;
        OnAcceptInput?.Invoke(this, EventArgs.Empty);
    }

    public void CancelInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;
        OnCancelInput?.Invoke(this, EventArgs.Empty);
    }

    public void NavigationInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;
        OnNavigateInput?.Invoke(this, EventArgs.Empty);
    }

    public void SkipInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;
        OnSkipInput?.Invoke(this, EventArgs.Empty);
    }
}
