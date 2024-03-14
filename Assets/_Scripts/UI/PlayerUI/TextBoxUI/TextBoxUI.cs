using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBoxUI : MonoBehaviour{
    public static TextBoxUI Instance;

    public TextMeshProUGUI TextBoxText => textBoxText;

    [Header("UI References")]
    [SerializeField] private Transform textBoxParent;
    [SerializeField] private TextMeshProUGUI textBoxText;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private Transform textBoxContinueIndicator;

    [Header("Question UI References")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Transform answerButtonParent;
    [SerializeField] private TextBoxAnswerButtonUI answerButtonUIPrefab;

    [Header("Text Box Animation Variables")]
    [SerializeField] private float animationDuration = 0.15f;
    
    private const float closeXScale = 0f;
    private const float openXScale = 1f;

    [Header("Text Box Indicator Animation Variables")]
    [SerializeField] private float indicatorMoveSpeed = 1.5f;
    [SerializeField] private float indicatorMoveDistance = 0.5f;
    private float indicatorOriginalYPosition;

    private Dialogue[] currentDialogue;
    private int currentDialogueIndex = 0;
    private IEnumerator currentTextPrint = null;

    private Dialogue currentQuestion;
    private List<TextBoxAnswerButtonUI> currentAnswerButtonUI = new List<TextBoxAnswerButtonUI>();

    private IEnumerator currentTextboxAnimation;
    private const float SNAP_DISTANCE = 0.01f;

    private IEnumerator currentIndicatorAnimation;

    public event EventHandler OnCurrentDialogueFinished;

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

        textBoxParent.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void StartDialogue(Dialogue[] dialogue){
        if(currentDialogue != null || currentQuestion != null) return;

        ShowTextBox(true);

        currentDialogue = dialogue;

        PrintNextLine();
    }

    public void StartQuestion(Dialogue questionDialogue, ChoiceDialogue[] choices){
        if(currentDialogue != null || currentQuestion != null) return;
        
        currentQuestion = questionDialogue;

        ShowTextBox(true);
        SetupChoices(choices);

        PrintQuestion();
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
        ShowTextBox(false);

        if(currentTextPrint != null){
            StopCoroutine(currentTextPrint);
            currentTextPrint = null;
        }

        if(currentQuestion != null){
            RemoveAnswerButtonUI();
            HideChoices();
        }

        questionText.text = "";
        textBoxText.text = "";

        currentQuestion = null;
        currentDialogue = null;
        currentDialogueIndex = 0;
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

        AnimateText(questionText, currentQuestion.SentenceDialogueEffect);
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

        if(currentDialogue.Length <= currentDialogueIndex){
            OnCurrentDialogueFinished?.Invoke(this, EventArgs.Empty);
            StopDialogue();
            return;
        }

        currentTextPrint = TextPrinter.PrintSentence(currentDialogue[currentDialogueIndex].Sentence, textBoxText, SentenceFinishedPrinting);
        StartCoroutine(currentTextPrint);
        
        textBoxText.color = currentDialogue[currentDialogueIndex].SentenceColor;
        
        AnimateText(textBoxText, currentDialogue[currentDialogueIndex].SentenceDialogueEffect);
        
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

    private void ShowTextBoxIndicator(){
        textBoxContinueIndicator.gameObject.SetActive(true);
        currentIndicatorAnimation = TextBoxIndicatorCorutine();
        StartCoroutine(TextBoxIndicatorCorutine());
    }

    private void HideTextBoxIndicator(){
        textBoxContinueIndicator.gameObject.SetActive(false);
        if(currentIndicatorAnimation != null){
            StopCoroutine(currentIndicatorAnimation);
            currentIndicatorAnimation = null;
        }
    }

    private void ShowTextBox(bool state){
        if(currentTextboxAnimation != null){
            StopCoroutine(currentTextboxAnimation);
            currentTextboxAnimation = null;
        }       

        if(state){
            textBoxParent.gameObject.SetActive(true);
            textBoxParent.localScale = new Vector3(closeXScale, textBoxParent.localScale.y, textBoxParent.localScale.z);
        }
        else{
            textBoxParent.localScale = new Vector3(openXScale, textBoxParent.localScale.y, textBoxParent.localScale.z);
        }

        currentTextboxAnimation = TextBoxAnimationCorutine(state);

        StartCoroutine(currentTextboxAnimation);
    }

    private IEnumerator TextBoxIndicatorCorutine(){
        while(true){
            textBoxContinueIndicator.localPosition = new Vector3(textBoxContinueIndicator.localPosition.x, SinAmount(), textBoxContinueIndicator.localPosition.z);
            yield return null;
        }
    }

    private float SinAmount(){
        return indicatorOriginalYPosition + Mathf.Sin(Time.time * indicatorMoveSpeed) * indicatorMoveDistance;
    }

    private IEnumerator TextBoxAnimationCorutine(bool isOpening){
        Vector3 goalScale = isOpening ? new Vector3(openXScale, textBoxParent.localScale.y, textBoxParent.localScale.z) : 
                                        new Vector3(closeXScale, textBoxParent.localScale.y, textBoxParent.localScale.z);

        float current = 0;

        while(Mathf.Abs(textBoxParent.localScale.x - goalScale.x) > SNAP_DISTANCE){
            textBoxParent.localScale = Vector3.Lerp(textBoxParent.localScale, goalScale, current / animationDuration);
            current += Time.deltaTime;
            yield return null;
        }

        textBoxParent.localScale = goalScale;

        if(!isOpening){
            textBoxParent.gameObject.SetActive(false);
        }
    }

    private void AnimateText(TextMeshProUGUI textToAnimate, DialogueEffect sentenceDialogueEffect){
        switch (sentenceDialogueEffect){
            case DialogueEffect.None:
                break;
            case DialogueEffect.Wobble:
                break;
            case DialogueEffect.Pulse:
                break;
            case DialogueEffect.Shake:
                break;
        }
    }
}