using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailApplicationUI : ApplicationUI{
    [Header("UI References")]
    [SerializeField] private VerticalLayoutGroup EmailUIParent;
    [SerializeField] private EmailUI EmailMessageUIPrefab;
    [SerializeField] private TextMeshProUGUI recipientText;
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private TextMeshProUGUI subjectText;

    [Header("Email UI Variables")]
    [SerializeField] private float emailParentStartingSpacing = -350f;
    [SerializeField] private float emailParentSpacingModifier = 50f;

    private EmailComputerApplication emailComputerApplication;
    private List<Email> emailList;

    private Email currentSelectedEmail;

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public override void SetupApplicationUI(ComputerUI _computerUI, ComputerApplication application){
        base.SetupApplicationUI(_computerUI, application);
        emailComputerApplication = (EmailComputerApplication)application;
        emailList = emailComputerApplication.EmailMessageListSO.EmailMessageList;
        currentSelectedEmail = emailList[0];

        EmailUIParent.spacing = emailParentStartingSpacing;

        SetupEmailUI();
    }

    public override void CloseApplication(){
        base.CloseApplication();
    }
    
    public void SelectEmail(Email email){
        if(currentSelectedEmail == email) return;

        currentSelectedEmail = email;
        
        computerUI.UpdateCursor(ComputerCursorState.Loading);

        DisableApplicationUIInteractivity();

        StartCoroutine(EmailLoadCoroutine());
    }

    private void SetupEmailUI(){
        foreach (Email email in emailList){
            Instantiate(EmailMessageUIPrefab, EmailUIParent.transform).SetupEmailUI(this, email);
            EmailUIParent.spacing += emailParentSpacingModifier;
        }

        UpdateCurrentEmailUI();
    }

    private IEnumerator EmailLoadCoroutine(){
        yield return new WaitForSeconds(emailComputerApplication.EmailLoadingTime);
        
        computerUI.UpdateCursor(ComputerCursorState.Default);

        EnableApplicationUIInteractivity();

        UpdateCurrentEmailUI();
    }

    private void UpdateCurrentEmailUI(){
        StopAllCoroutines();

        List<TextEffectDefinition> textEffects = new List<TextEffectDefinition>();
        
        UpdateText(recipientText, currentSelectedEmail.Recipient, textEffects);
        UpdateText(senderText, currentSelectedEmail.Sender, textEffects);
        UpdateText(subjectText, currentSelectedEmail.Subject, textEffects);

        TextContentProfile textContentProfile = TextParser.Parse(currentSelectedEmail.Body, out List<TextEffectDefinition> textEffectsBuffer);
        inputField.text = textContentProfile.textContent;
        scrollbar.value = 0;

        StartTextEffects(textEffectsBuffer, textContentProfile);

        if(currentSelectedEmail.saveNote){
            PlayerNoteHandler.Instance.AttemptAddNewNoteToDataList(currentSelectedEmail.noteToSave);
            PlayerNoteHandler.Instance.AttemptShowNotePopup();
        }
    }

    private void UpdateText(TextMeshProUGUI textToUpdate, string unParsedString, List<TextEffectDefinition> textEffectsBuffer){
        TextContentProfile textContentProfile = TextParser.Parse(unParsedString, out textEffectsBuffer);
        textToUpdate.text = textContentProfile.textContent;

        StartTextEffects(textEffectsBuffer, textContentProfile);
    }

    private void StartTextEffects(List<TextEffectDefinition> textEffects, TextContentProfile textContentProfile){
        if(textEffects.Count > 0){
            UIAnimator.StartTextAnimations(this, recipientText, textContentProfile);
        }

        textEffects.Clear();
    }

    public override void EnableApplicationUIInteractivity(){
        base.EnableApplicationUIInteractivity();
        inputField.interactable = false;
    }
}
