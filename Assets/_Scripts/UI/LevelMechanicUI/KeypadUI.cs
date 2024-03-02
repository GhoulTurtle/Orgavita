using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeypadUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private KeypadInteractable keypadInteractable;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private List<ObjectUIButton> buttons;

    private void Start() {
        answerText.text = "";
        ChangeButtonsIsEnabled(false);

        if(keypadInteractable == null) return;
        keypadInteractable.OnNumberEntered += UpdateAnswerText;
        keypadInteractable.OnKeypadCleared += ClearAnswerText;
        keypadInteractable.OnCorrectPasswordEntered += CorrectPasswordEntered;
        keypadInteractable.OnTriggerState += (sender, e) => ChangeButtonsIsEnabled(true);
        keypadInteractable.OnExitState += (sender, e) => ChangeButtonsIsEnabled(false);
    }

    private void CorrectPasswordEntered(object sender, EventArgs e){
        answerText.color = Color.green;
    }

    private void ClearAnswerText(object sender, EventArgs e){
        answerText.text = "";
    }

    private void OnDestroy() {
        if(keypadInteractable == null) return;
        keypadInteractable.OnNumberEntered -= UpdateAnswerText;
        keypadInteractable.OnTriggerState -= (sender, e) => ChangeButtonsIsEnabled(true);
        keypadInteractable.OnExitState -= (sender, e) => ChangeButtonsIsEnabled(false);
    }

    private void UpdateAnswerText(object sender, KeypadInteractable.KeypadNumberEnteredEventArgs e){
        answerText.text = e.Entry;
    }

    private void ChangeButtonsIsEnabled(bool enabled){
        for (int i = 0; i < buttons.Count; i++){
            buttons[i].SetIsEnabled(enabled);
        }
    }
}
