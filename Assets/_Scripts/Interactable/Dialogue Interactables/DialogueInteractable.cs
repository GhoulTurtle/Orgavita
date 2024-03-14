using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class DialogueInteractable : MonoBehaviour, IInteractable{
    public abstract string InteractionPrompt {get;}

    protected PlayerInputHandler playerInputHandler;

    protected TextBoxUI textBoxUI => TextBoxUI.Instance;

    public class DialogueEventArgs : EventArgs{
        public Dialogue[] incomingDialogue;

        public DialogueEventArgs(Dialogue[] _incomingDialogue){
            incomingDialogue = _incomingDialogue;
        }
    }

    public bool Interact(PlayerInteract player){
        if(player.TryGetComponent(out playerInputHandler)){
            playerInputHandler.OnCancelInput += CancelDialogue;
            playerInputHandler.OnAcceptInput += ContinueDialogue;
        }

        StartDialogue();
        return true;
    }

    private void OnDestroy() {
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= CancelDialogue;
            playerInputHandler.OnAcceptInput -= ContinueDialogue;
        }

        textBoxUI.OnCurrentDialogueFinished -= (sender, e) => EndDialogue();
    }

    public virtual void StartDialogue(){
        GameManager.UpdateGameState(GameState.UI);
        textBoxUI.OnCurrentDialogueFinished += (sender, e) => EndDialogue();
    }

    public virtual void EndDialogue(){
        textBoxUI.StopDialogue();

        GameManager.UpdateGameState(GameState.Game);

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= CancelDialogue;
            playerInputHandler.OnAcceptInput -= ContinueDialogue;
        }

        textBoxUI.OnCurrentDialogueFinished -= (sender, e) => EndDialogue();
    }

    public virtual void ContinueDialogue(object sender, PlayerInputHandler.InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;

        textBoxUI.AttemptPrintNextLine();
    }

    public virtual void CancelDialogue(object sender, PlayerInputHandler.InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        
        textBoxUI.StopDialogue();

        GameManager.UpdateGameState(GameState.Game);

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= CancelDialogue;
            playerInputHandler.OnAcceptInput -= ContinueDialogue;
        }

        textBoxUI.OnCurrentDialogueFinished -= (sender, e) => EndDialogue();
    }
}