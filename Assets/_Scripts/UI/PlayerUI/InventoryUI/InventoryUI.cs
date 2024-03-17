using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Transform inventoryScreen;
    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private ItemSlotUI itemSlotTemplate;
    [SerializeField] private Transform itemScrollContent;
    [SerializeField] private ItemUI itemUITemplate;
    [SerializeField] private ItemUI equippedItemUI;
    [SerializeField] private ItemUI emergencyItemUI;

    [Header("Required References")]
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private MenuSelector inventoryUISelector;

    public EventHandler<SlotSelectedEventArgs> OnSlotSelected;
    public class SlotSelectedEventArgs : EventArgs{
        public InventoryItem inventoryItemSelected;
        public ItemDataSO itemDataSelected;

        public SlotSelectedEventArgs(InventoryItem _inventoryItemSelected, ItemDataSO _itemDataSelected){
            itemDataSelected = _itemDataSelected;
            inventoryItemSelected = _inventoryItemSelected;
        }
    }

    private ItemUI currentSelectedItemUI;

    private List<ItemUI> currentItemUI;
    private List<ItemSlotUI> currentItemSlotUI;

    private void Awake() {
        currentItemUI = new List<ItemUI>();
        currentItemSlotUI = new List<ItemSlotUI>();

        HideInventoryScreen();

        if(playerInventoryHandler == null) return;

        playerInventoryHandler.OnShowInventory += (sender, e) => ShowInventoryScreen();
        playerInventoryHandler.OnHideInventory += (sender, e) => HideInventoryScreen();
        
        playerInventoryHandler.OnSetupInventory += SetupInventoryUI;
    }

    private void OnDestroy() {
        if(playerInventoryHandler == null) return;

        playerInventoryHandler.OnShowInventory -= (sender, e) => ShowInventoryScreen();
        playerInventoryHandler.OnHideInventory -= (sender, e) => HideInventoryScreen();

        playerInventoryHandler.OnSetupInventory -= SetupInventoryUI;

        playerInventoryHandler.GetInventory().OnMaxInventoryIncreased -= (sender, e) => AddNewInventoryUI(e.newSlotsAdded);
    }

    public void SelectItemUI(ItemUI itemUI){
        currentSelectedItemUI = itemUI;
        var itemUIInventoryItem = itemUI.GetInventoryItem();
        OnSlotSelected?.Invoke(this, new SlotSelectedEventArgs(itemUIInventoryItem, itemUIInventoryItem.GetHeldItem()));
    }

    private void SetupInventoryUI(object sender, PlayerInventoryHandler.SetupInventoryEventArgs e){
        equippedItemUI.SetupItemUI(this, e.playerInventorySO.GetEquippedInventoryItem());
        emergencyItemUI.SetupItemUI(this, e.playerInventorySO.GetEmergencyInventoryItem());

        playerInventoryHandler.GetInventory().OnMaxInventoryIncreased += (sender, e) => AddNewInventoryUI(e.newSlotsAdded);

        List<InventoryItem> inventory = e.playerInventorySO.GetCurrentInventory();
        
        AddNewInventoryUI(inventory);
        SetupInventorySelector();
    }

    private void SetupInventorySelector(){
        inventoryUISelector.SetCursorStartingSelection(currentItemUI[0].transform);
    }

    private void AddNewInventoryUI(List<InventoryItem> newInventoryItemsAdded){
        foreach (InventoryItem item in newInventoryItemsAdded){
            var newItemUI = Instantiate(itemUITemplate, itemScrollContent);
            newItemUI.SetupItemUI(this, item);
            currentItemUI.Add(newItemUI);

            var newItemSlotUI = Instantiate(itemSlotTemplate, itemSlotParent);
            newItemSlotUI.SetupItemSlotUI(this, item);
            currentItemSlotUI.Add(newItemSlotUI);
        }
    }

    private void ShowInventoryScreen(){        
        inventoryScreen.gameObject.SetActive(true);

        inventoryUISelector.EnableSelector();
        if(currentSelectedItemUI != null){
            var itemUIInventoryItem = currentSelectedItemUI.GetInventoryItem();
            OnSlotSelected?.Invoke(this, new SlotSelectedEventArgs(itemUIInventoryItem, itemUIInventoryItem.GetHeldItem()));
        }
    }

    private void HideInventoryScreen(){
        inventoryUISelector.DisableSelector();
        inventoryScreen.gameObject.SetActive(false);
    }
}