using TMPro;
using UnityEngine;

public class ApplicationUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI windowNameText;

    public ComputerApplication ApplicationSO {get; private set;}
    private ComputerUI computerUI;

    public virtual void SetupApplicationUI(ComputerUI _computerUI, ComputerApplication application){
        computerUI = _computerUI;
        ApplicationSO = application;

        windowNameText.text = ApplicationSO.Name;
    }

    public virtual void CloseApplication(){
        //Make stuff non-interactable
        computerUI.CloseApplication();
    }
}
