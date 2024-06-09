using UnityEngine;

public class FearPistolBehaviour : EquippedItemBehaviour{
    [Header("Required References")]
    [SerializeField] private ResourceDataSO fearPistolResourceData;

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler);

        
    }

    public override void HolsterWeaponInput(object sender, InputEventArgs e){

    }

    public override void WeaponUseInput(object sender, InputEventArgs e){

    }

    public override void WeaponAltUseInput(object sender, InputEventArgs e){

    }
    
    public override void ReloadInput(object sender, InputEventArgs e){

    }

    public override void InspectInput(object sender, InputEventArgs e){
        
    }

    protected override void SubscribeToInputEvents(){
       playerInputHandler.OnHolsterWeapon += HolsterWeaponInput;
       playerInputHandler.OnWeaponUse += WeaponUseInput;
       playerInputHandler.OnAltWeaponUse += WeaponUseInput;
       playerInputHandler.OnReload += ReloadInput;
       playerInputHandler.OnInspect += InspectInput;
    }

    protected override void UnsubscribeFromInputEvents(){
        playerInputHandler.OnHolsterWeapon -= HolsterWeaponInput;
       playerInputHandler.OnWeaponUse -= WeaponUseInput;
       playerInputHandler.OnAltWeaponUse -= WeaponUseInput;
       playerInputHandler.OnReload -= ReloadInput;
       playerInputHandler.OnInspect -= InspectInput;
    }

    public override ResourceDataSO GetEquippedItemResourceData(){
        return fearPistolResourceData;
    }
}
