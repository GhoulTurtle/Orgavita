using System.Collections.Generic;
using UnityEngine;

public class QuickSelectMenuUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerQuickSelectHandler playerQuickSelectHandler;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private MenuSelector inventorySelector;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("UI References")]
    [SerializeField] private List<QuickSelectUI> quickSelectUIList = new();

    private void Awake() {
        DisableInteractivity();

        if(playerInventoryHandler != null){
            playerInventoryHandler.OnInventoryStateChanged += EvaluateInventoryState;
        }
    }

    private void Start() {
        SetupQuickSelectUI();
    }

    private void EvaluateInventoryState(object sender, PlayerInventoryHandler.InventoryStateChangedEventArgs e){
        if(e.inventoryState == InventoryState.Assign){
            EnableInteractivity();

            inventorySelector.SetTarget(quickSelectUIList[0].GetQuickSwapButton().transform);
        }
        else if(playerInventoryHandler.CurrentInventoryState == InventoryState.Assign){
            DisableInteractivity();
        }
    }

    public void SelectedSlot(QuickSelectUI selectedSlot){
        if(!quickSelectUIList.Contains(selectedSlot)){
            Debug.Log(selectedSlot + " is not currently assigned to the quick select ui list.");
            return;
        }

        int slotIndex = quickSelectUIList.IndexOf(selectedSlot);

        playerQuickSelectHandler.AttemptQuickSelectAssignment(slotIndex);
    }

    private void SetupQuickSelectUI(){
        for (int i = 0; i < quickSelectUIList.Count; i++){
            quickSelectUIList[i].SetupQuickSelectUI(this, playerQuickSelectHandler.GetQuickSelectDataSO(), i, emptySlotSprite);
        }
    }

    private void EnableInteractivity(){
        for (int i = 0; i < quickSelectUIList.Count; i++){
            quickSelectUIList[i].EnableUIInteractivity();
        }
    }

    private void DisableInteractivity(){
        for (int i = 0; i < quickSelectUIList.Count; i++){
            quickSelectUIList[i].DisableUIInteractivity();
        }
    }
}
