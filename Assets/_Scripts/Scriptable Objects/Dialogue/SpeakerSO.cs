using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Speaker", fileName = "NewSpeaker")]
public class SpeakerSO : ScriptableObject{
    public string speakerName;
    public Color speakerColor = Color.white;
    public AudioEvent speakerVoice;
}