using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueEvent{
    [HideInInspector] public string name;
    public UnityEvent OnEventTriggered;
}