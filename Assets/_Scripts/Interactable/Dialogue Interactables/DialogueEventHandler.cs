using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventHandler : MonoBehaviour{
    [SerializeField] private DialogueSO dialogueSO;
    [SerializeField] private DialogueEvent[] dialogueEvents;
    [SerializeField] private DialogueEvent[] responseEvents;

    public DialogueEvent[] DialogueEvents => dialogueEvents;
    public DialogueEvent[] ResponseEvents => responseEvents;

    public bool IsDialogueValid(DialogueSO _dialogueSO){
        return dialogueSO == _dialogueSO;
    }

    public void OnValidate(){
        if (dialogueSO == null) return;
        GenerateDialogueEvents();
        GenerateResponseEvents();
    }

    private void GenerateDialogueEvents(){
        if(!dialogueSO.HasDialogueEvents(out int amount, out List<string> eventNames)) return;
        if(dialogueEvents != null && dialogueEvents.Length == amount) return;
        if (dialogueEvents == null){
            dialogueEvents = new DialogueEvent[amount];
        }
        else{
            Array.Resize(ref dialogueEvents, amount);
        }

        for (int i = 0; i < amount; i++){
            if(dialogueEvents[i] != null){
                dialogueEvents[i].name = eventNames[i];
                continue;
            }

            dialogueEvents[i] = new DialogueEvent() {name = eventNames[i]};
        }
    }

    private void GenerateResponseEvents(){
        if (dialogueSO.responses == null) return;
        if (responseEvents != null && responseEvents.Length == dialogueSO.responses.Length) return;
        if (responseEvents == null){
            responseEvents = new DialogueEvent[dialogueSO.responses.Length];
        }
        else{
            Array.Resize(ref responseEvents, dialogueSO.responses.Length);
        }

        for (int i = 0; i < dialogueSO.responses.Length; i++){
            Response response = dialogueSO.responses[i];

            if (responseEvents[i] != null){
                responseEvents[i].name = response.responseText;
                continue;
            }

            responseEvents[i] = new DialogueEvent() { name = response.responseText };
        }
    }
}
