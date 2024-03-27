using System.Collections;
using UnityEngine;

public class InventoryItemModelUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private Transform itemModelPivot;

    [Header("Item Model UI Variables")]
    [SerializeField] private float idleSpinStartDelay = 2f;
    [SerializeField] private float itemSpinSpeed = 0.15f;

    private Transform currentItemModel;

    private IEnumerator currentItemSpinDelayTimer;

    private IEnumerator currentItemSpinAnimation;

    private void Awake() {
        inventoryUI.OnSlotSelected += UpdateItemModelOnInventorySelected;

        playerInventoryHandler.OnInventoryStateChanged += EvaluateInventoryState;        
    }

    private void OnDestroy() {
        inventoryUI.OnSlotSelected -= UpdateItemModelOnInventorySelected;
        playerInventoryHandler.OnInventoryStateChanged -= EvaluateInventoryState;     
        StopAllCoroutines();                
    }
    
    private void EvaluateInventoryState(object sender, PlayerInventoryHandler.InventoryStateChangedEventArgs e){
        switch (e.inventoryState){
            case InventoryState.Default:
                if(playerInventoryHandler.CurrentInventoryState == InventoryState.ContextUI){
                    if(inventoryUI.GetSelectedInventoryItem().IsEmpty()){
                        DestroyItemModel();
                    }
                }
                break;
            case InventoryState.ContextUI:
                if(playerInventoryHandler.CurrentInventoryState == InventoryState.Combine){
                    CreateItemModel(inventoryUI.GetSelectedInventoryItem());
                }

                if(playerInventoryHandler.CurrentInventoryState == InventoryState.Inspect && currentItemModel != null){
                    currentItemSpinDelayTimer = IdleSpinStartTimerCorutine();
                    StartCoroutine(currentItemSpinDelayTimer);
                }
                break;
            case InventoryState.Inspect:
                StopItemIdleAnimationCoroutines();
                break;
        }
    }

    private void UpdateItemModelOnInventorySelected(object sender, InventoryUI.SlotSelectedEventArgs e){
        CreateItemModel(e.inventoryItemSelected);
    }

    private void DestroyItemModel(){
        StopItemIdleAnimationCoroutines();
        Destroy(currentItemModel.gameObject);
        currentItemModel = null;
    }

    private void CreateItemModel(InventoryItem inventoryItemSelected){
        if (currentItemModel != null){
            DestroyItemModel();
        }

        if (inventoryItemSelected.IsEmpty()) return;

        Transform itemModel = inventoryItemSelected.GetHeldItem().GetItemModel();

        if (itemModel == null) return;

        currentItemModel = Instantiate(itemModel, itemModelPivot);

        currentItemSpinDelayTimer = IdleSpinStartTimerCorutine();
        StartCoroutine(currentItemSpinDelayTimer);
    }

    private void StopItemIdleAnimationCoroutines(){
        if(currentItemSpinAnimation != null){
            StopCoroutine(currentItemSpinAnimation);
            currentItemSpinAnimation = null;
        }

        if(currentItemSpinDelayTimer != null){
            StopCoroutine(currentItemSpinDelayTimer);
            currentItemSpinDelayTimer = null;
        }
    }

    private IEnumerator IdleSpinStartTimerCorutine(){
        yield return new WaitForSeconds(idleSpinStartDelay);

        currentItemSpinAnimation = IdleSpinCorutine();

        StartCoroutine(currentItemSpinAnimation);

        currentItemSpinDelayTimer = null;
    }

    private IEnumerator IdleSpinCorutine(){
        while(true){
            currentItemModel.Rotate(0, itemSpinSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
