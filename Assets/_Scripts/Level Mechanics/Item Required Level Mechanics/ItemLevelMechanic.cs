using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ItemLevelMechanic : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private InspectInteractable inspectInteractable;

    [Header("Key Item Variables")]
    [SerializeField] private List<KeyItemDataSO> associatedKeyItemDataList = new List<KeyItemDataSO>();
    [SerializeField] private bool consumeItem = false;
    [SerializeField] private bool isOneShot = false;

    [Header("Dialogue Variables")]
    [SerializeField] private BasicDialogueSO hintDialogue;
    [SerializeField] private BasicDialogueSO unlockedDialogue;
    [SerializeField] private string wrongItemMessage = "That item won't work.";
    [SerializeField] private string alreadyUnlockedMessage = "I can't use this here...";
    [SerializeField] private string correctItemMessage = "Used ";

    [Header("Events")]
    public UnityEvent OnUnlockEvent;
    public Action<ItemDataSO> OnUnlockAction;

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
        
        if(!associatedKeyItemDataList.Contains(keyItemDataSO)){
            return wrongItemMessage;
        }

        TriggerLevelMechanic(inventoryItem);
        if(correctItemMessage == "Used "){
            return correctItemMessage + keyItemDataSO.GetItemName() + ".";
        }
        else{
            return correctItemMessage;
        }
    }

    public virtual void TriggerLevelMechanic(InventoryItem inventoryItem){
        ItemDataSO itemDataSO = inventoryItem.GetHeldItem();

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
        OnUnlockAction?.Invoke(itemDataSO);
        isUnlocked = true;
    }

    public void SetIsUnlocked(bool state){
        if(isUnlocked == state) return;

        isUnlocked = state;

        if(!isUnlocked){
            if(inspectInteractable != null){
               inspectInteractable.SetInspectDialogue(hintDialogue);
            }
        }
        else{
            if(inspectInteractable != null){
               inspectInteractable.SetInspectDialogue(unlockedDialogue);
            }
        }
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