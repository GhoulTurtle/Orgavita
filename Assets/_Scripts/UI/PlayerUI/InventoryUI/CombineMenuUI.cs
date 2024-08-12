using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombineMenuUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private MenuSelector menuSelector;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;

    [Header("UI References")]
    [SerializeField] private Transform combineMenuParent;
    [SerializeField] private Button combineButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button increaseCombineAmountButton;
    [SerializeField] private Button decreaseCombineAmountButton;
    [SerializeField] private CombineUI firstCombineUI;
    [SerializeField] private CombineUI secondCombineUI;
    [SerializeField] private CombineUI resultCombineUI;

    [Header("Combine Menu Animation Variables")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Vector2 animationPopoutOffset;

    private Vector2 combineMenuOriginalPosition;
    private Vector2 combineMenuPopupGoalPosition;

    private IEnumerator combineMenuPopoutAnimationCoroutine;

    private PlayerInventorySO.OpenCombinationMenuEventArgs currentCombinationEventArgs;

    private int currentCombinationAmount = 1;
    private int maxCombinationAmount;

    private void Awake() {
        DisableCombineMenuInteractivity();

        combineMenuOriginalPosition = combineMenuParent.localPosition;
        combineMenuPopupGoalPosition = combineMenuOriginalPosition + animationPopoutOffset;

        if(playerInventoryHandler != null){
            playerInventoryHandler.GetInventory().OnOpenCombinationMenu += SetupCombinationMenu;
            playerInventoryHandler.OnInventoryStateChanged += EvaluateInventoryState;
        }
    }

    private void Start() {
        SetupCombineUI();
    }

    private void OnDestroy() {
        StopAllCoroutines();

        if(playerInventoryHandler != null){
            playerInventoryHandler.GetInventory().OnOpenCombinationMenu -= SetupCombinationMenu;
        }
    }

    public void ShowCombineUI(){
        EnableCombineMenuInteractivity();
        menuSelector.SetTarget(combineButton.transform);
        
        if(combineMenuPopoutAnimationCoroutine != null){
            StopCoroutine(combineMenuPopoutAnimationCoroutine);
            combineMenuPopoutAnimationCoroutine = null;
        }
        
        combineMenuPopoutAnimationCoroutine = UIAnimator.LerpingAnimationCoroutine(combineMenuParent, combineMenuPopupGoalPosition, animationDuration, false);
        StartCoroutine(combineMenuPopoutAnimationCoroutine);
    }

    public void CombineItems(){
        //Do stuff here
        PlayerInventorySO playerInventorySO = playerInventoryHandler.GetInventory();
        ComboResult comboResult = new ComboResult();
        for (int i = 0; i < currentCombinationAmount; i++){
            playerInventorySO.AttemptCraftNewItem(currentCombinationEventArgs.firstItem, currentCombinationEventArgs.secondItem, comboResult, currentCombinationEventArgs.resultItem);
        }

        HideCombineUI();
        playerInventoryHandler.UpdateInventoryState(InventoryState.Default);
    }

    public void CancelCombineUI(){
        HideCombineUI();
        playerInventoryHandler.UpdateInventoryState(InventoryState.ContextUI);
    }

    public void IncreaseCombinationAmount(){
        currentCombinationAmount++;

        if(currentCombinationAmount > maxCombinationAmount){
            currentCombinationAmount = maxCombinationAmount;
        }

        UpdateCombinationUI();
    }

    public void DecreaseCombinationAmount(){
        currentCombinationAmount--;
        if(currentCombinationAmount < 1){
            currentCombinationAmount = 1;
        }

        UpdateCombinationUI();
    }


    private void HideCombineUI(){     
        DisableCombineMenuInteractivity();

        if(combineMenuPopoutAnimationCoroutine != null){
            StopCoroutine(combineMenuPopoutAnimationCoroutine);
            combineMenuPopoutAnimationCoroutine = null;
        }

        combineMenuPopoutAnimationCoroutine = UIAnimator.LerpingAnimationCoroutine(combineMenuParent, combineMenuOriginalPosition, animationDuration, false);
        StartCoroutine(combineMenuPopoutAnimationCoroutine);
    
        currentCombinationEventArgs = null;
    }

    private void EvaluateInventoryState(object sender, PlayerInventoryHandler.InventoryStateChangedEventArgs e){
        if(e.inventoryState != InventoryState.CombineUI && combineButton.interactable == true){
            HideCombineUI();
        }
    }

    private void EnableCombineMenuInteractivity(){
        combineButton.interactable = true;
        cancelButton.interactable = true;
        increaseCombineAmountButton.interactable = true;
        decreaseCombineAmountButton.interactable = true;
    }

    private void DisableCombineMenuInteractivity(){
        combineButton.interactable = false;
        cancelButton.interactable = false;
        increaseCombineAmountButton.interactable = false;
        decreaseCombineAmountButton.interactable = false;
    }

    private void SetupCombineUI(){
        firstCombineUI.SetupCombineUI(this);
        secondCombineUI.SetupCombineUI(this);
        resultCombineUI.SetupCombineUI(this);
    }

    private void SetupCombinationMenu(object sender, PlayerInventorySO.OpenCombinationMenuEventArgs e){
        playerInventoryHandler.UpdateInventoryState(InventoryState.CombineUI);
        
        currentCombinationEventArgs = e;
        
        currentCombinationAmount = 1;
        maxCombinationAmount = currentCombinationEventArgs.playerInventoryRecipeListSO.GetMaxAmountOfCombinations(e.firstItem, e.secondItem, e.resultItem.GetHeldItem());

        UpdateCombinationUI();

        ShowCombineUI();
    }

    private void UpdateCombinationUI(){
        firstCombineUI.UpdateCombineUI(currentCombinationEventArgs.firstItem.GetHeldItem().GetItemSprite(), currentCombinationAmount);
        secondCombineUI.UpdateCombineUI(currentCombinationEventArgs.secondItem.GetHeldItem().GetItemSprite(), currentCombinationAmount);
        resultCombineUI.UpdateCombineUI(currentCombinationEventArgs.resultItem.GetHeldItem().GetItemSprite(), 
        currentCombinationEventArgs.playerInventoryRecipeListSO.GetResultItemAmountByCombinations(currentCombinationEventArgs.resultItem.GetHeldItem(), currentCombinationAmount));
    }
}