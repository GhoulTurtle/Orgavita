using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ContextMenuUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI selectedItemNameText;
    [SerializeField] private TextMeshProUGUI selectedItemDescriptionText;
    [SerializeField] private Transform contextUIParent;
    [SerializeField] private Transform contextButtonParent;
    [SerializeField] private ContextButtonUI contextButtonTemplate;

    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("UI Animation References")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Vector2 animationPopoutOffset;

    private Vector2 contextMenuOriginalPosition;
    private Vector2 contextMenuPopupGoalPosition;

    private IEnumerator currentDescriptionPrint;
    private IEnumerator currentContextUIAnimation;

    private void Awake() {
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

    private void ShowContextUI(object sender, EventArgs e){
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
