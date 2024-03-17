using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable{
    [Header("Item Pickup Variables")]
    [SerializeField] private ItemDataSO itemToPickup;
    [SerializeField] private int itemPickupStackAmount;

    public string InteractionPrompt {get; private set;}

    private void Awake() {
        if(itemPickupStackAmount <= 0) itemPickupStackAmount = 0;
        if(itemToPickup != null){
            InteractionPrompt = "Pickup " + itemToPickup.GetItemName();
        }
    }

    public void PickupItem(PlayerInventorySO playerInventory){
        var itemRemainder = playerInventory.AttemptToAddItemToInventory(itemToPickup, itemPickupStackAmount);

        if(itemRemainder == 0){
            Destroy(gameObject);
            return;
        }

        itemPickupStackAmount = itemRemainder;
    }

    public bool Interact(PlayerInteract player){
        if(player.TryGetComponent(out PlayerInventoryHandler playerInventoryHandler)){
            PickupItem(playerInventoryHandler.GetInventory());
            return true;
        }

        return false;
    }
}