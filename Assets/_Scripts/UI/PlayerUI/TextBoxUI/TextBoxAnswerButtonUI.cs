using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxAnswerButtonUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI answerButtonText;
    [SerializeField] private Button answerButton;

    private ChoiceDialogue answerButtonChoiceDialogue;

    public void SetupAnswerButton(ChoiceDialogue _answerButtonChoiceDialogue){
        answerButtonChoiceDialogue = _answerButtonChoiceDialogue;

        answerButtonText.text = answerButtonChoiceDialogue.Sentence;
        answerButtonText.color = answerButtonChoiceDialogue.SentenceColor;

        //Trigger the text effect
    }

    public void TriggerAnswerEvent(){
        answerButtonChoiceDialogue.ChooseChoice();
    }
}