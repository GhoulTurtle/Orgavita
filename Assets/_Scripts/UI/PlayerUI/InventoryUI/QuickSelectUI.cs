using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSelectUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Button slotButton;
    [SerializeField] private Image slotItemImage;
    [SerializeField] private TextMeshProUGUI slotItemNameText;

    private Sprite emptySprite;

    private QuickSelectMenuUI quickSelectMenuUI;
    private QuickSelectDataSO quickSelectDataSO; 
    private int slotIndex;

    public void SetupQuickSelectUI(QuickSelectMenuUI _quickSwapMenuUI, QuickSelectDataSO _quickSelectDataSO, int _slotIndex, Sprite _emptySprite){
        quickSelectMenuUI = _quickSwapMenuUI;
        quickSelectDataSO = _quickSelectDataSO;
        slotIndex = _slotIndex;
        emptySprite = _emptySprite;

        if(quickSelectDataSO != null){
            quickSelectDataSO.OnQuickSelectUpdated += QuickSelectUIUpdated;
            QuickSelectUIUpdated();
        }
    }

    private void OnDestroy() {
        if(quickSelectDataSO != null){
            quickSelectDataSO.OnQuickSelectUpdated -= QuickSelectUIUpdated;
        }
    }

    public void EnableUIInteractivity(){
        slotButton.interactable = true;
    }

    public void DisableUIInteractivity(){
        slotButton.interactable = false;
    }

    public void AssignSlot(){
        quickSelectMenuUI.SelectedSlot(this);
    }

    private void QuickSelectUIUpdated(){
        ItemDataSO slotItemData = quickSelectDataSO.GetItemDataSO(slotIndex);

        if(slotItemData == null){
            ClearSlotUI();
            return;
        }

        UpdateSlotUI(slotItemData.GetItemSprite(), slotItemData.GetItemName());
    }    

    private void UpdateSlotUI(Sprite itemSprite, string itemName){
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