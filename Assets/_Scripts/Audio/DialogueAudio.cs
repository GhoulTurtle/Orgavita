using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueAudio : MonoBehaviour{
    [Header("Dialogue Event")]
    [SerializeField] private UnityEvent TextBoxPrintingEvent;

    private void Start() {
        TextBoxUI.Instance.OnTextBoxOpen += ListenForTextPrinting;
        TextBoxUI.Instance.OnTextBoxClosed += StopListeningForTextPrinting;      
    }

    private void ListenForTextPrinting(object sender, EventArgs e){
        TextPrinter.OnTypeDivisible += InvokeTextPrintingEvent;
    }

    private void StopListeningForTextPrinting(object sender, EventArgs e){
        TextPrinter.OnTypeDivisible -= InvokeTextPrintingEvent;
    }

    private void InvokeTextPrintingEvent(object sender, EventArgs e){
        TextBoxPrintingEvent?.Invoke();
    }

    private void OnDestroy() {
        TextPrinter.OnTypeDivisible -= InvokeTextPrintingEvent;      
        TextBoxUI.Instance.OnTextBoxOpen -= ListenForTextPrinting;
        TextBoxUI.Instance.OnTextBoxClosed -= StopListeningForTextPrinting;      
    }
}