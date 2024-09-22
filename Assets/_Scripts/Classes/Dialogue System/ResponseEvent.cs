using UnityEngine;
using UnityEngine.Events;

public class DialogueEvent : MonoBehaviour{
    [HideInInspector] public string eventName;
    public UnityEvent onEventTriggered;
}