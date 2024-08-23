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

        computerUI.OnEnableUI += (sender, e) => EnableApplicationUIInteractivity();
        computerUI.OnDisableUI += (sender, e) => DisableApplicationUIInteractivity();

        EnableApplicationUIInteractivity();
    }

    public virtual void CloseApplication(){
        DisableApplicationUIInteractivity();

        computerUI.OnEnableUI -= (sender, e) => EnableApplicationUIInteractivity();
        computerUI.OnDisableUI -= (sender, e) => DisableApplicationUIInteractivity();
        
        computerUI.CloseApplication();
    }

    protected virtual void EnableApplicationUIInteractivity(){
        Selectable[] selectableUI = transform.GetComponentsInChildren<Selectable>(true);
        for (int i = 0; i < selectableUI.Length; i++){
            selectableUI[i].interactable = true;
        }
    }

    protected virtual void DisableApplicationUIInteractivity(){
        Selectable[] selectableUI = transform.GetComponentsInChildren<Selectable>(true);
        for (int i = 0; i < selectableUI.Length; i++){
            selectableUI[i].interactable = false;
        }
    }
}
