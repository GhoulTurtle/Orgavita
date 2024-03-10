using TMPro;
using UnityEngine;

public class EmailUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TextMeshProUGUI subjectText;

    private Email email;
    private EmailApplicationUI emailApplicationUI;

    public void SetupEmailUI(EmailApplicationUI _emailApplicationUI, Email _email){
        senderText.text = _email.Sender;
        subjectText.text = _email.Subject;

        email = _email;
        emailApplicationUI = _emailApplicationUI;
    }

    public void ShowEmail(){
        emailApplicationUI.SelectEmail(email);
    }
}
