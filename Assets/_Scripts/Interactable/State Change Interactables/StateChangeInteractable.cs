using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public abstract class StateChangeInteractable : MonoBehaviour, IInteractable{
    [Header("Required References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("State Change Options")]
    [SerializeField] private bool showCursor = true;

    protected PlayerInputHandler playerInputHandler;

    public abstract string InteractionPrompt {get;}
    public bool Interact(PlayerInteract player){
        if(player.TryGetComponent(out playerInputHandler)){
            playerInputHandler.OnCancelInput += ExitState;
        }

        EnterState();
        return true;
    }

    private void OnDestroy() {
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= ExitState;
        }
    }

    public event EventHandler OnTriggerState;
    public event EventHandler OnExitState;

    public virtual void EnterState(){
        GameManager.UpdateGameState(GameState.UI);
        Cursor.lockState = CursorLockMode.Confined;
        if(showCursor){
            Cursor.visible = true;
        }
        else{
            Cursor.visible = false;
        }
        virtualCamera.Priority = 11;
        OnTriggerState?.Invoke(this, EventArgs.Empty);
    }

    public virtual void ExitState(object sender, PlayerInputHandler.InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        GameManager.UpdateGameState(GameState.Game);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        virtualCamera.Priority = 9;

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= ExitState;
        }
        OnExitState?.Invoke(this, EventArgs.Empty);
    }
}
