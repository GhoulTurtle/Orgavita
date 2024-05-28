using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon Item", fileName = "NewWeaponItemDataSO")]
public class WeaponItemDataSO : ItemDataSO{
    [Header("Weapon Item Variables")]
    public WeaponItemBehaviour weaponItemBehaviour;

    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        playerInventoryHandler.GetInventory().EquipWeaponItem(selectedItem);
        base.UseItem(selectedItem, playerInventoryHandler, out resultMessage);
    }

    public WeaponItemBehaviour GetWeaponItemBehaviour(){
        return weaponItemBehaviour;
    }
}