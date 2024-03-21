using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A scriptable object that manages player inventory data.
/// Manages a list of inventory items, current equipped item, current emergency item, and any unlocked tools.
/// Also holds the max inventory size, that can be incremented.
/// </summary>

[CreateAssetMenu(menuName = "Inventory/Player Inventory", fileName = "NewPlayerInventorySO")]
public class PlayerInventorySO : ScriptableObject{
    [Header("Inventory Settings")]
    [SerializeField] private int maxInventorySize = 6;

    [Header("Inventory Data")]
    [SerializeField] private List<InventoryItem> inventory;
    [SerializeField] private InventoryItem equippedItem;
    [SerializeField] private InventoryItem emergencyItem;
    [SerializeField] private List<InventoryItem> unlockedTools;

    #if UNITY_EDITOR
    [Header("Inventory Editor Variables")]
    [SerializeField] private bool cleanInventoryOnPlay = false;
    [SerializeField] private int defaultMaxInventory = 6;
    #endif

    public EventHandler<MaxInventoryIncreasedEventArgs> OnMaxInventoryIncreased;
    public class MaxInventoryIncreasedEventArgs : EventArgs{
        public int CurrentMaxInventorySize;
        public int AmountAdded;
        public List<InventoryItem> newSlotsAdded;

        public MaxInventoryIncreasedEventArgs(int currentMaxInventorySize, int amountAdded, List<InventoryItem> _newSlotsAdded){
            CurrentMaxInventorySize = currentMaxInventorySize;
            AmountAdded = amountAdded;
            newSlotsAdded =_newSlotsAdded;
        }
    }

    #if UNITY_EDITOR
    public void OnEnable(){
        if(cleanInventoryOnPlay){
            maxInventorySize = defaultMaxInventory;
            GenerateNewInventory();
        }
    }
    #endif

    [ContextMenu("Generate New Inventory")]
    public void GenerateNewInventory(){
        inventory.Clear();
        unlockedTools.Clear();

        for (int i = 0; i < maxInventorySize; i++){
            inventory.Add(new InventoryItem(null));
        }

        equippedItem = new InventoryItem(null);
        emergencyItem = new InventoryItem(null);
    }

    public void IncreaseMaxInventory(int amount){
        List<InventoryItem> newSlotsAdded = AddInventorySlots(amount);
        
        maxInventorySize += amount;
        OnMaxInventoryIncreased?.Invoke(this, new MaxInventoryIncreasedEventArgs(maxInventorySize, amount, newSlotsAdded));
    }

    public int AttemptToAddItemToInventory(ItemDataSO item, int itemAmount){
        if(item.GetIsStackable()){
            List<InventoryItem> validInventoryItems = CheckValidStackableInventoryItems(item);
            if(validInventoryItems.Count > 0){
                for (int i = 0; i < validInventoryItems.Count; i++){
                    itemAmount = validInventoryItems[i].AddToStack(itemAmount);
                    if(itemAmount == 0) return 0;
                }
            }
        }

        List<InventoryItem> emptyInventorySlots = FindEmptyInventoryItems();
        if(emptyInventorySlots.Count > 0){
            for (int i = 0; i < emptyInventorySlots.Count; i++){
                itemAmount = emptyInventorySlots[i].SetItem(item, itemAmount);
                if(itemAmount == 0) return 0;
            }
        }

        return itemAmount;
    }

    public int AttemptToStackItems(InventoryItem itemToStack, InventoryItem itemToAdd){
        if(itemToStack.GetHeldItem() == null) return 0;

        int itemAmount = itemToAdd.GetCurrentStack();

        return itemToStack.AddToStack(itemAmount);
    }

    public void EquipItem(InventoryItem itemToEquip){
        if(!equippedItem.IsEmpty()){
            SwapInventoryItems(equippedItem, itemToEquip);
            return;
        }
    
        equippedItem.SetItem(itemToEquip.GetHeldItem(), itemToEquip.GetCurrentStack());
        
        itemToEquip.ClearItem();
    }

    public void UnEquipItem(){
        if(equippedItem.IsEmpty()) return;
        
        List<InventoryItem> emptyInventorySpaces = FindEmptyInventoryItems();
        
        if(emptyInventorySpaces.Count == 0) return;

        AttemptToAddItemToInventory(equippedItem.GetHeldItem(), equippedItem.GetCurrentStack());
        equippedItem.ClearItem();
    }

    public void EquipEmergencyItem(InventoryItem itemToEquip){
        if(!emergencyItem.IsEmpty()){
            SwapInventoryItems(emergencyItem, itemToEquip);
            return;
        }
    
        emergencyItem.SetItem(itemToEquip.GetHeldItem(), itemToEquip.GetCurrentStack());
        
        itemToEquip.ClearItem();
    }

    public void UnEquipEmergencyItem(){
        if(emergencyItem.IsEmpty()) return;
        
        List<InventoryItem> emptyInventorySpaces = FindEmptyInventoryItems();
        
        if(emptyInventorySpaces.Count == 0) return;
        
        AttemptToAddItemToInventory(emergencyItem.GetHeldItem(), emergencyItem.GetCurrentStack());
        emergencyItem.ClearItem();
    }

    public void AddTool(ItemDataSO itemDataSO, int initalToolStack){
        foreach (InventoryItem tool in unlockedTools){
            if(tool.GetHeldItem() == itemDataSO){
                return;
            }
        }

        unlockedTools.Add(new InventoryItem(itemDataSO, initalToolStack));
    }

    public void RemoveTool(InventoryItem toolInventoryItem){
        if(!unlockedTools.Contains(toolInventoryItem)) return;

        var toolIndex = unlockedTools.IndexOf(toolInventoryItem);

        unlockedTools[toolIndex].ClearItem();

        unlockedTools.Remove(toolInventoryItem);
    }

    public InventoryItem GetEquippedInventoryItem(){
        return equippedItem;
    }

    public InventoryItem GetEmergencyInventoryItem(){
        return emergencyItem;
    }

    public List<InventoryItem> GetCurrentInventory(){
        return inventory;
    }

    public int GetMaxInventorySize(){
        return maxInventorySize;
    }

    private List<InventoryItem> AddInventorySlots(int slotsToAdd){
        List<InventoryItem> newSlotsAdded = new List<InventoryItem>();
        for (int i = 0; i <= slotsToAdd; i++){
            var newInventoryItem = new InventoryItem(null);
            inventory.Add(newInventoryItem);
            newSlotsAdded.Add(newInventoryItem);
        }

        return newSlotsAdded;
    }

    private void SwapInventoryItems(InventoryItem item1, InventoryItem item2){
        if(item1.IsEmpty() && item2.IsEmpty()) return;

        var item1Data = item1.GetHeldItem();
        var item1Stack = item1.GetCurrentStack();

        item1.SetItem(item2.GetHeldItem(), item2.GetCurrentStack());
        item2.SetItem(item1Data, item1Stack);
    }

    private List<InventoryItem> FindEmptyInventoryItems(){
        return inventory.Where(inventoryItem => inventoryItem.IsEmpty()).ToList();
    }

    private List<InventoryItem> CheckValidStackableInventoryItems(ItemDataSO item){
        return inventory.Where(inventoryItem => inventoryItem.GetHeldItem() == item && !inventoryItem.IsFull()).ToList();
    }
}