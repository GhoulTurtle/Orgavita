using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DesktopUI : MonoBehaviour{
    public ComputerApplication ApplicationSO {get; private set;}

    [Header("UI References")]
    [SerializeField] private Image applicationIconImage;
    [SerializeField] private TextMeshProUGUI applicationNameText;

    private ComputerUI computerUI;

    public void SetupDesktopUI(ComputerUI _computerUI, ComputerApplication application){
        computerUI = _computerUI;
        
        ApplicationSO = application;

        applicationIconImage.sprite = application.Icon;
        applicationNameText.text = application.Name;
    }

    public void SelectApplication(){
        //Check if this is the current selected desktop
        if(!computerUI.SelectDesktopApplication(this)){
            applicationIconImage.color = computerUI.SelectedIconTint;
        }
    }

    public void DeselectApplication(){
        applicationIconImage.color = Color.white;
    }
}