using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI selectedItemNameText;
    [SerializeField] private TextMeshProUGUI selectedItemDescriptionText;
    [SerializeField] private Transform contextUIParent;
    [SerializeField] private ContextButtonUI contextButtonTemplate;
    [SerializeField] private VerticalLayoutGroup contextButtonParent;
    [SerializeField] private Transform selectedItemDivider;

    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;

    [Header("Context Menu Variables")]
    [SerializeField] private float startingContextButtonSpacing = -100;
    [SerializeField] private float addContextButtonSpacing = 15;

    [Header("Context Menu Text")]
    [SerializeField] private Dialogue destroyConfirmationText;

    [Header("Context Menu Animation Variables")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Vector2 animationPopoutOffset;

    [Header("Inspect Animation Variables")]
    [SerializeField] private float inspectAnimationDuration = 1.5f;
    [SerializeField] private Vector2 itemDividerOffset;
    [SerializeField] private Vector4 inspectItemDescriptionMargins;

    private List<ContextButtonUI> currentContextButtons;

    private Vector2 itemDividerOriginalPosition;
    private Vector2 itemDividerGoalPosition;

    private Vector2 contextMenuOriginalPosition;
    private Vector2 contextMenuPopupGoalPosition;

    private Vector4 itemDescriptionOriginalMargins = Vector4.zero;

    private IEnumerator currentItemDividerAnimation;
    private IEnumerator currentItemDescriptionAnimation;

    private IEnumerator currentDescriptionPrint;
    private IEnumerator currentContextUIAnimation;

    private bool updateItemDescription = false;

    private void Awake() {
        currentContextButtons = new List<ContextButtonUI>();

        inventoryUI.OnSlotSelected += UpdateSelectionUI;
        inventoryUI.OnSlotCombined += ShowCombineResult;
        playerInventoryHandler.OnInventoryStateChanged += EvaluateInventoryState;

        contextMenuOriginalPosition = contextUIParent.localPosition;
        contextMenuPopupGoalPosition = contextMenuOriginalPosition + animationPopoutOffset;

        itemDividerOriginalPosition = selectedItemDivider.localPosition;
        itemDividerGoalPosition = itemDividerOriginalPosition + itemDividerOffset;

        ClearSelectionUI();
    }

    private void OnDestroy() {
        inventoryUI.OnSlotSelected -= UpdateSelectionUI;
        inventoryUI.OnSlotCombined -= ShowCombineResult;
        playerInventoryHandler.OnInventoryStateChanged -= EvaluateInventoryState;
        StopAllCoroutines();
    }

    public void DestroyConfirmationText(){
        DescriptionFinishedPrinting();

        selectedItemDescriptionText.text = destroyConfirmationText.Sentence;
        selectedItemDescriptionText.color = destroyConfirmationText.SentenceColor;
    }

    public void ShowUseResultText(string resultText){
        DescriptionFinishedPrinting();
        selectedItemDescriptionText.text = resultText;
    }

    private void EvaluateInventoryState(object sender, PlayerInventoryHandler.InventoryStateChangedEventArgs e){
        switch (e.inventoryState){
            case InventoryState.Default:
                updateItemDescription = true;
                if(playerInventoryHandler.CurrentInventoryState == InventoryState.ContextUI){
                    HideContextUI();
                    StartQuickDescriptionPrint(inventoryUI.GetSelectedInventoryItem().GetHeldItem());
                }
                break;
            case InventoryState.ContextUI:
                if(playerInventoryHandler.CurrentInventoryState == InventoryState.Inspect){
                    ExitInspectAnimation();
                }
                if(playerInventoryHandler.CurrentInventoryState == InventoryState.Combine || playerInventoryHandler.CurrentInventoryState == InventoryState.Move 
                || playerInventoryHandler.CurrentInventoryState == InventoryState.Assign){
                    UpdateSelectedItemUI();
                }
                
                ShowContextUI();
                break;
            case InventoryState.Combine: 
                ShowCombineUI();
                break;
            case InventoryState.Inspect: 
                HideContextUI();
                EnterInspectAnimation();
                break;
            case InventoryState.Move:
                ShowMoveUI();
                break;
            case InventoryState.Assign: 
                ShowAssignUI();
                break;
        }
    }

    private void EnterInspectAnimation(){
        StopInspectAnimationCoroutines();
        currentItemDividerAnimation = UIAnimator.LerpingAnimationCoroutine(selectedItemDivider, itemDividerGoalPosition, inspectAnimationDuration, false);
        StartCoroutine(currentItemDividerAnimation);
        currentItemDescriptionAnimation = UIAnimator.LerpingTextMarginAnimationCoroutine(selectedItemDescriptionText, inspectItemDescriptionMargins, inspectAnimationDuration);
        StartCoroutine(currentItemDescriptionAnimation);
        StartInspectDescriptionPrint(inventoryUI.GetSelectedItemData());
    }

    private void ExitInspectAnimation(){
        StopInspectAnimationCoroutines();
        currentItemDividerAnimation = UIAnimator.LerpingAnimationCoroutine(selectedItemDivider, itemDividerOriginalPosition, inspectAnimationDuration, false);
        StartCoroutine(currentItemDividerAnimation);
        
        currentItemDescriptionAnimation = UIAnimator.LerpingTextMarginAnimationCoroutine(selectedItemDescriptionText, itemDescriptionOriginalMargins, inspectAnimationDuration);
        StartCoroutine(currentItemDescriptionAnimation);
        
        StartQuickDescriptionPrint(inventoryUI.GetSelectedItemData());
    }

    private void StopInspectAnimationCoroutines(){
        if(currentItemDividerAnimation != null){
            StopCoroutine(currentItemDividerAnimation);
            currentItemDividerAnimation = null;
        }

        if(currentItemDescriptionAnimation != null){
            StopCoroutine(currentItemDescriptionAnimation);
            currentItemDescriptionAnimation = null;
        }
    }

    private void ShowCombineUI(){
        updateItemDescription = false;
        HideContextUI();
        
        DescriptionFinishedPrinting();

        selectedItemDescriptionText.text = "Combine " + inventoryUI.GetSelectedItemData().GetItemName() + " with...";
        selectedItemDescriptionText.color = Color.white;
    }

    private void ShowMoveUI(){
        updateItemDescription = false;
        HideContextUI();
        
        DescriptionFinishedPrinting();

        selectedItemDescriptionText.text = "Move " + inventoryUI.GetSelectedItemData().GetItemName() + " to...";
        selectedItemDescriptionText.color = Color.white;
    }

    private void ShowAssignUI(){
        updateItemDescription = false;
        HideContextUI();

        DescriptionFinishedPrinting();
    
        selectedItemDescriptionText.text = "Assign " + inventoryUI.GetSelectedItemData().GetItemName() + " to...";
        selectedItemDescriptionText.color = Color.white;
    }

    private void ShowCombineResult(object sender, InventoryUI.SlotCombinedEventArgs e){
        selectedItemDescriptionText.text = e.comboResultText;
        selectedItemDescriptionText.color = Color.white;
    }

    private void UpdateSelectedItemUI(){
        selectedItemNameText.text = inventoryUI.GetSelectedItemData().GetItemName();
        selectedItemDescriptionText.color = Color.white;
        selectedItemDescriptionText.text = inventoryUI.GetSelectedItemData().GetItemQuickDescription();
    }

    private void ShowContextUI(){
        if(inventoryUI.GetSelectedInventoryItem() == null) return;

        GenerateContextUIButtons(inventoryUI.GetSelectedInventoryItem(), inventoryUI.GetSelectedItemData());

        UpdateContextParentSpacing();
        
        if(currentContextUIAnimation != null){
            StopCoroutine(currentContextUIAnimation);
            currentContextUIAnimation = null;
        }
        
        currentContextUIAnimation = UIAnimator.LerpingAnimationCoroutine(contextUIParent, contextMenuPopupGoalPosition, animationDuration, false);
        StartCoroutine(currentContextUIAnimation);

        inventoryUI.GetInventoryMenuSelector().SetTarget(currentContextButtons[0].transform);
    }

    private void HideContextUI(){        
        if(currentContextUIAnimation != null){
            StopCoroutine(currentContextUIAnimation);
            currentContextUIAnimation = null;
        }

        currentContextUIAnimation = UIAnimator.LerpingAnimationCoroutine(contextUIParent, contextMenuOriginalPosition, animationDuration, false);
        StartCoroutine(currentContextUIAnimation);

        RemoveContextUIButtons();
    }

    private void GenerateContextUIButtons(InventoryItem inventoryItem, ItemDataSO itemData){
        if(inventoryUI.GetSelectedItemInEquipmentSlot()) AddNewContextUIButton(ContextButtonType.UnEquip, inventoryItem, playerInventoryHandler);
        else if(itemData.GetItemType() == ItemType.Weapon || itemData.GetItemType() == ItemType.Tool) AddNewContextUIButton(ContextButtonType.Equip, inventoryItem, playerInventoryHandler);
        else if(itemData.GetIsUseable()) AddNewContextUIButton(ContextButtonType.Use, inventoryItem, playerInventoryHandler);

        //Keeping this here in case I want to add inspecting back into the game
        // AddNewContextUIButton(ContextButtonType.Inspect, inventoryItem, playerInventoryHandler);
        if(!inventoryUI.GetSelectedItemInEquipmentSlot()){
            AddNewContextUIButton(ContextButtonType.Move, inventoryItem, playerInventoryHandler);
        }
        if(itemData.GetIsUseable()){
            AddNewContextUIButton(ContextButtonType.Assign, inventoryItem, playerInventoryHandler);
        }

        if(itemData.GetIsCombinable()){
            AddNewContextUIButton(ContextButtonType.Combine, inventoryItem, playerInventoryHandler);
        }

        if(itemData.GetIsDestroyable()){
            AddNewContextUIButton(ContextButtonType.Destroy, inventoryItem, playerInventoryHandler);
        }
    }

    private void RemoveContextUIButtons(){
        foreach (ContextButtonUI contextButtonUI in currentContextButtons){
            Destroy(contextButtonUI.gameObject);
        }

        currentContextButtons.Clear();
    }

    private void AddNewContextUIButton(ContextButtonType contextButtonType, InventoryItem selectedItemData, PlayerInventoryHandler playerInventoryHandler){
        ContextButton newContextButton = new ContextButton(contextButtonType, this, selectedItemData, playerInventoryHandler);
        ContextButtonUI newContextButtonUI = Instantiate(contextButtonTemplate, contextButtonParent.transform);
        newContextButtonUI.SetupContextButtonUI(this, newContextButton);
        currentContextButtons.Add(newContextButtonUI);
    }

    private void UpdateContextParentSpacing(){
        var buttonParentSpacing = startingContextButtonSpacing + (currentContextButtons.Count * addContextButtonSpacing);
        contextButtonParent.spacing = buttonParentSpacing;
    }

    private void UpdateSelectionUI(object sender, InventoryUI.SlotSelectedEventArgs e){
        DescriptionFinishedPrinting();
        
        ItemDataSO selectedItemData = e.itemDataSelected;

        if (selectedItemData == null){
            ClearSelectionUI();
            return;
        }

        selectedItemNameText.text = selectedItemData.GetItemName();

        if (!updateItemDescription) return;

        StartQuickDescriptionPrint(selectedItemData);
    }

    private void StartQuickDescriptionPrint(ItemDataSO selectedItemData){
        DescriptionFinishedPrinting();
        if(selectedItemData == null){
            ClearSelectionUI();
            return;
        }

        selectedItemDescriptionText.color = Color.white;
        currentDescriptionPrint = TextPrinter.PrintSentence(selectedItemData.GetItemQuickDescription(), selectedItemDescriptionText, DescriptionFinishedPrinting);
        StartCoroutine(currentDescriptionPrint);
    }

    private void StartInspectDescriptionPrint(ItemDataSO selectedItemData){
        DescriptionFinishedPrinting();
        if(selectedItemData == null){
            ClearSelectionUI();
            return;
        }

        selectedItemDescriptionText.color = Color.white;
        currentDescriptionPrint = TextPrinter.PrintSentence(selectedItemData.GetItemInspectDescription(), selectedItemDescriptionText, DescriptionFinishedPrinting);
        StartCoroutine(currentDescriptionPrint);
    }

    private void DescriptionFinishedPrinting(){
        if(currentDescriptionPrint != null){
            StopCoroutine(currentDescriptionPrint);
            currentDescriptionPrint = null;
        }
    }

    private void ClearSelectionUI(){
        selectedItemNameText.text = "";

        if(playerInventoryHandler.CurrentInventoryState == InventoryState.Combine || playerInventoryHandler.CurrentInventoryState == InventoryState.Move) return;
        
        selectedItemDescriptionText.text = "";
    }
}