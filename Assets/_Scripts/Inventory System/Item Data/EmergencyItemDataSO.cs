using UnityEngine;

[CreateAssetMenu(menuName = "Item/Emergency Item", fileName = "NewEmergencyItemDataSO")]
public class EmergencyItemDataSO : ItemDataSO{
    [Header("Emergency Item Variables")]
    public EquippedItemBehaviour emergencyItemBehaviour;

    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        playerInventoryHandler.GetInventory().EquipEmergencyItem(selectedItem);
        base.UseItem(selectedItem, playerInventoryHandler, out resultMessage);
    }

    public EquippedItemBehaviour GetEquippedItemBehaviour(){
        return emergencyItemBehaviour;
    }
}
