using System;
using UnityEngine;

/// <summary>
/// Responsible for handling item data and the stack amount of the current held item.
/// </summary>
[Serializable]
public class InventoryItem{
    [SerializeField] private ItemDataSO heldItemData;
    [SerializeField] private int currentItemStackAmount;

    //Events
    public EventHandler<ItemUpdatedEventArgs> OnItemUpdated;
    public EventHandler OnItemCleared;

    public class ItemUpdatedEventArgs : EventArgs{
        public ItemDataSO itemData;
        public int stackAmount;

        public ItemUpdatedEventArgs(ItemDataSO _itemData, int _stackAmount){
            itemData = _itemData;
            stackAmount = _stackAmount;
        }
    }

    public InventoryItem(ItemDataSO itemData, int itemStackAmount = 0){
        heldItemData = itemData;
        currentItemStackAmount = itemStackAmount;
    }

    public int AddToStack(int amount){
        if(!IsStackable() || IsFull()) return amount;

        var remainingStackSpace = heldItemData.GetItemMaxStackSize() - currentItemStackAmount;

        int addAmount = amount >= remainingStackSpace ? remainingStackSpace : amount;
        int returnAmount = amount >= remainingStackSpace ? amount -= remainingStackSpace : 0;

        currentItemStackAmount += addAmount;

        OnItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(heldItemData, currentItemStackAmount));

        return returnAmount;
    }

    public void RemoveFromStack(int amount){
        if(IsEmpty()) return;

        if(!IsStackable()) ClearItem();

        currentItemStackAmount -= amount;
        
        if(currentItemStackAmount <= 0){
            ClearItem();
            return;
        }

        OnItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(heldItemData, currentItemStackAmount));
    }

    public void SetStack(int stackAmount){
         if(IsEmpty() || !IsStackable()) return;

         currentItemStackAmount = stackAmount;

         if(currentItemStackAmount <= 0){
            ClearItem();
            return;
        }

        OnItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(heldItemData, currentItemStackAmount));
    }

    public int SetItem(ItemDataSO itemData, int itemStackAmount = 1){
        if(itemData == null){
            ClearItem();
            return 0;
        }
        
        heldItemData = itemData;
        
        var stackAddAmount = itemStackAmount >= itemData.GetItemMaxStackSize() ? itemData.GetItemMaxStackSize() : itemStackAmount;
        var returnAmount = itemStackAmount > itemData.GetItemMaxStackSize() ? itemStackAmount - itemData.GetItemMaxStackSize() : 0;

        currentItemStackAmount = stackAddAmount;

        OnItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(heldItemData, currentItemStackAmount));

        return returnAmount;
    }

    public void ClearItem(){
        heldItemData = null;
        currentItemStackAmount = 0;

        OnItemCleared?.Invoke(this, EventArgs.Empty);
    }

    public ItemDataSO GetHeldItem(){
        return heldItemData;
    }

    public int GetCurrentStack(){
        return currentItemStackAmount;
    }

    public int ReturnItemMaxStack(){
        if(IsEmpty()) return 0;
        return heldItemData.GetItemMaxStackSize();
    }

    public bool IsEmpty(){
        return heldItemData == null;
    }

    public bool IsFull(){
        return heldItemData.GetItemMaxStackSize() == currentItemStackAmount;
    }

    public bool IsStackable(){
        return heldItemData.GetIsStackable();
    }
}