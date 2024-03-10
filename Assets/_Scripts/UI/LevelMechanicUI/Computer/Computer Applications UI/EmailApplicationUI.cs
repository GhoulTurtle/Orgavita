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
    [SerializeField] private TextMeshProUGUI subjectText;
    [SerializeField] private TextMeshProUGUI bodyText;

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

        UpdateCurrentEmailUI();
    }

    private void UpdateCurrentEmailUI(){
        recipientText.text = currentSelectedEmail.Recipient;
        senderText.text = currentSelectedEmail.Sender;
        subjectText.text = currentSelectedEmail.Subject;
        bodyText.text = currentSelectedEmail.Body;
    }
}
