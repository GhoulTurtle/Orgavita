using UnityEngine;

public class TestWeaponBehaviour : EquippedItemBehaviour{
    [SerializeField] private MeshRenderer meshRenderer;

    public override void SaveData(){
        playerInputHandler.OnHolsterWeapon -= HolsterWeaponInput;
    }

    public override void HolsterWeaponInput(object sender, InputEventArgs e){
        base.HolsterWeaponInput(sender, e);
    }

    public override void WeaponUseInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != UnityEngine.InputSystem.InputActionPhase.Performed) return;

        transform.rotation *= Quaternion.Euler(0, 15f, 0);
    }

    public override void WeaponAltUseInput(object sender, InputEventArgs e){
        if(e.inputActionPhase == UnityEngine.InputSystem.InputActionPhase.Performed){
            transform.localScale = Vector3.one * 0.4f;
        }
        else if(e.inputActionPhase == UnityEngine.InputSystem.InputActionPhase.Canceled){
            transform.localScale = Vector3.one * 0.2f;            
        }
    }

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler);
        playerInputHandler.OnHolsterWeapon += HolsterWeaponInput;
    }

    protected override void SubscribeToInputEvents(){
        playerInputHandler.OnWeaponUse += WeaponUseInput;
        playerInputHandler.OnAltWeaponUse += WeaponAltUseInput;
    }

    protected override void UnsubscribeFromInputEvents(){
        playerInputHandler.OnWeaponUse -= WeaponUseInput;
        playerInputHandler.OnAltWeaponUse -= WeaponAltUseInput;
    }
}