using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextButtonUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;

    private ContextMenuUI contextMenuUI;
    private ContextButton contextButton;

    public void SetupContextButtonUI(ContextMenuUI _contextMenuUI, ContextButton _contextButton){
        contextMenuUI = _contextMenuUI;
        contextButton = _contextButton;

        buttonText.text = GetButtonText();
        SetupButtonEvent();
    }

    public void SetupButtonEvent(){
        button.onClick.AddListener(contextButton.GetButtonAction());
    }


    public string GetButtonText(){
        return contextButton.GetContextButtonType() switch{
            ContextButtonType.Use => "Use",
            ContextButtonType.Equip => "Equip",
            ContextButtonType.UnEquip => "Unequip",
            ContextButtonType.Inspect => "Inspect",
            ContextButtonType.Combine => "Combine",
            ContextButtonType.Destroy => "Destroy",
            _ => "",
        };
    }
}
