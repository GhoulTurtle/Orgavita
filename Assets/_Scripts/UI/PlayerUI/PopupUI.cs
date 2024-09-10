using System.Collections;
using TMPro;
using UnityEngine;

public class PopupUI : MonoBehaviour{
    public static PopupUI Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI popupText;

    [Header("Popup Variables")]
    [SerializeField] private float defaultPopupWaitTime = 3f;
    [SerializeField] private float defaultPopupFadeTime = 0.5f;

    private IEnumerator currentPrintCoroutine;
    private IEnumerator currentFadeCoroutine;

    private Color popupFadeColor = Color.clear;

    private float currentWaitTime;
    private float currentFadeTime;
 
    private void Awake() {
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start() {
        popupText.text = "";
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void PrintText(Dialogue dialogueToDisplay, float printTime = -1, bool fadeAfterCompletion = true, float waitTime = -1f, float fadeTime = -1f){
        currentWaitTime = waitTime;
        currentFadeTime = fadeTime;
        
        StopCurrentFade();
        StopCurrentPrint();
        popupText.color = dialogueToDisplay.SentenceColor;
        if(fadeAfterCompletion){
            currentPrintCoroutine = TextPrinter.PrintSentence(dialogueToDisplay.Sentence, popupText, StartPopupFadeCoroutine, printTime);
        }
        else{
            currentPrintCoroutine = TextPrinter.PrintSentence(dialogueToDisplay.Sentence, popupText, null, printTime);
        }

        StartCoroutine(currentPrintCoroutine);
    }

    private void StartPopupFadeCoroutine(){
        StopCurrentFade();
        currentFadeCoroutine = PopupFadeCoroutine(currentWaitTime, currentFadeTime);
        StartCoroutine(currentFadeCoroutine);
    }

    private void StopCurrentPrint(){
        if(currentPrintCoroutine != null){
            StopCoroutine(currentPrintCoroutine);
            currentPrintCoroutine = null;
        }
    }

    private void StopCurrentFade(){
        if(currentFadeCoroutine != null){
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }
    }

    private IEnumerator PopupFadeCoroutine(float waitTime, float fadeTime){
        float popupWaitTime = waitTime;
        if(popupWaitTime == -1f){
            popupWaitTime = defaultPopupWaitTime;
        }

        float popupFadeTime = fadeTime;
        if(popupFadeTime == -1){
            popupFadeTime = defaultPopupFadeTime;
        }

        yield return new WaitForSeconds(popupWaitTime);

        float currentTime = 0f;

        while(currentTime < popupFadeTime){
            popupText.color = Color.Lerp(popupText.color, popupFadeColor, currentTime / popupFadeTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        popupText.color = popupFadeColor;
        popupText.text = "";
    }
}
