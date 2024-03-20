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

    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Context Menu Variables")]
    [SerializeField] private float startingContextButtonSpacing = -100;
    [SerializeField] private float addContextButtonSpacing = 25;

    [Header("UI Animation References")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Vector2 animationPopoutOffset;

    private List<ContextButtonUI> currentContextButtons;

    private Vector2 contextMenuOriginalPosition;
    private Vector2 contextMenuPopupGoalPosition;

    private IEnumerator currentDescriptionPrint;
    private IEnumerator currentContextUIAnimation;

    private void Awake() {
        currentContextButtons = new List<ContextButtonUI>();

        inventoryUI.OnSlotSelected += UpdateSelectionUI;
        inventoryUI.OnCurrentSlotClicked += ShowContextUI;
        inventoryUI.OnExitContextUI += HideContextUI;
        
        contextMenuOriginalPosition = contextUIParent.localPosition;
        contextMenuPopupGoalPosition = contextMenuOriginalPosition + animationPopoutOffset;

        ClearSelectionUI();
    }

    private void OnDestroy() {
        inventoryUI.OnSlotSelected -= UpdateSelectionUI;
        inventoryUI.OnCurrentSlotClicked -= ShowContextUI;
        inventoryUI.OnExitContextUI -= HideContextUI;
        StopAllCoroutines();
    }

    private void ShowContextUI(object sender, InventoryUI.SlotClickedEventArgs e){
        GenerateContextUIButtons(e);

        UpdateContextParentSpacing();

        inventoryUI.GetInventoryMenuSelector().SetTarget(currentContextButtons[0].transform);
        
        if(currentContextUIAnimation != null){
            StopCoroutine(currentContextUIAnimation);
            currentContextUIAnimation = null;
        }
        
        currentContextUIAnimation = UIAnimator.UILerpingAnimationCoroutine(contextUIParent, contextMenuPopupGoalPosition, animationDuration, false);
        StartCoroutine(currentContextUIAnimation);
    }

    private void HideContextUI(object sender, EventArgs e){        
        if(currentContextUIAnimation != null){
            StopCoroutine(currentContextUIAnimation);
            currentContextUIAnimation = null;
        }

        currentContextUIAnimation = UIAnimator.UILerpingAnimationCoroutine(contextUIParent, contextMenuOriginalPosition, animationDuration, false);
        StartCoroutine(currentContextUIAnimation);

        RemoveContextUIButtons();
        inventoryUI.MoveSelectorBackToSelectedItemUI();
    }

    private void GenerateContextUIButtons(InventoryUI.SlotClickedEventArgs e){
        var selectedItemData = e.itemDataClicked;
        if(e.inEquipmentSlot) AddNewContextUIButton(ContextButtonType.UnEquip, e.inventoryItemClicked, e.playerInventoryHandler);
        else if(selectedItemData.GetItemType() == ItemType.Weapon || selectedItemData.GetItemType() == ItemType.Emergency_Item) AddNewContextUIButton(ContextButtonType.Equip, e.inventoryItemClicked, e.playerInventoryHandler);
        else AddNewContextUIButton(ContextButtonType.Use, e.inventoryItemClicked, e.playerInventoryHandler);

        AddNewContextUIButton(ContextButtonType.Inspect, e.inventoryItemClicked, e.playerInventoryHandler);

        if(selectedItemData.GetIsCombinable()){
            AddNewContextUIButton(ContextButtonType.Combine, e.inventoryItemClicked, e.playerInventoryHandler);
        }

        if(selectedItemData.GetIsDestroyable()){
            AddNewContextUIButton(ContextButtonType.Destroy, e.inventoryItemClicked, e.playerInventoryHandler);
        }
    }

    private void RemoveContextUIButtons(){
        foreach (ContextButtonUI contextButtonUI in currentContextButtons){
            Destroy(contextButtonUI.gameObject);
        }

        currentContextButtons.Clear();
    }

    private void AddNewContextUIButton(ContextButtonType contextButtonType, InventoryItem selectedItemData, PlayerInventoryHandler playerInventoryHandler){
        ContextButton newContextButton = new ContextButton(contextButtonType, selectedItemData, playerInventoryHandler);

        ContextButtonUI newContextButtonUI = Instantiate(contextButtonTemplate, contextButtonParent.transform);
        newContextButtonUI.SetupContextButtonUI(this, newContextButton);
        currentContextButtons.Add(newContextButtonUI);
    }

    private void UpdateContextParentSpacing(){
        var buttonParentSpacing = startingContextButtonSpacing + (currentContextButtons.Count * addContextButtonSpacing);
        contextButtonParent.spacing = buttonParentSpacing;
    }

    private void UpdateSelectionUI(object sender, InventoryUI.SlotSelectedEventArgs e){
        if(currentDescriptionPrint != null){
            DescriptionFinishedPrinting();
        }

        ItemDataSO selectedItemData = e.itemDataSelected;
        
        if(selectedItemData == null){
            ClearSelectionUI();
            return;
        }

        selectedItemNameText.text = selectedItemData.GetItemName();
        currentDescriptionPrint = TextPrinter.PrintSentence(selectedItemData.GetItemQuickDescription(), selectedItemDescriptionText, DescriptionFinishedPrinting);
        StartCoroutine(currentDescriptionPrint);
    }

    private void DescriptionFinishedPrinting(){
        StopCoroutine(currentDescriptionPrint);
        currentDescriptionPrint = null;
    }

    private void ClearSelectionUI(){
        selectedItemNameText.text = "";
        selectedItemDescriptionText.text = "";
    }
}
