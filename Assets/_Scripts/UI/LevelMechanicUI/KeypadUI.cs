using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeypadUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private KeypadInteractable keypadInteractable;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private List<ObjectUIButton> buttons;

    [Header("Color Variables")]
    [SerializeField] private Color incorrectAnswerTextColor = Color.red;
    [SerializeField] private Color correntAnswerTextColor = Color.green;

    private void Start() {
        answerText.text = "";
        ChangeButtonsIsEnabled(false);

        if(keypadInteractable == null) return;
        keypadInteractable.OnNumberEntered += UpdateAnswerText;
        keypadInteractable.OnKeypadCleared += ClearAnswerText;
        keypadInteractable.OnCorrectPasswordEntered += CorrectPasswordEntered;
        keypadInteractable.OnIncorrectPasswordEntered += IncorrectPasswordEntered;
        keypadInteractable.OnTriggerState += (sender, e) => ChangeButtonsIsEnabled(true);
        keypadInteractable.OnExitState += (sender, e) => ChangeButtonsIsEnabled(false);
    }

    private void OnDestroy() {
        if(keypadInteractable == null) return;
        keypadInteractable.OnNumberEntered -= UpdateAnswerText;
        keypadInteractable.OnCorrectPasswordEntered -= CorrectPasswordEntered;
        keypadInteractable.OnIncorrectPasswordEntered -= IncorrectPasswordEntered;
        keypadInteractable.OnTriggerState -= (sender, e) => ChangeButtonsIsEnabled(true);
        keypadInteractable.OnExitState -= (sender, e) => ChangeButtonsIsEnabled(false);
        keypadInteractable.OnTriggerState -= (sender, e) => ChangeButtonsIsEnabled(true);
        keypadInteractable.OnExitState -= (sender, e) => ChangeButtonsIsEnabled(false);
    }

    private void CorrectPasswordEntered(object sender, EventArgs e){
        answerText.color = correntAnswerTextColor;
    }

    private void IncorrectPasswordEntered(object sender, EventArgs e){
        answerText.color = incorrectAnswerTextColor;
    }

    private void ClearAnswerText(object sender, EventArgs e){
        answerText.text = "";
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
