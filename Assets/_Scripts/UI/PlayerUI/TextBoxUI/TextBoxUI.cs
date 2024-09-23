using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// NOTE: Needs to be refactored, and split up. This does alot.
/// </summary>
public class TextBoxUI : MonoBehaviour{
    public static TextBoxUI Instance;

    public TextMeshProUGUI TextBoxText => textBoxText;

    [Header("Required References")]
    [SerializeField] private ResponseUI responseUI;

    [Header("UI References")]
    [SerializeField] private Transform textBoxParent;
    [SerializeField] private TextMeshProUGUI textBoxText;
    [SerializeField] private Transform textBoxContinueIndicator;

    [Header("Speaker UI References")]
    [SerializeField] private Transform speakerUIParent;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private Image speakerBoxBorder;

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
    private List<IEnumerator> currentTextAnimations = new List<IEnumerator>();
    private int currentDialogueIndex = 0;
    private IEnumerator currentTextPrint = null;

    private SpeakerSO currentSpeakerSO;
    private DialogueSO currentDialogueSO;
    private List<DialogueEventHandler> currentDialogueEventHandlers;
    private IEnumerator currentSpeakerTextPrint = null;

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
        speakerUIParent.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void SetDialogueEventHandlers(List<DialogueEventHandler> dialogueEventHandlers){
        currentDialogueEventHandlers = dialogueEventHandlers;
    }

    public void StartDialogue(DialogueSO dialogueSO){
        if(dialogueSO == null) return;
        currentDialogueSO = dialogueSO;
        currentDialogue = currentDialogueSO.dialogueSentences;
        currentDialogueIndex = 0;

        if(!IsTextBoxOpen()){
            ShowTextBox(true);
        }

        //Get the current dialogue handler
        if(currentDialogueEventHandlers != null){
            DialogueEventHandler dialogueEventHandler = GetCurrentDialogueEventHandler();
            if(dialogueEventHandler != null){
                responseUI.AddDialogueEvents(dialogueEventHandler.ResponseEvents);
            }
            else{
                responseUI.ClearDialogueEvents();
            }
        }   

        PrintNextLine();
    }

    public void AttemptPrintNextLine(){
        if(currentTextPrint != null){
            SentenceFinishedPrinting();
            
            textBoxText.text = currentDialogue[currentDialogueIndex-1].sentence;
            return;
        }
       
       PrintNextLine();
    }

    public void StopDialogue(){
        OnCurrentDialogueFinished?.Invoke(this, EventArgs.Empty);

        if (currentTextPrint != null){
            StopCoroutine(currentTextPrint);
            currentTextPrint = null;
        }

        StopCurrentTextEffects();

        ShowTextBox(false);
        ShowSpeakerUI(false);

        speakerText.text = "";
        textBoxText.text = "";

        currentSpeakerSO = null;
        currentDialogue = null;
        currentDialogueSO = null;
        currentDialogueIndex = 0;
    }

    private void StopCurrentTextEffects(){
        if (currentTextAnimations.Count > 0){
            for (int i = 0; i < currentTextAnimations.Count; i++){
                StopCoroutine(currentTextAnimations[i]);
            }

            currentTextAnimations.Clear();
        }
    }

    private void PrintNextLine(){
        HideTextBoxIndicator();

        //Stop the current animations if any
        StopCurrentTextEffects();

        if(currentDialogue.Length <= currentDialogueIndex){
            if(currentDialogueSO.HasResponses){
                if(IsSpeakerBoxOpen()){
                    ShowSpeakerUI(false);
                }
                
                responseUI.ShowResponses(currentDialogueSO.responses);
            }
            else{
                StopDialogue();
            }
            return;
        }

        //if the current dialogue has a speaker then show the speaker
        if(currentDialogue[currentDialogueIndex].HasSpeaker()){            
            if(!IsSpeakerBoxOpen()){
                ShowSpeakerUI(true);
            }

            if(currentSpeakerSO != currentDialogue[currentDialogueIndex].speakerSO){
                currentSpeakerSO = currentDialogue[currentDialogueIndex].speakerSO;
                
                speakerText.color = currentSpeakerSO.speakerColor;
                speakerBoxBorder.color = currentSpeakerSO.speakerColor;

                if(currentSpeakerTextPrint != null){
                    StopCoroutine(currentSpeakerTextPrint);
                    currentSpeakerTextPrint = null;
                }
                
                //Print the speaker name
                currentSpeakerTextPrint = TextPrinter.PrintSentence(currentSpeakerSO.speakerName, speakerText, SpeakerTextFinishedPrinting);
                StartCoroutine(currentSpeakerTextPrint);
            }
        }
        else if(IsSpeakerBoxOpen()){
            ShowSpeakerUI(false);
        }

        TextContentProfile parsedText = TextParser.Parse(currentDialogue[currentDialogueIndex].sentence);
        //Animate the text
        UIAnimator.StartTextAnimations(this, textBoxText, parsedText, out currentTextAnimations);

        currentTextPrint = TextPrinter.PrintSentence(parsedText.textContent, textBoxText, SentenceFinishedPrinting);
        StartCoroutine(currentTextPrint);
        
        //See if the current dialogue has a event to trigger
        if(currentDialogue[currentDialogueIndex].hasDialogueEvent && currentDialogueEventHandlers != null){
            DialogueEventHandler dialogueEventHandler = GetCurrentDialogueEventHandler();

            if(dialogueEventHandler != null){
                 if(dialogueEventHandler.IsDialogueValid(currentDialogueSO)){
                    int dialogueEventIndex = currentDialogueSO.GetDialogueEventIndex(currentDialogue[currentDialogueIndex]);

                    if(dialogueEventIndex != -1){
                        dialogueEventHandler.DialogueEvents[dialogueEventIndex].OnEventTriggered?.Invoke();
                    }
                }   
            }
        }

        currentDialogueIndex++;
    }

    private DialogueEventHandler GetCurrentDialogueEventHandler(){
        for (int i = 0; i < currentDialogueEventHandlers.Count; i++){
            if(currentDialogueEventHandlers[i].IsDialogueValid(currentDialogueSO)){
                return currentDialogueEventHandlers[i];
            }
        }

        return null;
    } 

    private void SentenceFinishedPrinting(){
        StopCoroutine(currentTextPrint);
        currentTextPrint = null;

        ShowTextBoxIndicator();
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

    public bool ShowingResponses(){
        return responseUI.ShowingResponses();
    }

    private bool IsSpeakerBoxOpen(){
        return speakerUIParent.gameObject.activeInHierarchy;
    }

}