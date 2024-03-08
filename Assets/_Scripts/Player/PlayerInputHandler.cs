using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour{
    private PlayerInput playerInput;

    private const string INGAME = "InGamePlayer";
    private const string UI = "UI";
    private const string CUTSCENE = "Cutscene";

    public event EventHandler<InputEventArgs> OnAcceptInput;
    public event EventHandler<InputEventArgs> OnCancelInput;
    //Need to update navigation
    public event EventHandler<InputEventArgs> OnNavigateInput;
    public event EventHandler<InputEventArgs> OnSkipInput;
    public event EventHandler<InputEventArgs> OnClickInput;

    public class InputEventArgs : EventArgs{
        public InputActionPhase inputActionPhase;
        public InputAction.CallbackContext callbackContext;

        public InputEventArgs(InputActionPhase _inputActionPhase, InputAction.CallbackContext _callbackContext){
            inputActionPhase = _inputActionPhase;
            callbackContext = _callbackContext;
        }
    }

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
        OnAcceptInput?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void CancelInput(InputAction.CallbackContext context){
        OnCancelInput?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void NavigationInput(InputAction.CallbackContext context){
        OnNavigateInput?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void SkipInput(InputAction.CallbackContext context){
        OnSkipInput?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void ClickInput(InputAction.CallbackContext context){
        OnClickInput?.Invoke(this, new InputEventArgs(context.phase, context));
    }
}
