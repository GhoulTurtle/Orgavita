using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI windowNameText;
    [SerializeField] private Button closeButton;

    public ComputerApplication ApplicationSO {get; private set;}
    protected ComputerUI computerUI;

    public virtual void SetupApplicationUI(ComputerUI _computerUI, ComputerApplication application){
        computerUI = _computerUI;
        ApplicationSO = application;

        windowNameText.text = ApplicationSO.Name;
    }

    public virtual void CloseApplication(){
        //Make stuff non-interactable
        closeButton.interactable = false;
        computerUI.CloseApplication();
    }
}
