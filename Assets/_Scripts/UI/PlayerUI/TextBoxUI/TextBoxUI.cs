using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// NOTE: Needs to be refactored, and split up. This does alot.
/// </summary>
public class TextBoxUI : MonoBehaviour{
    public static TextBoxUI Instance;

    public TextMeshProUGUI TextBoxText => textBoxText;

    [Header("UI References")]
    [SerializeField] private Transform textBoxParent;
    [SerializeField] private TextMeshProUGUI textBoxText;
    [SerializeField] private Transform textBoxContinueIndicator;

    [Header("Question UI References")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Transform answerButtonParent;
    [SerializeField] private TextBoxAnswerButtonUI answerButtonUIPrefab;

    [Header("Speaker UI References")]
    [SerializeField] private Transform speakerUIParent;
    [SerializeField] private TextMeshProUGUI speakerText;

    [Header("Text Box Animation Variables")]
    [SerializeField] private float animationDuration = 0.15f;

    [Header("Speaker UI Animation Variables")]
    [SerializeField] private float speakerUIPopupAnimationDuration;
    [SerializeField] private Vector2 speakerUIPopupPosition = new Vector2(1, 2);
    private Vector3 speakerUIOriginalPosition;
    
    [Header("Text Box Indicator Animation Variables")]
    [SerializeField] private float indicatorMoveSpeed = 1.5f;
    [SerializeField] private float indicatorMoveDistance = 0.5f;
    private float indicatorOriginalYPosition;

    private const float closeXScale = 0f;
    private const float openXScale = 1f;

    private Dialogue[] currentDialogue;
    private int currentDialogueIndex = 0;
    private IEnumerator currentTextPrint = null;

    private Dialogue currentQuestion;
    private List<TextBoxAnswerButtonUI> currentAnswerButtonUI = new List<TextBoxAnswerButtonUI>();

    private DialogueSO currentDialogueSO;
    private int currentConversationIndex = 0;
    private IEnumerator currentSpeakerTextPrint = null;

    private ConversationDialogueSO currentConversationSO;

    private IEnumerator currentTextboxAnimation;
    private IEnumerator currentIndicatorAnimation;
    private IEnumerator currentSpeakerUIAnimation;

    public event EventHandler OnCurrentDialogueFinished;
    public event EventHandler OnTextBoxOpen;
    public event EventHandler OnTextBoxClosed;

    private void Awake() {
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }
    
        if(textBoxContinueIndicator != null){
            indicatorOriginalYPosition = textBoxContinueIndicator.localPosition.y; 
        }

        if(speakerUIParent != null){
            speakerUIOriginalPosition = speakerUIParent.localPosition;
        }

        textBoxParent.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void StartDialogue(DialogueSO dialogueSO){
        if(currentDialogueSO != null || dialogueSO == null) return;
        
        ShowTextBox(true);
    
        currentDialogueSO = dialogueSO;

        SetupDialogue();
    }

    private void SetupDialogue(){
        switch (currentDialogueSO){
            case BasicDialogueSO basicDialogueSO: 
            currentDialogue = basicDialogueSO.DialogueSentences;
            PrintNextLine();
            break;
            case ChoiceDialogueSO choiceDialogue:
            currentQuestion = choiceDialogue.Question;
            SetupChoices(choiceDialogue.Choices);
            PrintQuestion();
            break;
            case ConversationDialogueSO conversationDialogueSO:
            currentConversationSO = conversationDialogueSO;
            ShowSpeakerUI(true);
            ProceedConversation();
            break;
            default:
            StopDialogue();
            break;
        }
    }

    private void ProceedConversation(){
        if(currentDialogue == null || currentDialogue.Length <= currentDialogueIndex){
            if(currentConversationSO.Conversation.Length <= currentConversationIndex){
                //End the conversation
                OnCurrentDialogueFinished?.Invoke(this, EventArgs.Empty);
                StopDialogue();
                return;
            }

            //Grab the next conversation dialogue set
            currentDialogue = currentConversationSO.Conversation[currentConversationIndex].SpeakerDialogue;
            //Update the speaker text
            UpdateSpeakerText();

            //Increase the conversation index
            currentConversationIndex++;

            //Reset the dialogue index for the new speaker dialogue
            currentDialogueIndex = 0;
        }

        PrintNextLine();
    }

    private void UpdateSpeakerText(){
        if(currentSpeakerTextPrint != null){
            SpeakerTextFinishedPrinting();
        }

        speakerText.color = currentConversationSO.Conversation[currentConversationIndex].SpeakerName.SentenceColor;
        currentSpeakerTextPrint = TextPrinter.PrintSentence(currentConversationSO.Conversation[currentConversationIndex].SpeakerName.Sentence, speakerText, SpeakerTextFinishedPrinting);
        StartCoroutine(currentSpeakerTextPrint);
    }

    public void AttemptPrintNextLine(){
        if(currentTextPrint != null){
            SentenceFinishedPrinting();

            if(currentQuestion != null){
                questionText.text = currentQuestion.Sentence;
                return;
            }
            
            textBoxText.text = currentDialogue[currentDialogueIndex-1].Sentence;
            return;
        }

        if(currentQuestion != null){
            ShowChoices();
            return;
        }
       
       PrintNextLine();
    }

    public void StopDialogue(){
        if(currentConversationSO != null){
            ShowSpeakerUI(false);
        }

        if(currentTextPrint != null){
            StopCoroutine(currentTextPrint);
            currentTextPrint = null;
        }

        if(currentQuestion != null){
            RemoveAnswerButtonUI();
            HideChoices();
        }

        ShowTextBox(false);

        speakerText.text = "";
        questionText.text = "";
        textBoxText.text = "";

        currentConversationSO = null;
        currentQuestion = null;
        currentDialogue = null;
        currentDialogueSO = null;
        currentDialogueIndex = 0;
        currentConversationIndex = 0;
    }

    private void RemoveAnswerButtonUI(){
        for (int i = 0; i < currentAnswerButtonUI.Count; i++){
            Destroy(currentAnswerButtonUI[i].gameObject);
        }

        currentAnswerButtonUI.Clear();
    }

    private void PrintQuestion(){
        HideTextBoxIndicator();

        currentTextPrint = TextPrinter.PrintSentence(currentQuestion.Sentence, questionText, SentenceFinishedPrinting);
        StartCoroutine(currentTextPrint);

        questionText.color = currentQuestion.SentenceColor;
    }

    private void SetupChoices(ChoiceDialogue[] choices){
        HideChoices();

        for (int i = 0; i < choices.Length; i++){
            var answerButton = Instantiate(answerButtonUIPrefab, answerButtonParent);
            answerButton.SetupAnswerButton(choices[i]);
            currentAnswerButtonUI.Add(answerButton);
        }
    }

    private void ShowChoices(){
        answerButtonParent.gameObject.SetActive(true);
    }

    private void HideChoices(){
        answerButtonParent.gameObject.SetActive(false);
    }

    private void PrintNextLine(){
        HideTextBoxIndicator();
        
        if(currentDialogue.Length <= currentDialogueIndex && currentConversationSO == null){
            OnCurrentDialogueFinished?.Invoke(this, EventArgs.Empty);
            StopDialogue();
            return;
        }

        if(currentConversationSO != null && currentDialogue.Length <= currentDialogueIndex){
            ProceedConversation();
            return;
        }

        currentTextPrint = TextPrinter.PrintSentence(currentDialogue[currentDialogueIndex].Sentence, textBoxText, SentenceFinishedPrinting);
        StartCoroutine(currentTextPrint);
        
        textBoxText.color = currentDialogue[currentDialogueIndex].SentenceColor;
        
        currentDialogueIndex++;
    }

    private void SentenceFinishedPrinting(){
        StopCoroutine(currentTextPrint);
        currentTextPrint = null;

        if(currentQuestion == null){
            ShowTextBoxIndicator();
        }
        else{
            ShowChoices();
        }
    }

    private void SpeakerTextFinishedPrinting(){
        StopCoroutine(currentSpeakerTextPrint);
        currentSpeakerTextPrint = null;
    }

    private void ShowTextBoxIndicator(){
        textBoxContinueIndicator.gameObject.SetActive(true);
        
        currentIndicatorAnimation = UIAnimator.SinAnimationCoroutine(textBoxContinueIndicator, indicatorOriginalYPosition, indicatorMoveSpeed, indicatorMoveDistance);

        StartCoroutine(currentIndicatorAnimation);
    }

    private void HideTextBoxIndicator(){
        textBoxContinueIndicator.gameObject.SetActive(false);
        if(currentIndicatorAnimation != null){
            StopCoroutine(currentIndicatorAnimation);
            currentIndicatorAnimation = null;
        }
    }

    private void ShowTextBox(bool isOpening){
        if(currentTextboxAnimation != null){
            StopCoroutine(currentTextboxAnimation);
            currentTextboxAnimation = null;
        }       

        if(isOpening){
            textBoxParent.gameObject.SetActive(true);
            textBoxParent.localScale = new Vector3(closeXScale, textBoxParent.localScale.y, textBoxParent.localScale.z);
            OnTextBoxOpen?.Invoke(this, EventArgs.Empty);
        }
        else{
            textBoxParent.localScale = new Vector3(openXScale, textBoxParent.localScale.y, textBoxParent.localScale.z);
            OnTextBoxClosed?.Invoke(this, EventArgs.Empty);            
        }

        Vector3 textBoxGoalScale = isOpening ? new Vector3(openXScale, textBoxParent.localScale.y, textBoxParent.localScale.z) : 
                                           new Vector3(closeXScale, textBoxParent.localScale.y, textBoxParent.localScale.z);
        currentTextboxAnimation = UIAnimator.StretchAnimationCoroutine(textBoxParent, textBoxGoalScale, animationDuration, !isOpening);

        StartCoroutine(currentTextboxAnimation);
    }

    private void ShowSpeakerUI(bool isPoppingUp){
        if(currentSpeakerUIAnimation != null){
            StopCoroutine(currentSpeakerUIAnimation);
            currentSpeakerUIAnimation = null;
        }

        if(isPoppingUp){
            speakerUIParent.gameObject.SetActive(true);
            speakerUIParent.localPosition = speakerUIOriginalPosition;
        }
        else{
            speakerUIParent.localPosition = speakerUIPopupPosition;
        }

        Vector3 speakerUIGoalPosition = isPoppingUp ? speakerUIPopupPosition : speakerUIOriginalPosition;

        currentSpeakerUIAnimation = UIAnimator.LerpingAnimationCoroutine(speakerUIParent, speakerUIGoalPosition, speakerUIPopupAnimationDuration, !isPoppingUp);
        StartCoroutine(currentSpeakerUIAnimation);
    }

    public bool IsTextBoxOpen(){
        return textBoxParent.gameObject.activeInHierarchy;
    }
}