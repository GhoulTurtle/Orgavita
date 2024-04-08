using UnityEngine;

[CreateAssetMenu(menuName = "Item/Key Item", fileName = "NewKeyItemDataSO")]
public class KeyItemDataSO : ItemDataSO{
    [Header("Key Item Variables")]
    [SerializeField] private string itemCannotUseMessage = "I can't use this here...";

    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        ItemLevelMechanic currentLevelMechanic = playerInventoryHandler.GetCurrentLevelMechanic();
        if(currentLevelMechanic == null){
            resultMessage = itemCannotUseMessage;
            return;
        }

        resultMessage = currentLevelMechanic.AttemptLevelMechanicInteraction(this, selectedItem);
    }
}