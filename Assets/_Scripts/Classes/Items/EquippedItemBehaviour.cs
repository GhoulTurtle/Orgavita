using System.Collections;
using UnityEngine;

public abstract class EquippedItemBehaviour : MonoBehaviour{
    [Header("Item State Variables")]
    [SerializeField] private EquippableItemState defaultItemState;
    [SerializeField] private EquippableItemHolsterType defaultItemHolsterType;
    [SerializeField] private bool playHolsterAnimation;
    [SerializeField] private float holsterAnimationDuration = 0.5f;

    protected EquippableItemState currentItemState;
    protected EquippableItemHolsterType currentHolsterType;
    protected Transform currentHolsterTransform;

    protected InventoryItem inventoryItem;
    protected PlayerInputHandler playerInputHandler;
    
    private Transform activeHolsterTransform;
    private Transform defaultHolsterTransform;

    private IEnumerator currentHolsterAnimation;

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
        if(e.inputActionPhase != UnityEngine.InputSystem.InputActionPhase.Performed || currentHolsterAnimation != null) return;

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
        ChangeItemHolster(defaultHolsterTransform, true);
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

    private void ChangeItemHolster(Transform holsterTransform, bool spawnHolster = false){
        currentHolsterTransform = holsterTransform;

        if(holsterTransform == null) return;

        StopCurrentHolsterAnimation();

        transform.parent = currentHolsterTransform;

        if(playHolsterAnimation && !spawnHolster){            
            StartCoroutine(HolsterAnimationCoroutine(holsterTransform));
        }
        else{
            transform.forward = currentHolsterTransform.forward;
            transform.position = currentHolsterTransform.position;
        }
    }

    private void StopCurrentHolsterAnimation(){
        if(currentHolsterAnimation != null){
            StopCoroutine(currentHolsterAnimation);
            currentHolsterAnimation = null;
        }
    }

    private IEnumerator HolsterAnimationCoroutine(Transform holsterTransform){
        transform.parent = null;
        float elapsedTime = 0f;
    
        Vector3 startPostion = transform.position;
        Vector3 startRotationVector = transform.forward;
        
        while(elapsedTime < holsterAnimationDuration){
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / holsterAnimationDuration);

            transform.position = Vector3.Lerp(startPostion, holsterTransform.position, t);
            transform.forward = Vector3.Lerp(startRotationVector, holsterTransform.forward, t);

            yield return null;
        }

        transform.position = holsterTransform.position;
        transform.forward = holsterTransform.forward;
        transform.parent = currentHolsterTransform;

        currentHolsterAnimation = null;
    }
}