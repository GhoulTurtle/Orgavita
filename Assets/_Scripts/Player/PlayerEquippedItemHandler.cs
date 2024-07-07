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

    private EquippedItemBehaviour currentEmergencyItemBehaviour;
    private EquippedItemBehaviour currentWeaponItemBehaviour;

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

    private void Awake() {
        playerInventorySO = playerInventoryHandler.GetInventory();
        
        playerInventorySO.OnEmergencyItemEquipped += EmergencyItemEquipped;
        playerInventorySO.OnEmergencyItemUnequipped += EmergencyItemUnequipped;

        playerInventorySO.OnWeaponItemEquipped += WeaponItemEquipped;
        playerInventorySO.OnWeaponItemUnequipped += WeaponItemUnequipped;
    }

    private void OnDestroy() {
        playerInventorySO.OnEmergencyItemEquipped -= EmergencyItemEquipped;
        playerInventorySO.OnEmergencyItemUnequipped -= EmergencyItemUnequipped;

        playerInventorySO.OnWeaponItemEquipped -= WeaponItemEquipped;
        playerInventorySO.OnWeaponItemUnequipped -= WeaponItemUnequipped;
    }

    private void EmergencyItemEquipped(object sender, PlayerInventorySO.EquippedItemEventArgs e){
        currentEmergencyItemBehaviour = Instantiate(e.equippedItemBehaviour);

        SetupEquippedItem(e.inventoryItem, currentEmergencyItemBehaviour);
        OnEmergencyItemBehaviourSpawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentEmergencyItemBehaviour));
    }

    private void EmergencyItemUnequipped(object sender, EventArgs e){
        OnEmergencyItemBehaviourDespawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentEmergencyItemBehaviour));
        DestroyEquippedItem(currentEmergencyItemBehaviour);
    }

    private void WeaponItemEquipped(object sender, PlayerInventorySO.EquippedItemEventArgs e){
        currentWeaponItemBehaviour = Instantiate(e.equippedItemBehaviour);

        SetupEquippedItem(e.inventoryItem, currentWeaponItemBehaviour);
        OnWeaponItemBehaviourSpawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentWeaponItemBehaviour));
    }

    private void WeaponItemUnequipped(object sender, EventArgs e){
        OnWeaponItemBehaviourDespawned?.Invoke(this, new ItemBehaviourSpawnedEventArgs(currentWeaponItemBehaviour));
        DestroyEquippedItem(currentWeaponItemBehaviour);
    }

    private void SetupEquippedItem(InventoryItem inventoryItem, EquippedItemBehaviour equippedItemBehaviour){
        equippedItemBehaviour.SetupItemBehaviour(inventoryItem, playerInputHandler, playerInventoryHandler);

        AssignPlayerItemDefaultHolster(equippedItemBehaviour);
        AssignDefaultItemState(equippedItemBehaviour);
    }

    private void DestroyEquippedItem(EquippedItemBehaviour equippedItemBehaviour){
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
    switch (equippedItemBehaviour.GetPlayerItemState()){
            case EquippableItemState.None: Debug.LogError("Item: " + equippedItemBehaviour + " is marked as NONE on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
            case EquippableItemState.Active: Debug.LogError("Item: " + equippedItemBehaviour + " is marked as ACTIVE on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
            case EquippableItemState.Used: Debug.LogError("Item: " + equippedItemBehaviour + " is marked as USED on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
        }

        equippedItemBehaviour.TriggerDefaultState();
    }
}