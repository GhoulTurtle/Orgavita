using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private Transform itemVisualParent;
    [SerializeField] private AudioSource itemPickupAudioSource;

    [Header("Item Pickup Variables")]
    [SerializeField] private ItemDataSO itemToPickup;
    [SerializeField] private int itemPickupStackAmount;

    [Header("Events")]
    [SerializeField] private UnityEvent OnPickupEvent;

    private ItemModel currentItemModel;

    private void Awake() {
        if(itemToPickup == null) return;
        SetupItemPickup();
    }

    private void OnDestroy() {
        if(currentItemModel != null){
            currentItemModel.OnModelInteracted -= PickupItem;
        }
    }

    public void SetItemPickup(ItemDataSO _itemToPickup, int _itemPickupStackAmount){
        itemToPickup = _itemToPickup;
        itemPickupStackAmount = _itemPickupStackAmount;

        if(currentItemModel != null){
            currentItemModel.OnModelInteracted -= PickupItem;
            Destroy(currentItemModel.gameObject);
            currentItemModel = null;
        }

        SetupItemPickup();
    }

    public void PickupItem(PlayerInventorySO playerInventory){
        AudioEvent itemPickupAudioEvent = itemToPickup.GetPickupAudioEvent();

        var itemRemainder = playerInventory.AttemptToAddItemToInventory(itemToPickup, itemPickupStackAmount);

        if(itemPickupAudioEvent != null && itemRemainder != itemPickupStackAmount){
            itemPickupAudioEvent.Play(itemPickupAudioSource);
        }

        if(itemRemainder == 0){
            if(currentItemModel != null){
                Destroy(currentItemModel.gameObject);
            }
        }

        itemPickupStackAmount = itemRemainder;
        
        currentItemModel.UpdateInteractionText(itemToPickup.GetItemName(), itemPickupStackAmount);
    }

    private void SetupItemPickup(){
        if(itemPickupStackAmount <= 0) itemPickupStackAmount = 1;

        ItemModel itemModel = itemToPickup.GetItemInWorldModel();

        if(itemModel != null){
            currentItemModel = Instantiate(itemModel, itemVisualParent);
            currentItemModel.OnModelInteracted += PickupItem;
            currentItemModel.UpdateInteractionText(itemToPickup.GetItemName(), itemPickupStackAmount);
        }
    }
}