using UnityEngine;

[CreateAssetMenu(menuName = "Item/Tool", fileName = "NewToolItemDataSO")]
public class ToolItemDataSO : ItemDataSO{
    [Header("Tool Variables")]
    public EquippedItemBehaviour toolBehaviour;

    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        playerInventoryHandler.GetInventory().EquipToolItem(selectedItem);
        base.UseItem(selectedItem, playerInventoryHandler, out resultMessage);
    }

    public EquippedItemBehaviour GetEquippedItemBehaviour(){
        return toolBehaviour;
    }
}
