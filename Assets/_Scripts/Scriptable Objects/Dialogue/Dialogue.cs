using UnityEngine;

[System.Serializable]
public class Dialogue{
    public SpeakerSO speakerSO;
    public bool hasDialogueEvent;
    [TextArea] public string sentence = "";

    public bool HasSpeaker(){
        return speakerSO != null && speakerSO.speakerName != "";
    }
}