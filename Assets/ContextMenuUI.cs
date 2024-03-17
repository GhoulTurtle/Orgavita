using System.Collections;
using TMPro;
using UnityEngine;

public class ContextMenuUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI selectedItemNameText;
    [SerializeField] private TextMeshProUGUI selectedItemDescriptionText;
    [SerializeField] private Transform contextButtonParent;
    [SerializeField] private ContextButtonUI contextButtonTemplate;

    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;

    private IEnumerator currentDescriptionPrint;

    private void Awake() {
        inventoryUI.OnSlotSelected += UpdateSelectionUI;
        
        ClearSelectionUI();
    }

    private void OnDestroy() {
        inventoryUI.OnSlotSelected -= UpdateSelectionUI;
        StopAllCoroutines();
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
