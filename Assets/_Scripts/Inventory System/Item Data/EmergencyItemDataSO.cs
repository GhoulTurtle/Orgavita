using UnityEngine;

[CreateAssetMenu(menuName = "Item/Emergency Item", fileName = "NewEmergencyItemDataSO")]
public class EmergencyItemDataSO : ItemDataSO{
    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        playerInventoryHandler.GetInventory().EquipEmergencyItem(selectedItem);
        base.UseItem(selectedItem, playerInventoryHandler, out resultMessage);
    }
}
