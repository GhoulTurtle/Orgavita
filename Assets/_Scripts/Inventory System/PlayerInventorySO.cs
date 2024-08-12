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

    public EventHandler<EquippedItemEventArgs> OnWeaponItemEquipped;
    public EventHandler OnWeaponItemUnequipped;
    public EventHandler<EquippedItemEventArgs> OnEmergencyItemEquipped;
    public EventHandler OnEmergencyItemUnequipped;
    public class EquippedItemEventArgs : EventArgs{
        public InventoryItem inventoryItem;
        public EquippedItemBehaviour equippedItemBehaviour;

        public EquippedItemEventArgs(InventoryItem _inventoryItem, EquippedItemBehaviour _equippedItemBehaviour){
            inventoryItem = _inventoryItem;
            equippedItemBehaviour = _equippedItemBehaviour;
        }
    }

    public EventHandler<OpenCombinationMenuEventArgs> OnOpenCombinationMenu;
    public class OpenCombinationMenuEventArgs : EventArgs{
        public InventoryItem firstItem;
        public InventoryItem secondItem;
        public InventoryItem resultItem;
        public PlayerInventoryRecipeListSO playerInventoryRecipeListSO;
        
        public OpenCombinationMenuEventArgs(InventoryItem _firstItem, InventoryItem _secondItem, InventoryItem _resultItem, PlayerInventoryRecipeListSO _playerInventoryRecipeListSO){
            firstItem = _firstItem;
            secondItem = _secondItem;
            resultItem = _resultItem;
            playerInventoryRecipeListSO = _playerInventoryRecipeListSO;
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

    public void EquipWeaponItem(InventoryItem itemToEquip){
        ItemDataSO itemDataSO = itemToEquip.GetHeldItem();

        if(!equippedItem.IsEmpty()){
            OnWeaponItemUnequipped?.Invoke(this, EventArgs.Empty);
            SwapInventoryItems(equippedItem, itemToEquip);
        }
        else{
            equippedItem.SetItem(itemToEquip.GetHeldItem(), itemToEquip.GetCurrentStack());
            itemToEquip.ClearItem();
        }

        if(itemDataSO is WeaponItemDataSO weaponItemDataSO){
            if(weaponItemDataSO.GetEquippedItemBehaviour() != null){
                OnWeaponItemEquipped?.Invoke(this, new EquippedItemEventArgs(itemToEquip, weaponItemDataSO.GetEquippedItemBehaviour()));
            }   
        }
    }

    public void UnEquipWeaponItem(){
        if(equippedItem.IsEmpty()) return;
        
        List<InventoryItem> emptyInventorySpaces = FindEmptyInventoryItems();
        
        if(emptyInventorySpaces.Count == 0) return;

        AttemptToAddItemToInventory(equippedItem.GetHeldItem(), equippedItem.GetCurrentStack());
        equippedItem.ClearItem();
        OnWeaponItemUnequipped?.Invoke(this, EventArgs.Empty);
    }

    public void EquipEmergencyItem(InventoryItem itemToEquip){
        ItemDataSO itemDataSO = itemToEquip.GetHeldItem();

        if(!emergencyItem.IsEmpty()){
            OnEmergencyItemUnequipped?.Invoke(this, EventArgs.Empty);
            SwapInventoryItems(emergencyItem, itemToEquip);
        }
        else{
            emergencyItem.SetItem(itemToEquip.GetHeldItem(), itemToEquip.GetCurrentStack());
            itemToEquip.ClearItem();
        }
        
        if(itemDataSO is ToolItemDataSO emergencyItemDataSO){
            if(emergencyItemDataSO.GetEquippedItemBehaviour() != null){
                OnEmergencyItemEquipped?.Invoke(this, new EquippedItemEventArgs(itemToEquip, emergencyItemDataSO.GetEquippedItemBehaviour()));
            }   
        }
    }

    public void UnEquipEmergencyItem(){
        if(emergencyItem.IsEmpty()) return;
        
        List<InventoryItem> emptyInventorySpaces = FindEmptyInventoryItems();
        
        if(emptyInventorySpaces.Count == 0) return;

        AttemptToAddItemToInventory(emergencyItem.GetHeldItem(), emergencyItem.GetCurrentStack());
        emergencyItem.ClearItem();
        OnEmergencyItemUnequipped?.Invoke(this, EventArgs.Empty);
    }

    public void AttemptReloadEquippedItem(ResourceDataSO equippedItemResourceData){
        ItemDataSO validResourceItemData = equippedItemResourceData.GetValidItemData();


    }

    public void MoveItem(InventoryItem inventoryItemToMove, InventoryItem inventoryItemMoveLocation){
        if(!inventoryItemMoveLocation.IsEmpty()){
            SwapInventoryItems(inventoryItemToMove, inventoryItemMoveLocation);
        }
        else{
            inventoryItemMoveLocation.SetItem(inventoryItemToMove.GetHeldItem(), inventoryItemToMove.GetCurrentStack());
            inventoryItemToMove.ClearItem();
        }
    }

    public string AttemptItemCombination(InventoryItem initalComboItem, InventoryItem incomingItem, PlayerInventoryRecipeListSO playerInventoryRecipeListSO){
        ComboResult newComboResult = new ComboResult();

        ItemDataSO initalComboItemData = initalComboItem.GetHeldItem();
        ItemDataSO incomingItemData = incomingItem.GetHeldItem();

        if(initalComboItemData == incomingItemData && initalComboItem.IsFull() || 
           initalComboItemData == incomingItemData && incomingItem.IsFull()){
            return newComboResult.SetComboResult(ComboResultType.Invalid_Stack_Combo, null);
        }
    
        if(initalComboItemData == incomingItemData && !initalComboItem.IsFull() && !incomingItem.IsFull()){
            int remainingStack = AttemptToStackItems(incomingItem, initalComboItem);
            if(remainingStack != 0){
                initalComboItem.SetStack(remainingStack);
            }
            else{
                initalComboItem.ClearItem();
            }

            return newComboResult.SetComboResult(ComboResultType.Valid_Stack_Combo, incomingItem);
        }

        //if initalcomboitem is a resource and the incoming item is a weapon then return either INVALIDWEAPONRESOURCECOMBO, VALIDWEAPONRESOUCECOMBO, or FULLWEAPON
        if(initalComboItemData.GetItemType() == ItemType.Resource && incomingItemData.GetItemType() == ItemType.Weapon && incomingItemData is WeaponItemDataSO weaponItemDataSO){
            ResourceDataSO weaponResourceData = weaponItemDataSO.GetEquippedItemBehaviour().GetEquippedItemResourceData();

            return AttemptReloadResourceCombination(initalComboItem, incomingItem, weaponResourceData, newComboResult, true);
        }

        if(initalComboItemData.GetItemType() == ItemType.Weapon && incomingItemData.GetItemType() == ItemType.Resource && initalComboItemData is WeaponItemDataSO _weaponItemDataSO){
            ResourceDataSO weaponResourceData = _weaponItemDataSO.GetEquippedItemBehaviour().GetEquippedItemResourceData();
        
            return AttemptReloadResourceCombination(incomingItem, initalComboItem, weaponResourceData, newComboResult, true);
        }
        
        //if initalcomboitem is a resource and the incoming item is a tool then return either INVALIDTOOLRESOURCECOMBO, VALIDTOOLRESOUCECOMBO or FULLTOOL
        if(initalComboItemData.GetItemType() == ItemType.Resource && incomingItemData.GetItemType() == ItemType.Tool && incomingItemData is ToolItemDataSO emergencyItemDataSO){
            ResourceDataSO emergencyItemResourceData = emergencyItemDataSO.GetEquippedItemBehaviour().GetEquippedItemResourceData();
            return AttemptReloadResourceCombination(initalComboItem, incomingItem, emergencyItemResourceData, newComboResult, false);
        }

        if(initalComboItemData.GetItemType() == ItemType.Tool && incomingItemData.GetItemType() == ItemType.Resource && initalComboItemData is ToolItemDataSO _emergencyItemDataSO){
            ResourceDataSO emergencyItemResourceData = _emergencyItemDataSO.GetEquippedItemBehaviour().GetEquippedItemResourceData();
            return AttemptReloadResourceCombination(incomingItem, initalComboItem, emergencyItemResourceData, newComboResult, false);
        }

        InventoryItem attemptedItemCombo = playerInventoryRecipeListSO.ReturnValidInventoryRecipeResult(initalComboItemData, incomingItemData);
        if(attemptedItemCombo == null){
            return newComboResult.SetComboResult(ComboResultType.Invalid_Combo, null);
        }

        if(attemptedItemCombo != null)
        {
            //Check if both items are a resource or a consumable to use the comboination menu
            if ((initalComboItemData.GetItemType() == ItemType.Consumable || initalComboItemData.GetItemType() == ItemType.Resource) &&
                (incomingItemData.GetItemType() == ItemType.Consumable || incomingItemData.GetItemType() == ItemType.Resource))
            {
                //Check if more then 1 combination is possible
                if (playerInventoryRecipeListSO.GetMaxAmountOfCombinations(initalComboItem, incomingItem, attemptedItemCombo.GetHeldItem()) > 1)
                {
                    //Trigger the combination menu
                    OnOpenCombinationMenu?.Invoke(this, new OpenCombinationMenuEventArgs(initalComboItem, incomingItem, attemptedItemCombo, playerInventoryRecipeListSO));
                    return newComboResult.SetComboResult(ComboResultType.Opened_Combination_Menu, attemptedItemCombo);
                }
            }

            return AttemptCraftNewItem(initalComboItem, incomingItem, newComboResult, attemptedItemCombo);
        }

        return "INVALID ITEM COMBINATION";
    }

    public string AttemptCraftNewItem(InventoryItem initalComboItem, InventoryItem incomingItem, ComboResult newComboResult, InventoryItem attemptedItemCombo){
        ItemDataSO initalComboItemData = initalComboItem.GetHeldItem();
        ItemDataSO incomingItemData = incomingItem.GetHeldItem();
        
        initalComboItem.RemoveFromStack(1);
        incomingItem.RemoveFromStack(1);

        int remainingItemStack = AttemptToAddItemToInventory(attemptedItemCombo.GetHeldItem(), attemptedItemCombo.GetCurrentStack());

        //If the remaining item stack equals the craft amount then the inventory is full so add the crafting items back into the inventory
        if (remainingItemStack == attemptedItemCombo.GetCurrentStack()){
            //Add back the items into the inventory
            AttemptToAddItemToInventory(initalComboItemData, 1);
            AttemptToAddItemToInventory(incomingItemData, 1);

            return newComboResult.SetComboResult(ComboResultType.Full_Inventory, attemptedItemCombo);
        }

        return newComboResult.SetComboResult(ComboResultType.Valid_Combo, attemptedItemCombo);
    }

    private string AttemptReloadResourceCombination(InventoryItem resourceInventoryItem, InventoryItem equipmentInventoryItem, ResourceDataSO resourceData, ComboResult comboResult, bool isWeapon){
        ItemDataSO resourceItemData = resourceInventoryItem.GetHeldItem();
        ComboResultType comboResultType;

        if(resourceData == null){
            return comboResult.SetComboResult(ComboResultType.Invalid_Combo, null);
        }
        
        if(resourceData.GetValidItemData() != resourceItemData){
            comboResultType = isWeapon ? ComboResultType.Invalid_Weapon_Resource_Combo : ComboResultType.Invalid_Emergency_Item_Resource_Combo;

            return comboResult.SetComboResult(comboResultType, null);
        }

        if(resourceData.IsFull()){
            comboResultType = isWeapon ? ComboResultType.Full_Weapon : ComboResultType.Full_Emergency_Item;

            return  comboResult.SetComboResult(comboResultType, equipmentInventoryItem);
        }

        int missingResourceCount = resourceData.GetMissingStackCount();
        int currentStackCount = resourceInventoryItem.GetCurrentStack();

        if(missingResourceCount <= currentStackCount){
            resourceData.AddItemStack(missingResourceCount);
            resourceInventoryItem.RemoveFromStack(missingResourceCount);
        }
        else{
            resourceData.AddItemStack(currentStackCount);
            resourceInventoryItem.ClearItem();
        }
        
        comboResultType = isWeapon ? ComboResultType.Valid_Weapon_Resource_Combo : ComboResultType.Valid_Emergency_Item_Resource_Combo;

        return comboResult.SetComboResult(comboResultType, equipmentInventoryItem);
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

    public void SwapInventoryItems(InventoryItem item1, InventoryItem item2){
        if(item1.IsEmpty() && item2.IsEmpty()) return;

        var item1Data = item1.GetHeldItem();
        var item1Stack = item1.GetCurrentStack();

        item1.SetItem(item2.GetHeldItem(), item2.GetCurrentStack());
        item2.SetItem(item1Data, item1Stack);
    }

    public bool HasItemInInventory(ItemDataSO itemDataSO){
        return inventory.FirstOrDefault(inventoryItem => inventoryItem.GetHeldItem() == itemDataSO) != null;
    }

    public bool AttemptRemoveItemAmountFromInventory(ItemDataSO itemToRemove, int amountToRemove, out int amountRemoved){
        amountRemoved = 0;

        List<InventoryItem> validInventoryItems = AttemptGetItemFromInventory(itemToRemove);

        if(!validInventoryItems.Any()){
            return false;
        }

        for (int i = 0; i < validInventoryItems.Count; i++){
            int itemStackAmount = validInventoryItems[i].GetCurrentStack();

            if(itemStackAmount >= amountToRemove){
                validInventoryItems[i].RemoveFromStack(amountToRemove);
                amountRemoved += amountToRemove;
                return true;
            }
            else{
                validInventoryItems[i].ClearItem();
                amountToRemove -= itemStackAmount;
                amountRemoved += itemStackAmount;
            }
        }

        return true;
    } 

    public List<InventoryItem> AttemptGetItemFromInventory(ItemDataSO itemDataSO){
        List<InventoryItem> correspondingInventoryItemList = new List<InventoryItem>();

        correspondingInventoryItemList = inventory.Where(inventoryItem => inventoryItem.GetHeldItem() == itemDataSO).ToList();

        return correspondingInventoryItemList;
    }

    private List<InventoryItem> AddInventorySlots(int slotsToAdd){
        List<InventoryItem> newSlotsAdded = new List<InventoryItem>();
        for (int i = 0; i < slotsToAdd; i++){
            var newInventoryItem = new InventoryItem(null);
            inventory.Add(newInventoryItem);
            newSlotsAdded.Add(newInventoryItem);
        }

        return newSlotsAdded;
    }

    private List<InventoryItem> FindEmptyInventoryItems(){
        return inventory.Where(inventoryItem => inventoryItem.IsEmpty()).ToList();
    }

    private List<InventoryItem> CheckValidStackableInventoryItems(ItemDataSO item){
        return inventory.Where(inventoryItem => inventoryItem.GetHeldItem() == item && !inventoryItem.IsFull()).ToList();
    }
}