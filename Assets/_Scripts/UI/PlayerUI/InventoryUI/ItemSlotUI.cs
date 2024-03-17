using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Image slotImage;

    private InventoryUI inventoryUI;

    private InventoryItem associatedInventoryItem;

    [Header("Item Slot Variables")]
    [SerializeField] private Color itemSlotEmptyColor = Color.gray;
    [SerializeField] private Color itemSlotSelectedColor = Color.white;
    [SerializeField] private Color itemSlotFilledColor = Color.red;

    public void SetupItemSlotUI(InventoryUI _inventoryUI, InventoryItem _associatedInventoryItem){
        inventoryUI = _inventoryUI;

        associatedInventoryItem = _associatedInventoryItem;

        associatedInventoryItem.OnItemCleared += (sender, e) => EmptySlotColor();
        associatedInventoryItem.OnItemUpdated += (sender, e) => UpdateSlotColor();
        inventoryUI.OnSlotSelected += NewSlotSelected;
        UpdateSlotColor();
    }

    private void NewSlotSelected(object sender, InventoryUI.SlotSelectedEventArgs e){
        if(e.inventoryItemSelected == associatedInventoryItem){
            SelectedSlotColor();
            return;
        }

        UpdateSlotColor();
    }

    private void OnDestroy() {
        if(associatedInventoryItem == null) return;
        associatedInventoryItem.OnItemCleared -= (sender, e) => EmptySlotColor();
        associatedInventoryItem.OnItemUpdated -= (sender, e) => UpdateSlotColor();
    }

    private void SelectedSlotColor(){
        slotImage.color = itemSlotSelectedColor;
    }

    private void EmptySlotColor(){
        slotImage.color = itemSlotEmptyColor;
    }

    private void UpdateSlotColor(){
        var slotColor = associatedInventoryItem.IsEmpty() ? itemSlotEmptyColor : itemSlotFilledColor;

        slotImage.color = slotColor; 
    }
}
