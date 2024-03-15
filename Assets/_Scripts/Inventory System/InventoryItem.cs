using System;

/// <summary>
/// Responsible for handling item data and the stack amount of the current held item.
/// </summary>
[Serializable]
public class InventoryItem{
    public ItemDataSO HeldItemData;
    public int CurrentItemStackAmount;

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
        HeldItemData = itemData;
        CurrentItemStackAmount = itemStackAmount;
    }

    public int AddToStack(int amount){
        if(!IsStackable() || IsFull()) return amount;

        var remainingStackSpace = HeldItemData.maxStackSize - CurrentItemStackAmount;

        int addAmount = amount >= remainingStackSpace ? remainingStackSpace : amount;
        int returnAmount = amount >= remainingStackSpace ? amount -= remainingStackSpace : 0;

        CurrentItemStackAmount += addAmount;

        OnItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(HeldItemData, CurrentItemStackAmount));

        return returnAmount;
    }

    public void RemoveFromStack(int amount){
        if(IsEmpty() || !IsStackable()) return;

        CurrentItemStackAmount -= amount;
        
        if(CurrentItemStackAmount <= 0){
            ClearItem();
            return;
        }

        OnItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(HeldItemData, CurrentItemStackAmount));
    }

    public int SetItem(ItemDataSO itemData, int itemStackAmount = 1){
        HeldItemData = itemData;
        
        var stackAddAmount = itemStackAmount >= itemData.maxStackSize ? itemData.maxStackSize : itemStackAmount;
        var returnAmount = itemStackAmount > itemData.maxStackSize ? itemStackAmount - itemData.maxStackSize : 0;

        CurrentItemStackAmount = stackAddAmount;

        OnItemUpdated?.Invoke(this, new ItemUpdatedEventArgs(HeldItemData, CurrentItemStackAmount));

        return returnAmount;
    }

    public void ClearItem(){
        HeldItemData = null;
        CurrentItemStackAmount = 0;

        OnItemCleared?.Invoke(this, EventArgs.Empty);
    }

    public int ReturnItemMaxStack(){
        if(IsEmpty()) return 0;
        return HeldItemData.maxStackSize;
    }

    public bool IsEmpty(){
        return HeldItemData == null;
    }

    public bool IsFull(){
        return HeldItemData.maxStackSize == CurrentItemStackAmount;
    }

    public bool IsStackable(){
        return HeldItemData.IsStackable;
    }
}