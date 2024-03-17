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

    public EventHandler OnSlotSelected;

    private void Awake() {
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

        playerInventoryHandler.GetInventory().OnMaxInventoryIncreased -= (sender, e) => AddInventorySlotsUI(e.newSlotsAdded);
    }

    private void SetupInventoryUI(object sender, PlayerInventoryHandler.SetupInventoryEventArgs e){
        equippedItemUI.SetupItemUI(this, e.playerInventorySO.GetEquippedInventoryItem());
        emergencyItemUI.SetupItemUI(this, e.playerInventorySO.GetEmergencyInventoryItem());

        playerInventoryHandler.GetInventory().OnMaxInventoryIncreased += (sender, e) => AddInventorySlotsUI(e.newSlotsAdded);

        List<InventoryItem> inventory = e.playerInventorySO.GetCurrentInventory();
        
        SetupInventorySlotsUI(inventory);
        SetupInventoryItemsUI(inventory);
    }

    private void SetupInventoryItemsUI(List<InventoryItem> inventory){
        foreach (InventoryItem item in inventory){
            Instantiate(itemUITemplate, itemScrollContent).SetupItemUI(this, item);
        }
    }

    private void SetupInventorySlotsUI(List<InventoryItem> inventory){
        foreach (InventoryItem item in inventory){
            Instantiate(itemSlotTemplate, itemSlotParent).SetupItemSlotUI(this, item);
        }
    }

    private void AddInventorySlotsUI(List<InventoryItem> newInventoryItemsAdded){
        foreach (InventoryItem item in newInventoryItemsAdded){
            Instantiate(itemUITemplate, itemScrollContent).SetupItemUI(this, item);
            Instantiate(itemSlotTemplate, itemSlotParent).SetupItemSlotUI(this, item);
        }
    }

    private void ShowInventoryScreen(){
        inventoryScreen.gameObject.SetActive(true);
    }

    private void HideInventoryScreen(){
        inventoryScreen.gameObject.SetActive(false);
    }
}