using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class DialogueInteractable : MonoBehaviour, IInteractable{
    public abstract string InteractionPrompt {get;}
    [SerializeField] protected bool isCancelable = true;
    [SerializeField] protected bool waitBeforePrinting;
    [SerializeField] protected float waitTimeBeforePrintingInSeconds;

    [Header("Inspect Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    protected PlayerInputHandler playerInputHandler;
    protected PlayerEquippedItemHandler playerEquippedItemHandler;

    protected TextBoxUI textBoxUI => TextBoxUI.Instance;

    protected const float ACCEPT_INPUT_BUFFER_TIME_IN_SECONDS = 0.15f;

    protected IEnumerator currentInputBufferCoroutine;

    public class DialogueEventArgs : EventArgs{
        public Dialogue[] incomingDialogue;

        public DialogueEventArgs(Dialogue[] _incomingDialogue){
            incomingDialogue = _incomingDialogue;
        }
    }

    public virtual bool Interact(PlayerInteract player){
        if(playerInputHandler == null){
            player.TryGetComponent(out playerInputHandler);
        }

        if(playerEquippedItemHandler == null){
            player.TryGetComponent(out playerEquippedItemHandler);
        }

        if(playerEquippedItemHandler != null){
            playerEquippedItemHandler.HolsterEquippedItems();
        }

        OnDialogueStart?.Invoke();
        GameManager.UpdateGameState(GameState.UI);

        textBoxUI.OnCurrentDialogueFinished += (sender, e) => EndDialogue();

        if(waitBeforePrinting){
            StartCoroutine(WaitBeforePrintingCoroutine());
        }
        else{
            StartDialogue();
        }

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
        if(currentInputBufferCoroutine != null){
            StopCoroutine(currentInputBufferCoroutine);
            currentInputBufferCoroutine = null;
        }

        currentInputBufferCoroutine = InputBufferCoroutine();
        StartCoroutine(currentInputBufferCoroutine);
    }

    public virtual void EndDialogue(){
        textBoxUI.StopDialogue();

        GameManager.UpdateGameState(GameState.Game);

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= CancelDialogue;
            playerInputHandler.OnAcceptInput -= ContinueDialogue;
        }

        if(playerEquippedItemHandler != null){
            playerEquippedItemHandler.AttemptUnholsterPreviousActiveItem();
        }

        textBoxUI.OnCurrentDialogueFinished -= (sender, e) => EndDialogue();
                
        OnDialogueEnd?.Invoke();
    }

    public virtual void ContinueDialogue(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed || !textBoxUI.IsTextBoxOpen()) return;

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

        if(playerEquippedItemHandler != null){
            playerEquippedItemHandler.AttemptUnholsterPreviousActiveItem();
        }

        textBoxUI.OnCurrentDialogueFinished -= (sender, e) => EndDialogue();
    }

    public virtual void TriggerDialogueFromGameEvent(PlayerInteract player){
        Interact(player);
    }

    protected virtual IEnumerator InputBufferCoroutine(){
        yield return new WaitForSeconds(ACCEPT_INPUT_BUFFER_TIME_IN_SECONDS);
        
        if(playerInputHandler != null){
            playerInputHandler.OnAcceptInput += ContinueDialogue;
            playerInputHandler.OnCancelInput += CancelDialogue;
        }
    }

    protected virtual IEnumerator WaitBeforePrintingCoroutine(){
        yield return new WaitForSeconds(waitTimeBeforePrintingInSeconds);

        StartDialogue();
    }
}