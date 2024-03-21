using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboResult{
    private ComboResultType resultType;
    private InventoryItem resultInventoryItem;

    public void SetComboResult(ComboResultType _resultType, InventoryItem _resultInventoryItem){
        resultType = _resultType;
        resultInventoryItem = _resultInventoryItem;
    }

    public ComboResultType GetComboResultType(){
        return resultType;
    }

    public string GetResultItemName(){
        return resultInventoryItem.GetHeldItem().GetItemName();
    }

    public int GetResultItemStack(){
        return resultInventoryItem.GetCurrentStack();
    }
}
