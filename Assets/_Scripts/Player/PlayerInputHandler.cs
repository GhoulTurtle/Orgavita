using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInputHandler : MonoBehaviour{
    private PlayerInput playerInput;

    private const string INGAME = "InGamePlayer";
    private const string UI = "UI";
    private const string CUTSCENE = "Cutscene";

    //UI inputs
    public event EventHandler<InputEventArgs> OnAcceptInput;
    public event EventHandler<InputEventArgs> OnCancelInput;
    //Need to update navigation
    public event EventHandler<InputEventArgs> OnNavigateInput;
    public event EventHandler<InputEventArgs> OnSkipInput;
    public event EventHandler<InputEventArgs> OnClickInput;

    //Equippable item inputs
    public EventHandler<InputEventArgs> OnWeaponUse;
    public EventHandler<InputEventArgs> OnAltWeaponUse;
    public EventHandler<InputEventArgs> OnEmergencyItemUse;
    public EventHandler<InputEventArgs> OnHolsterWeapon;
    public EventHandler<InputEventArgs> OnReload;
    public EventHandler<InputEventArgs> OnInspect;

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

    public void WeaponUseInput(InputAction.CallbackContext context){
        OnWeaponUse?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void WeaponAltUseInput(InputAction.CallbackContext context){
        OnAltWeaponUse?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void EmergencyItemUseInput(InputAction.CallbackContext context){
        OnEmergencyItemUse?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void HolsterWeaponInput(InputAction.CallbackContext context){
        OnHolsterWeapon?.Invoke(this, new InputEventArgs(context.phase, context));
    }

    public void ReloadWeaponInput(InputAction.CallbackContext context){
        if(context.interaction is HoldInteraction){
            OnInspect?.Invoke(this, new InputEventArgs(context.phase, context));
        }
        else if(context.interaction is TapInteraction){
            OnReload?.Invoke(this, new InputEventArgs(context.phase, context));
        }
    }
}
