using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A scriptable object that manages player inventory data.
/// Manages a list of inventory items, current equipped item, current emergency item, and any unlocked tools.
/// Also holds the max inventory size, that can be incremented.
/// </summary>

[CreateAssetMenu(menuName = "Inventory/Player Inventory", fileName = "NewPlayerInventorySO")]
public class PlayerInventorySO : ScriptableObject{
    [Header("Inventory Settings")]
    public int MaxInventorySize = 6;

    [Header("Inventory Data")]
    public List<InventoryItem> Inventory;
    public InventoryItem EquippedItem;
    public InventoryItem EmergencyItem;
    public List<InventoryItem> UnlockedTools;

    #if UNITY_EDITOR
    [Header("Inventory Editor Variables")]
    [SerializeField] private bool cleanInventoryOnPlay = false;
    [SerializeField] private int defaultMaxInventory = 6;
    #endif

    public EventHandler<MaxInventoryIncreasedEventArgs> OnMaxInventoryIncreased;
    public class MaxInventoryIncreasedEventArgs : EventArgs{
        public int CurrentMaxInventorySize;
        public int AmountAdded;

        public MaxInventoryIncreasedEventArgs(int currentMaxInventorySize, int amountAdded){
            CurrentMaxInventorySize = currentMaxInventorySize;
            AmountAdded = amountAdded;
        }
    }

    #if UNITY_EDITOR
    public void OnEnable(){
        if(cleanInventoryOnPlay){
            MaxInventorySize = defaultMaxInventory;
            GenerateNewInventory();
        }
    }
    #endif

    [ContextMenu("Generate New Inventory")]
    public void GenerateNewInventory(){
        Inventory.Clear();
        UnlockedTools.Clear();

        for (int i = 0; i < MaxInventorySize; i++){
            Inventory.Add(new InventoryItem(null));
        }

        EquippedItem = new InventoryItem(null);
        EmergencyItem = new InventoryItem(null);
    }

    public void IncreaseMaxInventory(int amount){
        MaxInventorySize += amount;
        OnMaxInventoryIncreased?.Invoke(this, new MaxInventoryIncreasedEventArgs(MaxInventorySize, amount));
    }

    public int AttemptToAddItemToInventory(ItemDataSO item, int itemAmount){
        if(item.IsStackable){
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

    public List<InventoryItem> FindEmptyInventoryItems(){
        return Inventory.Where(inventoryItem => inventoryItem.IsEmpty()).ToList();
    }

    public List<InventoryItem> CheckValidStackableInventoryItems(ItemDataSO item){
        return Inventory.Where(inventoryItem => inventoryItem.HeldItemData == item && !inventoryItem.IsFull()).ToList();
    }
}