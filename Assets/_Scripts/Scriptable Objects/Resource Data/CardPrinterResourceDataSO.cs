using UnityEngine;

[CreateAssetMenu(menuName = "Resource Data/Card Printer Resource Data", fileName = "NewCardPrinterDataSO")]
public class CardPrinterResourceDataSO : ResourceDataSO{
    public ItemDataSO currentHeldItemData;

    #if UNITY_EDITOR
    public new void OnEnable(){
        base.OnEnable();
    }
    #endif

    public void HoldNewItem(ItemDataSO itemDataToHold){
        if(IsHoldingItem()) return;
    
        currentHeldItemData = itemDataToHold;
        AddItemStack(1);
        OnResourceUpdated?.Invoke(1);
    }

    public override void RemoveItem(){
        if(!IsHoldingItem()) return;
        
        currentStack--;
        if(currentStack == 0){
            currentHeldItemData = null;
        }
        OnResourceUpdated?.Invoke(-1);
    }

    public void ClearItem(){
        if(!IsHoldingItem()) return;
        
        int amountRemoved = currentStack;

        currentStack = 0;
        currentHeldItemData = null;

        OnResourceUpdated?.Invoke(amountRemoved);
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

    public override void ResetResourceData(){
        if(resetResourceStack){ 
            currentStack = defaultStack;
            currentHeldItemData = null;
        }
    }
}