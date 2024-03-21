using UnityEngine;

/// <summary>
/// A scriptable object that holds the conditions and results of a inventory recipe.
/// </summary>

[CreateAssetMenu(menuName = "Inventory/Inventory Recipe", fileName = "NewInventoryRecipeSO")]
public class InventoryRecipeSO : ScriptableObject{
    [Header("Crafting Settings")]
    [SerializeField] private bool returnOneItem = false;

    [Header("Crafting Inputs")]
    [SerializeField] private ItemDataSO combineItemDataOne;
    [SerializeField] private ItemDataSO combineItemDataTwo;

    [Header("Crafting Outputs")]
    [SerializeField] private ItemDataSO resultItemData;
    [SerializeField] private int resultStackAmount;

    public bool ReturnIsValidCombination(ItemDataSO firstItem, ItemDataSO secondItem){
        if(firstItem == null || secondItem == null){
            return false;
        }

        if(combineItemDataOne == firstItem && combineItemDataTwo == secondItem || combineItemDataOne == secondItem && combineItemDataTwo == firstItem){
            return true;
        }

        return false;
    }

    public ItemDataSO GetResultItemData(){
        return resultItemData;
    }

    public int GetResultItemStackAmount(){
        if(returnOneItem) return 1;
        if(resultItemData.GetItemMaxStackSize() < resultStackAmount) return resultItemData.GetItemMaxStackSize();
        else return resultStackAmount;
    }
}