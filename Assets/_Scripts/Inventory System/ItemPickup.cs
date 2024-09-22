using UnityEngine;
using UnityEngine.Events;

public class ItemPickup : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private Transform itemVisualParent;
    [SerializeField] private ParticleSystem itemParticleSystem;
    [SerializeField] private AudioSource itemPickupAudioSource;

    [Header("Item Pickup Variables")]
    [SerializeField] private ItemDataSO itemToPickup;
    [SerializeField] private int itemPickupStackAmount;

    [Header("Item Popup Text Variables")]
    [SerializeField] private bool displayPopup = true;
    [SerializeField] private float popupPrintSpeed = 0.01f;
    [SerializeField] private float popupWaitTime = 1f;
    [SerializeField] private float popupFadeTime = 1.5f;

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

        if(itemRemainder == itemPickupStackAmount){
            //Full inventory
            if(PopupUI.Instance != null && displayPopup){
                Dialogue pickupDialogue = new Dialogue{
                    sentence = "<color=\"red\">Inventory full.</color>",
                };

                PopupUI.Instance.PrintText(pickupDialogue, popupPrintSpeed, true, popupWaitTime, popupFadeTime);
            }
        }

        if(itemRemainder != itemPickupStackAmount){
            //Picked up items
            if(itemPickupAudioEvent != null){
                itemPickupAudioEvent.Play(itemPickupAudioSource);
            }

            if(PopupUI.Instance != null && displayPopup){
                int itemsPickedUp = itemPickupStackAmount - itemRemainder;
                Dialogue pickupDialogue = new Dialogue{
                    sentence = itemPickupStackAmount != 1 ? "Picked up " + itemToPickup.GetItemName() + " X" + itemsPickedUp : "Picked up " + itemToPickup.GetItemName(),
                };

                PopupUI.Instance.PrintText(pickupDialogue, popupPrintSpeed, true, popupWaitTime, popupFadeTime);
            }
        }

        if(itemRemainder == 0){
            if(currentItemModel != null){
                Destroy(currentItemModel.gameObject);
            }

            if(itemParticleSystem != null){
                itemParticleSystem.Stop();
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
        
            if(itemParticleSystem == null) return;

            Mesh itemMesh = currentItemModel.GetItemMesh();

            if(itemMesh != null){
                var particleShape = itemParticleSystem.shape;
                if(particleShape.shapeType == ParticleSystemShapeType.Mesh){
                    particleShape.mesh = itemMesh;
                    
                    particleShape.rotation = currentItemModel.transform.localEulerAngles;
                    
                    particleShape.scale = currentItemModel.transform.localScale;
                }
            }
        }

        if(itemParticleSystem == null) return;

        itemParticleSystem.Play(); 
    }
}