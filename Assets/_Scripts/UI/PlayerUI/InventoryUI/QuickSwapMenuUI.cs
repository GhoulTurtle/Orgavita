using UnityEngine;

public class QuickSwapMenuUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private MenuSelector inventorySelector;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("UI References")]
    [SerializeField] private QuickSwapUI[] quickSwapUIArray = new QuickSwapUI[4];

    private void Awake() {
        SetupQuickSwapUI();
        DisableInteractivity();

        if(playerInventoryHandler != null){
            playerInventoryHandler.OnInventoryStateChanged += EvaluateInventoryState;
        }
    }

    private void EvaluateInventoryState(object sender, PlayerInventoryHandler.InventoryStateChangedEventArgs e){
        if(e.inventoryState == InventoryState.Assign){
            EnableInteractivity();

            inventorySelector.SetTarget(quickSwapUIArray[0].GetQuickSwapButton().transform);
        }
        else if(playerInventoryHandler.CurrentInventoryState == InventoryState.Assign){
            DisableInteractivity();
        }
    }

    public void SelectedSlot(QuickSwapUI selectedSlot){
        ItemDataSO currentSelectedItem = inventoryUI.GetSelectedItemData();

        selectedSlot.UpdateSlotUI(currentSelectedItem.GetItemSprite(), currentSelectedItem.GetItemName());
        playerInventoryHandler.UpdateInventoryState(InventoryState.ContextUI);
    }

    private void SetupQuickSwapUI(){
        for (int i = 0; i < quickSwapUIArray.Length; i++){
            quickSwapUIArray[i].SetupQuickSwapUI(this, emptySlotSprite);
            quickSwapUIArray[i].ClearSlotUI();
        }
    }

    private void EnableInteractivity(){
        for (int i = 0; i < quickSwapUIArray.Length; i++){
            quickSwapUIArray[i].EnableUIInteractivity();
        }
    }

    private void DisableInteractivity(){
        for (int i = 0; i < quickSwapUIArray.Length; i++){
            quickSwapUIArray[i].DisableUIInteractivity();
        }
    }
}
