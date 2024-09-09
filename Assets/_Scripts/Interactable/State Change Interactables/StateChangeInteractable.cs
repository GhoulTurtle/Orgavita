using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public abstract class StateChangeInteractable : MonoBehaviour, IInteractable{
    [Header("Required References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("State Change Options")]
    [SerializeField] private bool showCursor = true;
    [SerializeField] protected bool isLocked = false;

    protected PlayerInputHandler playerInputHandler;
    
    public event EventHandler OnTriggerState;
    public event EventHandler OnExitState;
    public event EventHandler OnUnlockInteractable;
    public event EventHandler OnLockInteractable;

    public abstract string InteractionPrompt {get;}

    private InputSystemUIInputModule currentInputSystemUIInputModule;

    public bool Interact(PlayerInteract player){
        if(playerInputHandler == null){
            player.TryGetComponent(out playerInputHandler);
        }

        if(playerInputHandler != null){
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
        currentInputSystemUIInputModule = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
        currentInputSystemUIInputModule.deselectOnBackgroundClick = true;
        
        OnTriggerState?.Invoke(this, EventArgs.Empty);
    }

    public virtual void ExitState(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        GameManager.UpdateGameState(GameState.Game);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        virtualCamera.Priority = 9;

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= ExitState;
        }

        currentInputSystemUIInputModule.deselectOnBackgroundClick = false;

        OnExitState?.Invoke(this, EventArgs.Empty);
    }

    public virtual void UnlockInteractable(){
        isLocked = false;
        OnUnlockInteractable?.Invoke(this, EventArgs.Empty);
    }

    public virtual void LockInteractable(){
        isLocked = true;
        OnLockInteractable?.Invoke(this, EventArgs.Empty);
    }
}