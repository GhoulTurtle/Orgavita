using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChoiceInteractable : DialogueInteractable{
    public override string InteractionPrompt => "Inspect";

    [Header("Choice Interactable Variables")]
    public ChoiceDialogueSO choiceDialogueSO;

    public List<UnityEvent> ChoiceEvents = new List<UnityEvent>();

    private void OnDestroy() {
       UnsubscribeFromChoiceEvents();
    }

    public override void StartDialogue(){
        base.StartDialogue();

        SetupChoiceEvents();
    }

    public override void ContinueDialogue(object sender, PlayerInputHandler.InputEventArgs e){
        base.ContinueDialogue(sender, e);

    }

    public override void CancelDialogue(object sender, PlayerInputHandler.InputEventArgs e){
        if(!choiceDialogueSO.isCancelable) return;
        base.CancelDialogue(sender, e);

        UnsubscribeFromChoiceEvents();
    }

    public override void EndDialogue(){
        //When dialogue is over, let the user select a choice.
        base.EndDialogue();

        UnsubscribeFromChoiceEvents();
    }

    private void SetupChoiceEvents(){
        for (int i = 0; i < choiceDialogueSO.Choices.Length; i++){
            choiceDialogueSO.Choices[i].OnChoiceChosen += ChoiceChosen;
        }
    }

    private void UnsubscribeFromChoiceEvents(){
        for (int i = 0; i < choiceDialogueSO.Choices.Length; i++){
            choiceDialogueSO.Choices[i].OnChoiceChosen -= ChoiceChosen;
        }
    }

    public void ChoiceChosen(object sender, EventArgs e){
        ChoiceDialogue choiceDialogue = (ChoiceDialogue)sender;

        int arrayIndex = Array.IndexOf(choiceDialogueSO.Choices, choiceDialogue);
        if(arrayIndex >= 0 && arrayIndex < choiceDialogueSO.Choices.Length){
            ChoiceEvents[arrayIndex]?.Invoke();
        }
    }

    public void UpdateUnityChoiceEvents(){
        if(choiceDialogueSO == null){
            Debug.LogWarning("No valid Choice Dialogue found on: " + gameObject.name + " can't populate ChoiceEvents list.");
            return;
        }

        ChoiceEvents.Clear();

        for (int i = 0; i < choiceDialogueSO.Choices.Length; i++){
            ChoiceEvents.Add(new UnityEvent());
        }
    }
}
