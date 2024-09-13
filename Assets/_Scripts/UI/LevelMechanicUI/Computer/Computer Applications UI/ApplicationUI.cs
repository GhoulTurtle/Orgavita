using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] protected TextMeshProUGUI windowNameText;
    [SerializeField] protected Button closeButton;

    public ComputerApplication ApplicationSO {get; protected set;}
    protected ComputerUI computerUI;

    public virtual void SetupApplicationUI(ComputerUI _computerUI, ComputerApplication application){
        computerUI = _computerUI;
        ApplicationSO = application;

        windowNameText.text = ApplicationSO.Name;

        computerUI.OnEnableUI += EnableApplicationUIInteractivity;
        computerUI.OnDisableUI += DisableApplicationUIInteractivity;

        EnableApplicationUIInteractivity();
    }

    public virtual void CloseApplication(){
        DisableApplicationUIInteractivity();

        computerUI.OnEnableUI -= EnableApplicationUIInteractivity;
        computerUI.OnDisableUI -= DisableApplicationUIInteractivity;
        
        computerUI.CloseApplication();
    }

    public virtual void EnableApplicationUIInteractivity(){
        Selectable[] selectableUI = transform.GetComponentsInChildren<Selectable>(true);
        for (int i = 0; i < selectableUI.Length; i++){
            selectableUI[i].interactable = true;
        }
    }

    public virtual void DisableApplicationUIInteractivity(){
        Selectable[] selectableUI = transform.GetComponentsInChildren<Selectable>(true);
        for (int i = 0; i < selectableUI.Length; i++){
            selectableUI[i].interactable = false;
        }
    }
}
