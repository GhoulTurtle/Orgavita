using UnityEngine;

/// <summary>
/// Base scriptable object that all items derive from. Holds needed variables and methods for items.
/// </summary>
public abstract class ItemDataSO : ScriptableObject{
    [Header("Item Information")]
    [SerializeField] private string itemName;
    [SerializeField] private string itemQuickDescription;
    [SerializeField] private BasicDialogueSO itemInspectDescription;
    
    [Header("Item Visual")]
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private Transform itemModelPrefab;

    [Header("Item Settings")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private bool isCombinable = false;
    [SerializeField] private bool isStackable = true;
    [SerializeField] private bool isDestroyable = true;
    [SerializeField] private int maxStackSize = 1;

    public string GetItemName(){
        return itemName;
    }

    public string GetItemQuickDescription(){
        return itemQuickDescription;
    }

    public Sprite GetItemSprite(){
        return itemIcon;
    }

    public ItemType GetItemType(){
        return itemType;
    }

    public bool GetIsCombinable(){
        return isCombinable;
    }

    public bool GetIsStackable(){
        return isStackable;
    }

    public bool GetIsDestroyable(){
        return isDestroyable;
    }

    public int GetItemMaxStackSize(){
        return maxStackSize;
    }

    public virtual bool UseItem(){return true;}
}