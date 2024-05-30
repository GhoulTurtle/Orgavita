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

    private void Awake() {
        playerInventorySO = playerInventoryHandler.GetInventory();
        
        playerInventorySO.OnEmergencyItemUnequipped += EmergencyItemUnequipped;
        playerInventorySO.OnEmergencyItemEquipped += EmergencyItemEquipped;

        playerInventorySO.OnWeaponItemUnequipped += WeaponItemUnequipped;
        playerInventorySO.OnWeaponItemEquipped += WeaponItemEquipped;
    }

    private void OnDestroy() {
        playerInventorySO.OnEmergencyItemUnequipped -= EmergencyItemUnequipped;
        playerInventorySO.OnEmergencyItemEquipped -= EmergencyItemEquipped;

        playerInventorySO.OnWeaponItemUnequipped -= WeaponItemUnequipped;
        playerInventorySO.OnWeaponItemEquipped -= WeaponItemEquipped;
    }

    private void EmergencyItemEquipped(object sender, PlayerInventorySO.EquippedItemEventArgs e){
        currentEmergencyItemBehaviour = Instantiate(e.equippedItemBehaviour);

        SetupEquippedItem(e.inventoryItem, currentEmergencyItemBehaviour);
    }

    private void EmergencyItemUnequipped(object sender, EventArgs e){
        DestroyEquippedItem(currentEmergencyItemBehaviour);
        currentEmergencyItemBehaviour = null;
    }

    private void WeaponItemEquipped(object sender, PlayerInventorySO.EquippedItemEventArgs e){
        currentWeaponItemBehaviour = Instantiate(e.equippedItemBehaviour);

        SetupEquippedItem(e.inventoryItem, currentWeaponItemBehaviour);
    }

    private void WeaponItemUnequipped(object sender, EventArgs e){
        DestroyEquippedItem(currentWeaponItemBehaviour);
        currentWeaponItemBehaviour = null;
    }

    private void SetupEquippedItem(InventoryItem inventoryItem, EquippedItemBehaviour equippedItemBehaviour){
        equippedItemBehaviour.SetupItemBehaviour(inventoryItem, playerInputHandler);

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