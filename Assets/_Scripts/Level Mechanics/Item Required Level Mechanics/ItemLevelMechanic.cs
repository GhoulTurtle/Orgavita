using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ItemLevelMechanic : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private InspectInteractable inspectInteractable;

    [Header("Key Item Variables")]
    [SerializeField] private KeyItemDataSO associatedKeyItemDataSO;
    [SerializeField] private bool consumeItem = false;
    [SerializeField] private bool isOneShot = false;

    [Header("Dialogue Variables")]
    [SerializeField] private BasicDialogueSO hintDialogue;
    [SerializeField] private BasicDialogueSO unlockedDialogue;
    [SerializeField] private string wrongItemMessage = "That item won't work.";
    [SerializeField] private string alreadyUnlockedMessage = "It's already unlocked.";
    [SerializeField] private string correctItemMessage = "Used ";

    [Header("Events")]
    public UnityEvent OnUnlockEvent;

    private PlayerInventoryHandler playerInventoryHandler;

    private const string PLAYER_TAG = "Player";

    private bool isUnlocked = false;
    
    private void Awake() {
        if(inspectInteractable != null){
            inspectInteractable.SetInspectDialogue(hintDialogue);
        }
    }

    public virtual string AttemptLevelMechanicInteraction(KeyItemDataSO keyItemDataSO, InventoryItem inventoryItem){
        if(isUnlocked && isOneShot){
            return alreadyUnlockedMessage;
        }
        
        if(keyItemDataSO != associatedKeyItemDataSO){
            return wrongItemMessage;
        }

        TriggerLevelMechanic(inventoryItem);
        return correctItemMessage + keyItemDataSO.GetItemName() + ".";
    }

    public virtual void TriggerLevelMechanic(InventoryItem inventoryItem){
        if(consumeItem){
            inventoryItem.RemoveFromStack(1);
        }

        if(inspectInteractable != null){
            inspectInteractable.SetInspectDialogue(unlockedDialogue);
        }

        if(unlockedDialogue == null && isOneShot && inspectInteractable != null){
            if(playerInventoryHandler.GetCurrentLevelMechanic() == this){
                playerInventoryHandler.RemoveCurrentLevelMechanic(this);
            }
            
            Destroy(inspectInteractable);
        }

        OnUnlockEvent?.Invoke();
        isUnlocked = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag(PLAYER_TAG)) return;

        if(playerInventoryHandler == null){
            if(!other.TryGetComponent(out playerInventoryHandler)){
                Debug.LogError("Did not find a PlayerInventoryHandler component on " + other.gameObject.name + ".");
            }
        }

        playerInventoryHandler.AddCurrentLevelMechanic(this);
    }

    private void OnTriggerStay(Collider other) {
        if(!other.CompareTag(PLAYER_TAG) || playerInventoryHandler == null) return;

        if(playerInventoryHandler.GetCurrentLevelMechanic() == null){
            playerInventoryHandler.AddCurrentLevelMechanic(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(!other.CompareTag(PLAYER_TAG) || playerInventoryHandler == null) return;

        if(playerInventoryHandler.GetCurrentLevelMechanic() == this){
            playerInventoryHandler.RemoveCurrentLevelMechanic(this);
        }
    }
}