public class ComboResult{
    private ComboResultType resultType;
    private InventoryItem resultInventoryItem;

    public string SetComboResult(ComboResultType _resultType, InventoryItem _resultInventoryItem){
        resultType = _resultType;
        resultInventoryItem = _resultInventoryItem;
        return GetCombineResultMessage(resultType);
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

    private string GetCombineResultMessage(ComboResultType comboResult){
        return comboResult switch{
            ComboResultType.Invalid_Combo => "Can't combine those.",
            ComboResultType.Invalid_Weapon_Resource_Combo => "Wrong ammo type.",
            ComboResultType.Invalid_Emergency_Item_Resource_Combo => "Wrong type of resource.",
            ComboResultType.Invalid_Stack_Combo => "Can't stack those.",
            ComboResultType.Valid_Combo => "Made " + GetResultItemName() + " X" + GetResultItemStack() + ".",
            ComboResultType.Valid_Stack_Combo => "Successfully Stacked " + GetResultItemName() + " X" + GetResultItemStack() + ".",
            ComboResultType.Valid_Weapon_Resource_Combo => "Loaded " + GetResultItemName(),
            ComboResultType.Valid_Emergency_Item_Resource_Combo => "Recharged " + GetResultItemName(),
            ComboResultType.Full_Weapon => GetResultItemName() + " is already loaded.",
            ComboResultType.Full_Emergency_Item => GetResultItemName() + " is already charged.",
            ComboResultType.Full_Inventory => "Can't hold anymore items.",
            ComboResultType.Opened_Combination_Menu => "Use combination menu to choose the amount of items to make.",
            _ => "ERROR NO COMBO RESULT TYPE FOUND",
    };
}

    
}
