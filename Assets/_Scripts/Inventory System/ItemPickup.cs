using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable{
    [Header("Required References")]
    [SerializeField] private Transform itemVisualParent;

    [Header("Item Pickup Variables")]
    [SerializeField] private ItemDataSO itemToPickup;
    [SerializeField] private int itemPickupStackAmount;

    private Transform currentItemModel;

    public string InteractionPrompt {get; private set;}

    private void Awake() {
        if(itemToPickup == null) return;
        SetupItemPickup();
    }

    public void SetItemPickup(ItemDataSO _itemToPickup, int _itemPickupStackAmount){
        itemToPickup = _itemToPickup;
        itemPickupStackAmount = _itemPickupStackAmount;

        if(currentItemModel != null){
            Destroy(currentItemModel.gameObject);
            currentItemModel = null;
        }

        SetupItemPickup();
    }

    public void PickupItem(PlayerInventorySO playerInventory){
        var itemRemainder = playerInventory.AttemptToAddItemToInventory(itemToPickup, itemPickupStackAmount);

        if(itemRemainder == 0){
            Destroy(gameObject);
            return;
        }

        itemPickupStackAmount = itemRemainder;
        
        UpdateInteractionText();
    }

    public bool Interact(PlayerInteract player){
        if(player.TryGetComponent(out PlayerInventoryHandler playerInventoryHandler)){
            PickupItem(playerInventoryHandler.GetInventory());
            return true;
        }

        return false;
    }

    private void SetupItemPickup(){
        if(itemPickupStackAmount <= 0) itemPickupStackAmount = 1;

        var itemModel = itemToPickup.GetItemInWorldModel();

        if(itemModel != null){
            currentItemModel = Instantiate(itemModel, itemVisualParent);
        }

        UpdateInteractionText();
    }

    private void UpdateInteractionText(){
        if(itemToPickup != null){
            if(itemPickupStackAmount > 1){
                InteractionPrompt = "Pickup " + itemToPickup.GetItemName() + " X" + itemPickupStackAmount;
                return;
            }
            InteractionPrompt = "Pickup " + itemToPickup.GetItemName();
        }
    }
}