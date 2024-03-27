using System;
using System.Collections;
using UnityEngine;

public class InventoryItemModelUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private RectTransform itemModelRender;
    [SerializeField] private Transform itemModelPivot;

    [Header("Item Model Animation Variables")]
    [SerializeField] private float idleSpinStartDelay = 2f;
    [SerializeField] private float itemSpinSpeed = 0.15f;

    [Header("Inspect Animation Variables")]
    [SerializeField] private Vector2 inspectAnimationSlideOffset;
    [SerializeField] private Vector2 inspectAnimationScale = new Vector2(3, 3);
    [SerializeField] private float inspectAnimationTime = 1.25f;

    private const float ITEM_MODEL_SNAP = 0.01f;

    private Vector2 itemModelOriginalScale = Vector2.one;

    private Vector2 itemModelOriginalPosition;
    private Vector2 itemModelSlidePosition;

    private Transform itemModelPrefab;
    private Transform currentItemModel;

    private IEnumerator inspectInputCheck;

    private IEnumerator currentItemSpinDelayTimer;
    private IEnumerator currentItemSpinAnimation;
    private IEnumerator currentItemResetAnimation;

    private IEnumerator currentInspectMoveAnimation;
    private IEnumerator currentInspectScaleAnimation;

    private void Awake() {
        inventoryUI.OnSlotSelected += UpdateItemModelOnInventorySelected;

        playerInventoryHandler.OnInventoryStateChanged += EvaluateInventoryState;     
        
        itemModelOriginalPosition = itemModelRender.localPosition;
        itemModelSlidePosition = itemModelOriginalPosition + inspectAnimationSlideOffset;   
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

                if(playerInventoryHandler.CurrentInventoryState == InventoryState.Inspect){
                    if(inspectInputCheck != null){
                        StopCoroutine(inspectInputCheck);
                        inspectInputCheck = null;
                    }
                    ExitInspectStateAnimation();
                }
                break;
            case InventoryState.Inspect:
                StopItemIdleAnimationCoroutines();
                EnterInspectStateAnimation();
                
                inspectInputCheck = RotateModelOnInput();
                StartCoroutine(inspectInputCheck);
                break;
        }
    }

    private void EnterInspectStateAnimation(){
        StopInspectAnimationCoroutines();
        currentInspectMoveAnimation = UIAnimator.UILerpingAnimationCoroutine(itemModelRender, itemModelSlidePosition, inspectAnimationTime, false);
        StartCoroutine(currentInspectMoveAnimation);
        currentInspectScaleAnimation = UIAnimator.UIStretchAnimationCoroutine(itemModelRender, inspectAnimationScale, inspectAnimationTime, false);
        StartCoroutine(currentInspectScaleAnimation);
    }

    private void ExitInspectStateAnimation(){
        StopInspectAnimationCoroutines();
        currentInspectMoveAnimation = UIAnimator.UILerpingAnimationCoroutine(itemModelRender, itemModelOriginalPosition, inspectAnimationTime, false);
        StartCoroutine(currentInspectMoveAnimation);
        currentInspectScaleAnimation = UIAnimator.UIStretchAnimationCoroutine(itemModelRender, itemModelOriginalScale, inspectAnimationTime, false);
        StartCoroutine(currentInspectScaleAnimation);

        currentItemResetAnimation = ResetItemToDefault();
        StartCoroutine(currentItemResetAnimation);
    }

    private void StopInspectAnimationCoroutines(){
        if(currentInspectMoveAnimation != null){
            StopCoroutine(currentInspectMoveAnimation);
            currentInspectMoveAnimation = null;
        }
        if(currentInspectScaleAnimation != null){
            StopCoroutine(currentInspectScaleAnimation);
            currentInspectScaleAnimation = null;
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

        itemModelPrefab = inventoryItemSelected.GetHeldItem().GetItemModel();

        if (itemModelPrefab == null) return;

        currentItemModel = Instantiate(itemModelPrefab, itemModelPivot);

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

    private IEnumerator RotateModelOnInput(){
        if(currentItemModel == null) yield break;
        
        while (true){
            currentItemModel.Rotate(playerInventoryHandler.GetNavigationInput().y * itemSpinSpeed * Time.deltaTime, playerInventoryHandler.GetNavigationInput().x * itemSpinSpeed * Time.deltaTime, 0);
            yield return null;
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

    private IEnumerator ResetItemToDefault(){
        if(currentItemModel == null) yield break;
        
        float current = 0;

        while(Vector3.Distance(currentItemModel.rotation.eulerAngles, itemModelPrefab.rotation.eulerAngles) > ITEM_MODEL_SNAP){
            currentItemModel.rotation = Quaternion.Slerp(currentItemModel.rotation, itemModelPrefab.rotation, current / inspectAnimationTime);
            current += Time.deltaTime;
            yield return null;
        }

        currentItemModel.rotation = itemModelPrefab.rotation;

        currentItemSpinDelayTimer = IdleSpinStartTimerCorutine();
        StartCoroutine(currentItemSpinDelayTimer);
    }
}
