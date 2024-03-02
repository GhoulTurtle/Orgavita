using System;
using UnityEngine;
using Cinemachine;

public abstract class StateChangeInteractable : MonoBehaviour, IInteractable{
    [Header("Required References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private PlayerInputHandler playerInputHandler;

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
        Cursor.visible = true;
        virtualCamera.Priority = 11;
    }

    public virtual void ExitState(object sender, EventArgs e){
        GameManager.UpdateGameState(GameState.Game);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        virtualCamera.Priority = 9;

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= ExitState;
        }
    }
}
