using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue", fileName = "NewBasicDialogue")]
public class DialogueSO : ScriptableObject{
    [Header("Dialogue Variables")]
    public Dialogue[] dialogueSentences;
    public Response[] responses;

    public bool HasResponses => responses != null && responses.Length > 0;
    public bool HasDialogueEvents(out int eventsAmount, out List<string> eventNames){
        eventsAmount = 0;
        eventNames = new List<string>();
        
        for (int i = 0; i < dialogueSentences.Length; i++){
            if(dialogueSentences[i].hasDialogueEvent){
                eventsAmount++;
                eventNames.Add(dialogueSentences[i].sentence);
            }
        }

        if(eventsAmount > 0) return true;
        return false;
    }

    public int GetDialogueEventIndex(Dialogue dialogue){
        int eventIndex = 0;

        for (int i = 0; i < dialogueSentences.Length; i++){
            if(dialogueSentences[i] == dialogue) return eventIndex;
            if(dialogueSentences[i].hasDialogueEvent){
                eventIndex++;
            }
        }

        return -1;
    }
}