using System;
using UnityEngine;

public class ItemModel : MonoBehaviour, IInteractable{
    public string InteractionPrompt {get; private set;}
    
    public Action<PlayerInventorySO> OnModelInteracted;

    private Collider itemCollider;

    private void Awake() {
        if(!TryGetComponent(out itemCollider)){
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.5f, 0.1f, 0.5f);
            itemCollider = collider;
        }
    }

    public bool Interact(PlayerInteract player){
        if(player.TryGetComponent(out PlayerInventoryHandler playerInventoryHandler)){
            OnModelInteracted?.Invoke(playerInventoryHandler.GetInventory());
            return true;
        }
        return true;
    }

    public void UpdateInteractionText(string itemName, int stackAmount){
        if(stackAmount > 1){
            InteractionPrompt = "Pickup " + itemName + " X" + stackAmount;
            return;
        }
        InteractionPrompt = "Pickup " + itemName;
    }

    public Mesh GetItemMesh(){
        if(TryGetComponent(out MeshFilter meshFilter)){
            return meshFilter.mesh;
        }

        else return null;
    }
}
