using UnityEngine;

[CreateAssetMenu(menuName = "Item/Key Item", fileName = "NewKeyItemDataSO")]
public class KeyItemDataSO : ItemDataSO{
    [Header("Key Item Variables")]
    [SerializeField] private string itemSuccessfullyUseMessage = "";
    [SerializeField] private string itemCannotUseMessage = "I can't use this here...";
    [SerializeField] private string itemFailedUseMessage = "";

    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        resultMessage = itemCannotUseMessage;
    }
}
