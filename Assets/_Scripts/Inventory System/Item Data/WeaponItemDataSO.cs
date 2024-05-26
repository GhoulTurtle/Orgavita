using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon Item", fileName = "NewWeaponItemDataSO")]
public class WeaponItemDataSO : ItemDataSO{
    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        playerInventoryHandler.GetInventory().EquipItem(selectedItem);
        base.UseItem(selectedItem, playerInventoryHandler, out resultMessage);
    }
}