using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

//Handles the input, spawning/despawning, and holstering of any item that the player has equipped.

public class PlayerEquippedItemHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private Transform activeItemTransform;

    [Header("Required Holster References")]
    [SerializeField] private Transform sideArmHolsterTransform;
    [SerializeField] private Transform backHolsterTransform;
    [SerializeField] private Transform inactiveEmergencyItemHolsterTransform;
    [SerializeField] private Transform flashlightHolsterTransform;

    private EmergencyItemBehaviour currentEmergencyItemBehaviour;
    private WeaponItemBehaviour currentWeaponItemBehaviour;

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

    public void EmergencyItemUseInput(InputAction.CallbackContext context){

    }

    public void HolsterWeaponInput(InputAction.CallbackContext context){

    }

    public void ReloadWeaponInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return; 

        if(context.interaction is HoldInteraction){
            Debug.Log("Holding");
        }
        else{
            Debug.Log("Multi-Tap");
        }
    }

    public void WeaponUseInput(InputAction.CallbackContext context){

    }

    public void WeaponAltUseInput(InputAction.CallbackContext context){

    }

    private void EmergencyItemEquipped(object sender, PlayerInventorySO.EmergencyItemEquippedEventArgs e){
        currentEmergencyItemBehaviour = Instantiate(e.equippedEmergencyItemBehaviour);

        currentEmergencyItemBehaviour.SetupItemBehaviour(this);

        PlayerItem emergencyPlayerItem = currentEmergencyItemBehaviour.GetPlayerItem();

        AssignPlayerItemDefaultHolster(emergencyPlayerItem);
        AssignDefaultItemState(emergencyPlayerItem);

        Debug.Log(currentEmergencyItemBehaviour);
    }

    private void EmergencyItemUnequipped(object sender, EventArgs e){
        if(currentEmergencyItemBehaviour != null){
            Destroy(currentEmergencyItemBehaviour.gameObject);
            currentEmergencyItemBehaviour = null;
        }
    }

    private void WeaponItemEquipped(object sender, PlayerInventorySO.WeaponItemEquippedEventArgs e){
        currentWeaponItemBehaviour = e.equippedWeaponItemBehaviour;

        currentWeaponItemBehaviour.SetupItemBehaviour(this);

        PlayerItem weaponPlayerItem = currentWeaponItemBehaviour.GetPlayerItem();

        AssignPlayerItemDefaultHolster(weaponPlayerItem);
        AssignDefaultItemState(weaponPlayerItem);

        Debug.Log(currentWeaponItemBehaviour);
    }

    private void WeaponItemUnequipped(object sender, EventArgs e){
        if(currentWeaponItemBehaviour != null){
            
            currentWeaponItemBehaviour.SaveData();

            Destroy(currentWeaponItemBehaviour.gameObject);
            currentWeaponItemBehaviour = null;
        }
    }

    private void AssignPlayerItemDefaultHolster(PlayerItem playerItem){
        switch (playerItem.GetPlayerItemHolsterType()){
            case PlayerItemHolsterType.None:
                break;
            case PlayerItemHolsterType.Side_Arm_Holster: playerItem.SetupPlayerItemHolster(sideArmHolsterTransform);
                break;
            case PlayerItemHolsterType.Back_Holster: playerItem.SetupPlayerItemHolster(backHolsterTransform);
                break;
            case PlayerItemHolsterType.Inactive_Emergency_Item_Holster: playerItem.SetupPlayerItemHolster(inactiveEmergencyItemHolsterTransform);
                break;
            case PlayerItemHolsterType.Flashlight_Holster: playerItem.SetupPlayerItemHolster(flashlightHolsterTransform);
                break;
        }
    }

    private void AssignDefaultItemState(PlayerItem playerItem){
    switch (playerItem.GetPlayerItemState()){
            case PlayerItemState.None: Debug.LogError("Item: " + " is marked as NONE on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
            case PlayerItemState.Active: Debug.LogError("Item: " + " is marked as ACTIVE on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
            case PlayerItemState.Used: Debug.LogError("Item: " + playerItem + " is marked as USED on the time of spawning. This is incorrect. Did you forget to change the behaviour PlayerItemState?");
                return;
        }

        playerItem.TriggerDefaultState();
    }

    private void HolsterActiveWeapon(bool holsterWeapon = false){
        //Make any item that is active be holstered
        if(currentEmergencyItemBehaviour != null){
            PlayerItem currentEmergencyPlayerItem = currentEmergencyItemBehaviour.GetPlayerItem();

            if(currentEmergencyPlayerItem.PlayerItemState == PlayerItemState.Active){
                currentEmergencyPlayerItem.ChangeItemState(PlayerItemState.Holstered);
            } 
        }
        
        if(holsterWeapon && currentWeaponItemBehaviour != null){

        }
    }
}