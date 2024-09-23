using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResponseUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private ResponseButton responseButtonTemplatePrefab;
    [SerializeField] private RectTransform responseParent;

    [Header("Response UI Variables")]
    [SerializeField] private float responseSelectBuffer = 0.1f;

    private TextBoxUI textBoxUI => TextBoxUI.Instance;

    private DialogueEvent[] responseDialogueEvents;
    private List<ResponseButton> currentResponseButtons = new List<ResponseButton>();

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void AddDialogueEvents(DialogueEvent[] dialogueEvents){
        responseDialogueEvents = dialogueEvents;
    }

    public void ClearDialogueEvents(){
        responseDialogueEvents = null;
    }

    public void ShowResponses(Response[] responses){
        float responseBoxHeight = 0;

        for (int i = 0; i < responses.Length; i++){
            Response response = responses[i];
            int responseIndex = i;
            
            ResponseButton responseButton = Instantiate(responseButtonTemplatePrefab, responseParent);
            responseButton.gameObject.SetActive(true);
            responseButton.SetButtonText(response.responseText);
            responseButton.SetButtonListener(() => OnPickedResponse(response, responseIndex));
            
            currentResponseButtons.Add(responseButton);

            responseBoxHeight += responseButton.GetButtonSizeYDelta();
        }

        StartCoroutine(SelectResponseBufferCoroutine());

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex){
        responseBox.gameObject.SetActive(false);

        foreach (ResponseButton button in currentResponseButtons){
            button.RemoveAllButtonListeners();
            Destroy(button.gameObject);
        }

        currentResponseButtons.Clear();

        EventSystem.current.SetSelectedGameObject(null);

        if (responseDialogueEvents != null && responseIndex <= responseDialogueEvents.Length){
            responseDialogueEvents[responseIndex].OnEventTriggered?.Invoke();
        }

        responseDialogueEvents = null;

        if(response.dialogueSO){
            textBoxUI.StartDialogue(response.dialogueSO);
        }
        else{
            textBoxUI.StopDialogue();
        }
    }

    public bool ShowingResponses(){
        return responseBox.gameObject.activeInHierarchy;
    }

    //TMP Need to be replaced when I redo the UI select system for controller and mouse support
    private IEnumerator SelectResponseBufferCoroutine(){
        yield return new WaitForSeconds(responseSelectBuffer);
        if(currentResponseButtons.Count > 0){
            EventSystem.current.SetSelectedGameObject(currentResponseButtons[0].gameObject);
        }
    }
}
