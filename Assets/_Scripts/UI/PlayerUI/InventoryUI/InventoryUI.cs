using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Transform inventoryScreen;
    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private Transform itemScrollContent;
    [SerializeField] private ItemSlotUI itemSlotTemplate;
    [SerializeField] private ItemUI itemUITemplate;
    [SerializeField] private ItemUI equippedItemUI;
    [SerializeField] private ItemUI emergencyItemUI;

    [Header("UI Animation Variables")]
    [SerializeField] private Vector2 closeScale;
    [SerializeField] private float animationDuration = 0.5f;
    private Vector3 openScale = Vector3.one;
    private IEnumerator currentInventoryUIAnimation;

    [Header("Required References")]
    [SerializeField] private PlayerInventoryHandler playerInventoryHandler;
    [SerializeField] private MenuSelector inventoryUISelector;

    public EventHandler<SlotSelectedEventArgs> OnSlotSelected;
    public class SlotSelectedEventArgs : EventArgs{
        public InventoryItem inventoryItemSelected;
        public ItemDataSO itemDataSelected;

        public SlotSelectedEventArgs(InventoryItem _inventoryItemSelected, ItemDataSO _itemDataSelected){
            itemDataSelected = _itemDataSelected;
            inventoryItemSelected = _inventoryItemSelected;
        }
    }

    public EventHandler<SlotClickedEventArgs> OnCurrentSlotClicked;
    public class SlotClickedEventArgs : EventArgs{
        public InventoryItem inventoryItemClicked;
        public ItemDataSO itemDataClicked;
        public PlayerInventoryHandler playerInventoryHandler;
        public bool inEquipmentSlot = false;

        public SlotClickedEventArgs(InventoryItem _inventoryItemClicked, ItemDataSO _itemDataClicked, PlayerInventoryHandler _playerInventoryHandler, bool _inEquipmentSlot){
            inventoryItemClicked = _inventoryItemClicked;
            itemDataClicked = _itemDataClicked;
            playerInventoryHandler = _playerInventoryHandler;
            inEquipmentSlot = _inEquipmentSlot;
        }
    }
    public EventHandler OnExitContextUI;

    private ItemUI currentSelectedItemUI;

    private List<ItemUI> currentItemUI;
    private List<ItemSlotUI> currentItemSlotUI;

    private void Awake() {
        currentItemUI = new List<ItemUI>();
        currentItemSlotUI = new List<ItemSlotUI>();

        inventoryUISelector.DisableSelector();
        inventoryScreen.gameObject.SetActive(false);

        if(playerInventoryHandler == null) return;

        playerInventoryHandler.OnInventoryStateChanged += EvaluateInventoryState;
        
        playerInventoryHandler.OnSetupInventory += SetupInventoryUI;
    }

    private void OnDestroy() {
        StopAllCoroutines();

        if(playerInventoryHandler == null) return;
        playerInventoryHandler.OnInventoryStateChanged -= EvaluateInventoryState;
        playerInventoryHandler.OnSetupInventory -= SetupInventoryUI;
        playerInventoryHandler.GetInventory().OnMaxInventoryIncreased -= (sender, e) => AddNewInventoryUI(e.newSlotsAdded);
    }

    public void ClickedSelectedItemUI(){
        playerInventoryHandler.UpdateInventoryState(InventoryState.ContextUI);

        var selectedInventoryItem = currentSelectedItemUI.GetInventoryItem();
        OnCurrentSlotClicked?.Invoke(this, new SlotClickedEventArgs(selectedInventoryItem, selectedInventoryItem.GetHeldItem(), playerInventoryHandler, currentSelectedItemUI.GetIsEquipmentSlot()));
    }

    public void SelectItemUI(ItemUI itemUI){
        currentSelectedItemUI = itemUI;
        var itemUIInventoryItem = itemUI.GetInventoryItem();
        OnSlotSelected?.Invoke(this, new SlotSelectedEventArgs(itemUIInventoryItem, itemUIInventoryItem.GetHeldItem()));
    }

    private void SetupInventoryUI(object sender, PlayerInventoryHandler.SetupInventoryEventArgs e){
        equippedItemUI.SetupItemUI(this, e.playerInventorySO.GetEquippedInventoryItem(), true);
        emergencyItemUI.SetupItemUI(this, e.playerInventorySO.GetEmergencyInventoryItem(), true);

        playerInventoryHandler.GetInventory().OnMaxInventoryIncreased += (sender, e) => AddNewInventoryUI(e.newSlotsAdded);

        List<InventoryItem> inventory = e.playerInventorySO.GetCurrentInventory();
        
        AddNewInventoryUI(inventory);
        SetupInventorySelector();
    }

    private void SetupInventorySelector(){
        inventoryUISelector.SetCursorStartingSelection(currentItemUI[0].transform);
    }

    private void AddNewInventoryUI(List<InventoryItem> newInventoryItemsAdded){
        foreach (InventoryItem item in newInventoryItemsAdded){
            var newItemUI = Instantiate(itemUITemplate, itemScrollContent);
            newItemUI.SetupItemUI(this, item);
            currentItemUI.Add(newItemUI);

            var newItemSlotUI = Instantiate(itemSlotTemplate, itemSlotParent);
            newItemSlotUI.SetupItemSlotUI(this, item);
            currentItemSlotUI.Add(newItemSlotUI);
        }
    }

    private void EvaluateInventoryState(object sender, PlayerInventoryHandler.InventoryStateChangedEventArgs e){
        switch (e.inventoryState){
            case InventoryState.Closed: HideInventoryScreen();
                break;
            case InventoryState.Default:
                if(playerInventoryHandler.CurrentInventoryState == InventoryState.Closed){
                    ShowInventoryScreen();
                }

                if(playerInventoryHandler.CurrentInventoryState == InventoryState.ContextUI){
                    EnableItemUIInteractivity();
                    OnExitContextUI?.Invoke(this, EventArgs.Empty);
                }
                break;
            case InventoryState.ContextUI: DisableItemUIInteractivity();
                break;
            case InventoryState.Combine: 
                break;
            case InventoryState.Inspect:
                break;
        }
    }

    private void ShowInventoryScreen(){        
        inventoryUISelector.EnableSelector();
        
        if(currentInventoryUIAnimation != null){
            StopCoroutine(currentInventoryUIAnimation);
            currentInventoryUIAnimation = null;
        }

        inventoryScreen.gameObject.SetActive(true);

        inventoryScreen.localScale = closeScale;

        currentInventoryUIAnimation = UIAnimator.UIStretchAnimationCoroutine(inventoryScreen, openScale, animationDuration, false);        

        StartCoroutine(currentInventoryUIAnimation);

        if(currentSelectedItemUI != null){
            var itemUIInventoryItem = currentSelectedItemUI.GetInventoryItem();
            OnSlotSelected?.Invoke(this, new SlotSelectedEventArgs(itemUIInventoryItem, itemUIInventoryItem.GetHeldItem()));
        }
    }

    private void HideInventoryScreen(){
        inventoryUISelector.DisableSelector();

        if(currentInventoryUIAnimation != null){
            StopCoroutine(currentInventoryUIAnimation);
            currentInventoryUIAnimation = null;
        }

        inventoryScreen.localScale = openScale;

        currentInventoryUIAnimation = UIAnimator.UIStretchAnimationCoroutine(inventoryScreen, closeScale, animationDuration, true);        

        StartCoroutine(currentInventoryUIAnimation);
    }

    private void DisableItemUIInteractivity(){
        foreach (ItemUI itemUI in currentItemUI){
            itemUI.DisableInteractivity();
        }

        equippedItemUI.DisableInteractivity();
        emergencyItemUI.DisableInteractivity();
    }

    private void EnableItemUIInteractivity(){
        foreach (ItemUI itemUI in currentItemUI){
            itemUI.EnableInteractivity();
        }

        equippedItemUI.EnableInteractivity();
        emergencyItemUI.DisableInteractivity();
    }

    public void MoveSelectorBackToSelectedItemUI(){
        inventoryUISelector.SetTarget(currentSelectedItemUI.transform);
    }

    public MenuSelector GetInventoryMenuSelector(){
        return inventoryUISelector;
    }
}