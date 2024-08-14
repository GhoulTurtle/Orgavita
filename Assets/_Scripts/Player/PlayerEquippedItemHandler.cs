using System;
using UnityEngine;

//Handles the spawning/despawning, setup of equipped items, and handling the active item
public class PlayerEquippedItemHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Transform activeItemTransform;

    [Header("Required Holster References")]
    [SerializeField] private Transform sideArmHolsterTransform;
    [SerializeField] private Transform backHolsterTransform;
    [SerializeField] private Transform inactiveEmergencyItemHolsterTransform;
    [SerializeField] private Transform flashlightHolsterTransform;

    private EquippedItemBehaviour currentToolItemBehaviour;
    private EquippedItemBehaviour currentWeaponItemBehaviour;

    private EquippedItemBehaviour previousActiveItem;

    private PlayerInventorySO playerInventorySO;

    public EventHandler<ItemBehaviourSpawnedEventArgs> OnEmergencyItemBehaviourSpawned;
    public EventHandler<ItemBehaviourSpawnedEventArgs> OnEmergencyItemBehaviourDespawned;
    
    public EventHandler<ItemBehaviourSpawnedEventArgs> OnWeaponItemBehaviourSpawned;
    public EventHandler<ItemBehaviourSpawnedEventArgs> OnWeaponItemBehaviourDespawned;

    public class ItemBehaviourSpawnedEventArgs : EventArgs{
        public EquippedItemBehaviour equippedItemBehaviour;
        public ItemBehaviourSpawnedEventArgs(EquippedItemBehaviour _equippedItemBehaviour){
            equippedItemBehaviour = _equippedItemBehaviour;
        } 
    }

    private InventoryItem weaponInventoryItemToEquip;
    private InventoryItem toolInventoryItemToEquip;

    private void Awake() {
        playerInventorySO = playerInventoryHandler.GetInventory();
        
        playerInventorySO.OnToolItemEquipped += ToolItemEquipped;
        playerInventorySO.OnToolItemUnequipped += ToolItemUnequipped;

        playerInventorySO.OnWeaponItemEquipped += WeaponItemEquipped;
        playerInventorySO.OnWeaponItemUnequipped += WeaponItemUnequipped;
    }

    private void OnDestroy() {
        playerInventorySO.OnToolItemEquipped -= ToolItemEquipped;
        playerInventorySO.OnToolItemUnequipped -= ToolItemUnequipped;

        playerInventorySO.OnWeaponItemEquipped -= WeaponItemEquipped;
        playerInventorySO.OnWeaponItemUnequipped -= WeaponItemUnequipped;
    
        if(currentToolItemBehaviour != null){
            currentToolItemBehaviour.OnHolsterAnimationCompleted -= ToolUnequipAnimationCompleted;
        }

        if(currentWeaponItemBehaviour != null){
            currentWeaponItemBehaviour.OnHolsterAnimationCompleted -= ToolUnequipAnimationCompleted;
        }
    }

    public void HolsterActiveWeapon(){
        if(currentWeaponItemBehaviour != null && currentWeaponItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentWeaponItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
        }
    }

    public void HolsterActiveTool(){
        if(currentToolItemBehaviour != null && currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentToolItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
        }
    }

    public void UnholsterActiveWeapon(){
        if(currentToolItemBehaviour != null && currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentToolItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
        }
        
        if(currentWeaponItemBehaviour != null && currentWeaponItemBehaviour.GetCurrentItemState() == EquippableItemState.Holstered){
            currentWeaponItemBehaviour.ChangeItemState(EquippableItemState.Active);
        }
    }

    public void UnholsterActiveTool(){
        if(currentWeaponItemBehaviour != null && currentWeaponItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentWeaponItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
        }
        
        if(currentToolItemBehaviour != null && currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Holstered){
            currentToolItemBehaviour.ChangeItemState(EquippableItemState.Active);
        }
    }

    public void ActivateActiveTool(){
        if(currentToolItemBehaviour != null && currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Holstered){
            currentToolItemBehaviour.ChangeItemState(EquippableItemState.Active);
        }
    }

    public void HolsterEquippedItems(){
        if(currentWeaponItemBehaviour != null && currentWeaponItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentWeaponItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
            previousActiveItem = currentWeaponItemBehaviour;
            return;
        }

        if(currentToolItemBehaviour != null && currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentToolItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
            previousActiveItem = currentToolItemBehaviour;
        }
    }

    public void AttemptUnholsterPreviousActiveItem(){
        if(previousActiveItem != null){
            previousActiveItem.ChangeItemState(EquippableItemState.Active);
            previousActiveItem = null;
        }
    }

    public void SwitchActiveWeapon(){

    }

    public bool IsWeaponItemEquipped(ItemDataSO itemDataSO){
        if(currentWeaponItemBehaviour == null) return false;

        return currentWeaponItemBehaviour.GetItemData() == itemDataSO;
    }

    public bool IsToolItemEquipped(ItemDataSO itemDataSO){
        if(currentToolItemBehaviour == null) return false;

        return currentToolItemBehaviour.GetItemData() == itemDataSO;
    }

    public bool IsCurrentWeaponActive(){
        if(currentWeaponItemBehaviour == null) return false;
        
        if(currentWeaponItemBehaviour.GetCurrentItemState() == EquippableItemState.Active || currentWeaponItemBehaviour.GetCurrentItemState() == EquippableItemState.Passive) return true;

        return false;
    }

    public bool IsCurrentToolActive(){
        if(currentToolItemBehaviour == null) return false;
        
        if(currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Active || currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Passive) return true;

        return false;
    }

    public EquippedItemBehaviour GetCurrentWeaponItemBehaviour(){
        return currentWeaponItemBehaviour;
    }

    public EquippedItemBehaviour GetCurrentToolItemBehaviour(){
        return currentToolItemBehaviour;
    }

    private void ToolItemEquipped(object sender, PlayerInventorySO.EquippedItemEventArgs e){
        currentToolItemBehaviour = Instantiate(e.equippedItemBehaviour);

        SetupEquippedItem(e.inventoryItem, currentToolItemBehaviour);
        OnEmergencyItemBehaviourSpawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentToolItemBehaviour));
    }

    private void ToolItemUnequipped(InventoryItem inventoryItem){
        toolInventoryItemToEquip = inventoryItem;

        if(currentToolItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentToolItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
            currentToolItemBehaviour.OnHolsterAnimationCompleted += ToolUnequipAnimationCompleted;
            return;
        }

        ToolUnequipAnimationCompleted();
    }

    private void ToolUnequipAnimationCompleted(){
        currentToolItemBehaviour.OnHolsterAnimationCompleted -= ToolUnequipAnimationCompleted;
        OnEmergencyItemBehaviourDespawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentToolItemBehaviour));
        DestroyEquippedItem(currentToolItemBehaviour);

        if(toolInventoryItemToEquip != null){
            playerInventoryHandler.GetInventory().ToolUnequipAnimationFinished(toolInventoryItemToEquip);
        }
    }

    private void WeaponItemEquipped(object sender, PlayerInventorySO.EquippedItemEventArgs e){
        currentWeaponItemBehaviour = Instantiate(e.equippedItemBehaviour);

        SetupEquippedItem(e.inventoryItem, currentWeaponItemBehaviour);
        OnWeaponItemBehaviourSpawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentWeaponItemBehaviour));
    }

    private void WeaponItemUnequipped(InventoryItem inventoryItem){
        weaponInventoryItemToEquip = inventoryItem;

        if(currentWeaponItemBehaviour.GetCurrentItemState() == EquippableItemState.Active){
            currentWeaponItemBehaviour.ChangeItemState(EquippableItemState.Holstered);
            currentWeaponItemBehaviour.OnHolsterAnimationCompleted += WeaponUnequipAnimationCompleted;
            return;
        }

        WeaponUnequipAnimationCompleted();
    }

    private void WeaponUnequipAnimationCompleted(){
        currentWeaponItemBehaviour.OnHolsterAnimationCompleted -= ToolUnequipAnimationCompleted;
        OnWeaponItemBehaviourDespawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentWeaponItemBehaviour));
        DestroyEquippedItem(currentWeaponItemBehaviour);

        if(weaponInventoryItemToEquip != null){
            playerInventoryHandler.GetInventory().WeaponUnequipAnimationFinished(weaponInventoryItemToEquip);
        }
    }


    private void SetupEquippedItem(InventoryItem inventoryItem, EquippedItemBehaviour equippedItemBehaviour){
        equippedItemBehaviour.SetupItemBehaviour(inventoryItem, playerInputHandler, playerInventoryHandler);

        AssignPlayerItemDefaultHolster(equippedItemBehaviour);
        AssignDefaultItemState(equippedItemBehaviour);
    }

    private void DestroyEquippedItem(EquippedItemBehaviour equippedItemBehaviour){
        if(previousActiveItem == equippedItemBehaviour){
            previousActiveItem = null;
        }

        if(equippedItemBehaviour != null){
            equippedItemBehaviour.SaveData();

            Destroy(equippedItemBehaviour.gameObject);
        }
    }

    private void AssignPlayerItemDefaultHolster(EquippedItemBehaviour equippedItemBehaviour){
        switch (equippedItemBehaviour.GetPlayerItemHolsterType()){
            case EquippableItemHolsterType.None:
                break;
            case EquippableItemHolsterType.Side_Arm_Holster: equippedItemBehaviour.SetupPlayerItemHolster(sideArmHolsterTransform, activeItemTransform);
                break;
            case EquippableItemHolsterType.Back_Holster: equippedItemBehaviour.SetupPlayerItemHolster(backHolsterTransform, activeItemTransform);
                break;
            case EquippableItemHolsterType.Inactive_Emergency_Item_Holster: equippedItemBehaviour.SetupPlayerItemHolster(inactiveEmergencyItemHolsterTransform, activeItemTransform);
                break;
            case EquippableItemHolsterType.Flashlight_Holster: equippedItemBehaviour.SetupPlayerItemHolster(flashlightHolsterTransform, activeItemTransform);
                break;
        }
    }

    private void AssignDefaultItemState(EquippedItemBehaviour equippedItemBehaviour){
    switch (equippedItemBehaviour.GetDefaultItemState()){
            case EquippableItemState.None: Debug.LogError("Item: " + equippedItemBehaviour + " is marked as NONE on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
            case EquippableItemState.Used: Debug.LogError("Item: " + equippedItemBehaviour + " is marked as USED on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
        }

        if(equippedItemBehaviour.GetDefaultItemState() == EquippableItemState.Active){
            HolsterEquippedItems();
        }

        equippedItemBehaviour.TriggerDefaultState();

        if(equippedItemBehaviour.GetDefaultItemState() == EquippableItemState.Holstered){
            equippedItemBehaviour.ChangeItemState(EquippableItemState.Active);
        }
    }
}