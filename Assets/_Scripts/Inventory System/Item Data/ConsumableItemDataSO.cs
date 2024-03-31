using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable Item", fileName = "NewConsumableItemDataSO")]
public class ConsumableItemDataSO : ItemDataSO{
    [Header("Consumable Item Variables")]
    [SerializeField] private List<ConsumableJob> consumableItemJobList;
    [SerializeField] private string itemUsedMessage = "";

    public override void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        bool validJob = true;
        string resultString = itemUsedMessage;

        if(itemUsedMessage == ""){
            resultString = "Used " + GetItemName();
        }

        //Check if valid consume job(s)
        for (int i = 0; i < consumableItemJobList.Count; i++){
            validJob = consumableItemJobList[i].CheckValidConsumeJob(playerInventoryHandler, out string checkMessage);
            if (!validJob){
                resultString = checkMessage;
                break;
            }
        }

        if(!validJob){
            resultMessage = resultString;
            return;
        }

        //If all consume jobs are valid then perform them and get rid of 1 item from the item stack
        for (int i = 0; i < consumableItemJobList.Count; i++){
            consumableItemJobList[i].PerformConsumeJob(playerInventoryHandler, 0);
        }

        if(selectedItem.IsStackable()){
            selectedItem.RemoveFromStack(1);
        }
        else{
            selectedItem.ClearItem();
        }
        resultMessage = resultString;
    }
}