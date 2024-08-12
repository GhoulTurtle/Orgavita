using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSwapUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Button slotButton;
    [SerializeField] private Image slotItemImage;
    [SerializeField] private TextMeshProUGUI slotItemNameText;

    private Sprite emptySprite;

    private QuickSwapMenuUI quickSwapMenuUI;

    public void SetupQuickSwapUI(QuickSwapMenuUI _quickSwapMenuUI, Sprite _emptySprite){
        quickSwapMenuUI = _quickSwapMenuUI;
        emptySprite = _emptySprite;
    }

    public void EnableUIInteractivity(){
        slotButton.interactable = true;
    }

    public void DisableUIInteractivity(){
        slotButton.interactable = false;
    }

    public void AssignSlot(){
        quickSwapMenuUI.SelectedSlot(this);
    }

    public void UpdateSlotUI(Sprite itemSprite, string itemName){
        slotItemImage.sprite = itemSprite;
        slotItemNameText.text = itemName;
    }

    public Button GetQuickSwapButton(){
        return slotButton;
    }   

    public void ClearSlotUI(){
        slotItemImage.sprite = emptySprite;
        slotItemNameText.text = "";
    }
}