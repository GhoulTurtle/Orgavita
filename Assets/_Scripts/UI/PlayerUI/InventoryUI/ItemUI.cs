using System;
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
    private PlayerInventoryHandler playerInventoryHandler;
    private InventoryItem associatedInventoryItem;

    private ResourceDataSO associatedItemResourceData;

    private bool isEquipmentSlot = false;

    public void SetupItemUI(InventoryUI _inventoryUI, PlayerInventoryHandler _playerInventoryHandler, InventoryItem _associatedInventoryItem, bool _isEquipmentSlot = false){
        isEquipmentSlot = _isEquipmentSlot;

        inventoryUI = _inventoryUI;
        playerInventoryHandler = _playerInventoryHandler;
    
        associatedInventoryItem = _associatedInventoryItem;

        associatedInventoryItem.OnItemCleared += (sender, e) => EmptyItemUI();
        associatedInventoryItem.OnItemUpdated += (sender, e) => UpdateItemUI(associatedInventoryItem.GetHeldItem());

        UpdateItemUI(associatedInventoryItem.GetHeldItem());
    }

    private void OnDestroy() {
        if(associatedInventoryItem != null){
            associatedInventoryItem.OnItemCleared -= (sender, e) => EmptyItemUI();
            associatedInventoryItem.OnItemUpdated -= (sender, e) => UpdateItemUI(associatedInventoryItem.GetHeldItem());
        }

        if(associatedItemResourceData != null){
            associatedItemResourceData.OnResourceUpdated -= UpdateStackText;
        }

    }

    private void UpdateItemUI(ItemDataSO itemData){
        if(itemData == null){
            EmptyItemUI();
            return;
        }
        
        itemNameText.text = itemData.GetItemName();
        itemImage.sprite = itemData.GetItemSprite();

        if(itemData.GetIsStackable()){
            itemStackAmountText.text = associatedInventoryItem.GetCurrentStack().ToString();
            Color stackTextColor = associatedInventoryItem.IsFull() ? maxStackAmountTextColor : regularStackAmountTextColor;
            itemStackAmountText.color = stackTextColor;
        }
        else{
            SetNonStackableItemStackText(itemData);
        }
    }

    private void SetNonStackableItemStackText(ItemDataSO itemData){
        itemStackAmountText.text = "";

        switch (itemData.GetItemType()){
            case ItemType.Weapon:
                if(itemData is WeaponItemDataSO equippedItemBehaviour){
                    ResourceDataSO resourceDataSO = equippedItemBehaviour.GetEquippedItemBehaviour().GetEquippedItemResourceData();
                    if(resourceDataSO == null) return;

                    SetAssociatedItemResourceData(resourceDataSO);

                    int currentResourceStackCount = resourceDataSO.GetCurrentStackCount();
                    int maxResourceStackCount = resourceDataSO.GetMaxStackCount();

                    itemStackAmountText.text = currentResourceStackCount + "/" + maxResourceStackCount; 
                    Color stackTextColor = currentResourceStackCount == 0 ? maxStackAmountTextColor : regularStackAmountTextColor;
                    itemStackAmountText.color = stackTextColor;                 
                } 
                break;
            case ItemType.Emergency_Item: 
                if(itemData is EmergencyItemDataSO _equippedItemBehaviour){
                    ResourceDataSO resourceDataSO = _equippedItemBehaviour.GetEquippedItemBehaviour().GetEquippedItemResourceData();
                    if(resourceDataSO is FlashlightResourceDataSO flashlightResourceDataSO){
                        SetAssociatedItemResourceData(flashlightResourceDataSO);

                        int currentBatteryPercentage = flashlightResourceDataSO.GetCurrentBatteryPercentage();

                        itemStackAmountText.text = currentBatteryPercentage + "%";
                        Color stackTextColor = currentBatteryPercentage <= 20 ? maxStackAmountTextColor : regularStackAmountTextColor;
                        itemStackAmountText.color = stackTextColor;
                    }
                } 
                break;
        }
    }
    
    private bool SetAssociatedItemResourceData(ResourceDataSO resourceDataSO){
        if(associatedItemResourceData == resourceDataSO) return false;

        if(associatedItemResourceData != null){
            associatedItemResourceData.OnResourceUpdated -= UpdateStackText; 
            associatedItemResourceData = null;
        }

        if(resourceDataSO == null) return false;

        associatedItemResourceData = resourceDataSO;

        associatedItemResourceData.OnResourceUpdated += UpdateStackText;   
        return true;
    }

    private void UpdateStackText(int obj){
        SetNonStackableItemStackText(associatedInventoryItem.GetHeldItem());
    }

    private void EmptyItemUI(){
        itemNameText.text = "";
        itemStackAmountText.text = "";
        SetAssociatedItemResourceData(null);

        itemImage.sprite = emptyItemSlotSprite;
    }

    public void DisableInteractivity(){
        itemUIButton.interactable = false;
    }

    public void EnableInteractivity(){
        itemUIButton.interactable = true;
    }

    public void OnSelect(BaseEventData eventData){
        inventoryUI.SelectItemUI(this);
    }

    public void OnClick(){
        if(associatedInventoryItem.IsEmpty() && playerInventoryHandler.CurrentInventoryState != InventoryState.Move || playerInventoryHandler.CurrentInventoryState == InventoryState.Move && isEquipmentSlot) return;
        
        inventoryUI.ClickedSelectedItemUI(this);
    }

    public InventoryItem GetInventoryItem(){
        return associatedInventoryItem;
    }
    
    public bool GetIsEquipmentSlot(){
        return isEquipmentSlot;
    }
}