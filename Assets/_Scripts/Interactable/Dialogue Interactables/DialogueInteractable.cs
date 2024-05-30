using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class DialogueInteractable : MonoBehaviour, IInteractable{
    public abstract string InteractionPrompt {get;}
    public bool isCancelable = true;
    public bool waitBeforePrinting;
    public float waitTimeBeforePrintingInSeconds;

    [Header("Inspect Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    protected PlayerInputHandler playerInputHandler;

    protected TextBoxUI textBoxUI => TextBoxUI.Instance;

    public class DialogueEventArgs : EventArgs{
        public Dialogue[] incomingDialogue;

        public DialogueEventArgs(Dialogue[] _incomingDialogue){
            incomingDialogue = _incomingDialogue;
        }
    }

    public virtual bool Interact(PlayerInteract player){
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
        StopAllCoroutines();
    }

    public virtual void StartDialogue(){
        GameManager.UpdateGameState(GameState.UI);
        textBoxUI.OnCurrentDialogueFinished += (sender, e) => EndDialogue();

        OnDialogueStart?.Invoke();
    }

    public virtual void EndDialogue(){
        textBoxUI.StopDialogue();

        GameManager.UpdateGameState(GameState.Game);

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= CancelDialogue;
            playerInputHandler.OnAcceptInput -= ContinueDialogue;
        }

        textBoxUI.OnCurrentDialogueFinished -= (sender, e) => EndDialogue();
                
        OnDialogueEnd?.Invoke();
    }

    public virtual void ContinueDialogue(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;

        textBoxUI.AttemptPrintNextLine();
    }

    public virtual void CancelDialogue(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed || !isCancelable) return;
        
        textBoxUI.StopDialogue();

        GameManager.UpdateGameState(GameState.Game);

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= CancelDialogue;
            playerInputHandler.OnAcceptInput -= ContinueDialogue;
        }

        textBoxUI.OnCurrentDialogueFinished -= (sender, e) => EndDialogue();
    }

    public virtual void TriggerDialogueFromGameEvent(PlayerInteract player){
        if(waitBeforePrinting){
            StartCoroutine(WaitBeforePrintingCoroutine(player));
            return;
        }

        Interact(player);
    }

    private IEnumerator WaitBeforePrintingCoroutine(PlayerInteract player){
        yield return new WaitForSeconds(waitTimeBeforePrintingInSeconds);
        Interact(player);
    }
}