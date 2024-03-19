using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, ISelectHandler{
    [Header("UI References")]
    [SerializeField] private Button itemUIButton;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemStackAmountText;
    [SerializeField] private Image itemImage;

    [Header("Required References")]
    [SerializeField] private Sprite emptyItemSlotSprite;

    [Header("Item UI Variables")]
    [SerializeField] private Color regularStackAmountTextColor = Color.white;
    [SerializeField] private Color maxStackAmountTextColor = Color.red;

    private InventoryUI inventoryUI;

    private InventoryItem associatedInventoryItem;

    public void SetupItemUI(InventoryUI _inventoryUI, InventoryItem _associatedInventoryItem){
        inventoryUI = _inventoryUI;
    
        associatedInventoryItem = _associatedInventoryItem;

        associatedInventoryItem.OnItemCleared += (sender, e) => EmptyItemUI();
        associatedInventoryItem.OnItemUpdated += (sender, e) => UpdateItemUI(associatedInventoryItem.GetHeldItem());

        UpdateItemUI(associatedInventoryItem.GetHeldItem());
    }

    private void OnDestroy() {
        if(associatedInventoryItem == null) return;

        associatedInventoryItem.OnItemCleared -= (sender, e) => EmptyItemUI();
        associatedInventoryItem.OnItemUpdated -= (sender, e) => UpdateItemUI(associatedInventoryItem.GetHeldItem());
    }

    private void UpdateItemUI(ItemDataSO itemData){
        if(itemData == null){
            EmptyItemUI();
            return;
        }
        
        itemNameText.text = itemData.GetItemName();
        itemStackAmountText.text = associatedInventoryItem.GetCurrentStack().ToString();
        
        itemImage.sprite = itemData.GetItemSprite();

        Color stackTextColor = associatedInventoryItem.IsFull() ? maxStackAmountTextColor : regularStackAmountTextColor;

        itemStackAmountText.color = stackTextColor;
    }

    private void EmptyItemUI(){
        itemNameText.text = "";
        itemStackAmountText.text = "";

        itemImage.sprite = emptyItemSlotSprite;
    }

    public void OnSelect(BaseEventData eventData){
        inventoryUI.SelectItemUI(this);
    }

    public void OnClick(){
        inventoryUI.ClickedSelectedItemUI();
    }

    public InventoryItem GetInventoryItem(){
        return associatedInventoryItem;
    }
}