using UnityEngine;

[CreateAssetMenu(menuName = "Resource Data/Card Printer Resource Data", fileName = "NewCardPrinterDataSO")]
public class CardPrinterResourceDataSO : ResourceDataSO{
    private ItemDataSO currentHeldItemData;

    public void HoldNewItem(ItemDataSO itemDataToHold){
        if(IsHoldingItem()) return;
    
        currentHeldItemData = itemDataToHold;
        AddItemStack(1);
    }

    public override void RemoveItem(){
        if(!IsHoldingItem() || IsEmpty()) return;
        
        currentStack--;
        if(IsEmpty()){
            currentHeldItemData = null;
        }
        OnResourceUpdated?.Invoke(-1);
    }

    public bool IsHoldingItem(){
        return currentHeldItemData != null;
    }

    public bool IsHeldItemCorrect(){
        if(!IsHoldingItem()) return false;

        return currentHeldItemData == itemDataToHold;
    }

    public ItemDataSO GetCurrentHeldItem(){
        return currentHeldItemData;
    }
}