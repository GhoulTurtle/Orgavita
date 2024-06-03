using UnityEngine;

public abstract class EquippedItemBehaviour : MonoBehaviour{
    [Header("Item State Variables")]
    [SerializeField] private EquippableItemState defaultItemState;
    [SerializeField] private EquippableItemHolsterType defaultItemHolsterType;

    protected EquippableItemState currentItemState;
    protected EquippableItemHolsterType currentHolsterType;
    protected Transform currentHolsterTransform;

    protected InventoryItem inventoryItem;
    protected PlayerInputHandler playerInputHandler;
    
    private Transform activeHolsterTransform;
    private Transform defaultHolsterTransform;

    private void OnDestroy() {
        StopAllCoroutines();
        UnsubscribeFromInputEvents();
    }

    public virtual void SaveData(){
        
    }

    public virtual void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler){
        inventoryItem = _inventoryItem;
        playerInputHandler = _playerInputHandler;
    }

    public void SetupPlayerItemHolster(Transform _defaultHolsterTransform, Transform _activeHolsterTransform){
        defaultHolsterTransform = _defaultHolsterTransform;
        activeHolsterTransform = _activeHolsterTransform;
    }

    public virtual void WeaponUseInput(object sender, InputEventArgs e){

    }

    public virtual void WeaponAltUseInput(object sender, InputEventArgs e){

    }

    public virtual void EmergencyItemUseInput(object sender, InputEventArgs e){

    }

    public virtual void HolsterWeaponInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != UnityEngine.InputSystem.InputActionPhase.Performed) return;

        switch (currentItemState){
            case EquippableItemState.Active: ChangeItemState(EquippableItemState.Holstered);
                break;
            case EquippableItemState.Holstered: ChangeItemState(EquippableItemState.Active);
                break;
        }
    }

    public virtual void ReloadInput(object sender, InputEventArgs e){

    }

    public virtual void InspectInput(object sender, InputEventArgs e){

    }

    public virtual void UpdateControlOnItemStateChange(){
        switch (currentItemState){
            case EquippableItemState.Active: SubscribeToInputEvents();
                break;
            case EquippableItemState.Holstered: UnsubscribeFromInputEvents();
                break;
            case EquippableItemState.Used: UnsubscribeFromInputEvents();
                break;
            case EquippableItemState.Passive: SubscribeToInputEvents();
                break;
        }
    }

    protected virtual void SubscribeToInputEvents(){

    }

    protected virtual void UnsubscribeFromInputEvents(){

    }

    public void TriggerDefaultState(){
        currentItemState = defaultItemState;
        ChangeItemHolster(defaultHolsterTransform);
        UpdateControlOnItemStateChange();
    }

    public void ChangeItemState(EquippableItemState newState){
        if(newState == currentItemState) return;

        currentItemState = newState;

        switch (newState){
            case EquippableItemState.None: ChangeItemHolster(null);
                break;
            case EquippableItemState.Active: ChangeItemHolster(activeHolsterTransform);
                break;
            case EquippableItemState.Holstered: ChangeItemHolster(defaultHolsterTransform);
                break;
            case EquippableItemState.Used: ChangeItemHolster(null);
                break;
            case EquippableItemState.Passive: ChangeItemHolster(defaultHolsterTransform);
                break;
        }

        UpdateControlOnItemStateChange();
    }

    public EquippableItemHolsterType GetPlayerItemHolsterType(){
        return defaultItemHolsterType;
    }

    public EquippableItemState GetPlayerItemState(){
        return defaultItemState;      
    }

    public virtual ResourceDataSO GetEquippedItemResourceData(){
        return null;
    }

    private void ChangeItemHolster(Transform holsterTransform){
        currentHolsterTransform = holsterTransform;

        if(holsterTransform != null){
            transform.parent = currentHolsterTransform;
            transform.forward = currentHolsterTransform.forward;
            transform.localPosition = Vector3.zero;
        }
    }
}