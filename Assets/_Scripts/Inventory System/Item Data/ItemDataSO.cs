using UnityEngine;

/// <summary>
/// Base scriptable object that all items derive from. Holds needed variables and methods for items.
/// </summary>

[CreateAssetMenu(menuName = "Item/Basic Item", fileName = "NewItemDataSO")]
public class ItemDataSO : ScriptableObject{
    [Header("Item Information")]
    [SerializeField] private string itemName;
    [SerializeField] private string itemQuickDescription;
    [SerializeField, TextArea(3,3)] private string itemInspectDescription;
    
    [Header("Item Visual")]
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private Transform itemInventoryModelPrefab;
    [SerializeField] private ItemModel itemInWorldModelPrefab;

    [Header("Item Audio")]
    [SerializeField] private AudioEvent itemPickupAudioEvent;
    [SerializeField] private AudioEvent itemUseAudioEvent;
    [SerializeField] private AudioEvent itemDestoryAudioEvent;

    [Header("Item Settings")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private bool isUseable = true;
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

    public string GetItemInspectDescription(){
        return itemInspectDescription;
    }

    public Sprite GetItemSprite(){
        return itemIcon;
    }

    public Transform GetItemInventoryModel(){
        return itemInventoryModelPrefab;
    }

    public ItemModel GetItemInWorldModel(){
        return itemInWorldModelPrefab;
    }

    public AudioEvent GetPickupAudioEvent(){
        return itemPickupAudioEvent;
    }

    public AudioEvent GetUseAudioEvent(){
        return itemUseAudioEvent;
    }

    public AudioEvent GetDiscardAudioEvent(){
        return itemDestoryAudioEvent;
    }

    public ItemType GetItemType(){
        return itemType;
    }

    public bool GetIsUseable(){
        return isUseable;
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

    public virtual void UseItem(InventoryItem selectedItem, PlayerInventoryHandler playerInventoryHandler, out string resultMessage){
        resultMessage = "Used " + itemName;
    }
}