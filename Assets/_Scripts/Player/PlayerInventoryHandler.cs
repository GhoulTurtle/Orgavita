using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryHandler : MonoBehaviour{
    [Header("Input Variables")]
    [SerializeField] private float inputLockoutTime = 0.5f;

    [Header("Scriptable Object Required References")]
    [SerializeField] private PlayerInventorySO playerInventory;
    [SerializeField] private PlayerInventoryRecipeListSO playerInventoryRecipeList;

    public InventoryState CurrentInventoryState => currentInventoryState;

    private InventoryState currentInventoryState = InventoryState.Closed;

    public EventHandler<InventoryStateChangedEventArgs> OnInventoryStateChanged;
    public class InventoryStateChangedEventArgs : EventArgs{
        public InventoryState inventoryState;
        public InventoryStateChangedEventArgs(InventoryState _inventoryState){
            inventoryState = _inventoryState;
        }
    }

    public EventHandler<SetupInventoryEventArgs> OnSetupInventory;
    public class SetupInventoryEventArgs : EventArgs{
        public PlayerInventorySO playerInventorySO;
        public SetupInventoryEventArgs(PlayerInventorySO _playerInventorySO){
            playerInventorySO = _playerInventorySO;
        }
    }

    private PlayerInputHandler playerInputHandler;
    private Vector2 navigationInput;

    private ItemLevelMechanic currentItemLevelMechanic;
    
    private bool inputValid = true;

    private void Awake() {
        TryGetComponent(out playerInputHandler);
    }

    private void Start() {
        SetupInventory();
    }

    private void OnDestroy() {
        if(playerInputHandler == null) return;

        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        StopAllCoroutines();
    }

    public void InventoryInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;

        UpdateInventoryState(InventoryState.Default);
    }

    public void SetupInventory(){
        OnSetupInventory?.Invoke(this, new SetupInventoryEventArgs(playerInventory));
    }

    public void UpdateInventoryState(InventoryState inventoryState){
        if(currentInventoryState == inventoryState) return;

        StartCoroutine(InputLockoutCoroutine());

        switch (inventoryState){
            case InventoryState.Closed: ClosedInventoryState();
                break;
            case InventoryState.Default: DefaultInventoryState();
                break;
            case InventoryState.ContextUI: ContextUIInventoryState();
                break;
            case InventoryState.Move: MoveInventoryState();
                break;
            case InventoryState.Combine: CombineInventoryState();
                break;
            case InventoryState.Inspect: InspectInventoryState();
                break;
            case InventoryState.Assign: AssignInventoryState();
                break;
        }

        OnInventoryStateChanged?.Invoke(this, new InventoryStateChangedEventArgs(inventoryState));

        currentInventoryState = inventoryState;
    }

    public void AddCurrentLevelMechanic(ItemLevelMechanic itemLevelMechanic){
        if(currentItemLevelMechanic != null){
            RemoveCurrentLevelMechanic(currentItemLevelMechanic);
        }
        currentItemLevelMechanic = itemLevelMechanic;
    }

    public void RemoveCurrentLevelMechanic(ItemLevelMechanic itemLevelMechanic){
        if(currentItemLevelMechanic == null || currentItemLevelMechanic != itemLevelMechanic) return;
        currentItemLevelMechanic = null;
    }

    public ItemLevelMechanic GetCurrentLevelMechanic(){
        return currentItemLevelMechanic;
    }

    public string AttemptItemCombine(InventoryItem inventoryItem1, InventoryItem inventoryItem2){
        return playerInventory.AttemptItemCombination(inventoryItem1, inventoryItem2, playerInventoryRecipeList);
    }

    private IEnumerator InputLockoutCoroutine(){
        inputValid = false;
        yield return new WaitForSeconds(inputLockoutTime);
        inputValid = true;
    }

    private void InspectInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput -= ReturnDefaultInventoryUIInput;
        playerInputHandler.OnCancelInput += ReturnToContextUIInput;
        playerInputHandler.OnNavigateInput += NavigationInput;
    }

    private void AssignInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput -= ReturnDefaultInventoryUIInput;
        playerInputHandler.OnCancelInput += ReturnToContextUIInput;
    }

    private void NavigationInput(object sender, InputEventArgs e){
        navigationInput = e.callbackContext.ReadValue<Vector2>();
    }

    private void CombineInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput -= ReturnDefaultInventoryUIInput;
        playerInputHandler.OnCancelInput += ReturnToContextUIInput;
    }

    private void ContextUIInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput -= ReturnToContextUIInput;
        playerInputHandler.OnNavigateInput -= NavigationInput;
        navigationInput = Vector2.zero;
        playerInputHandler.OnCancelInput += ReturnDefaultInventoryUIInput;
    }

    private void MoveInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput -= ReturnDefaultInventoryUIInput;
        playerInputHandler.OnCancelInput += ReturnToContextUIInput;
    }

    private void DefaultInventoryState(){
        GameManager.UpdateGameState(GameState.UI);
        playerInputHandler.OnCancelInput -= ReturnDefaultInventoryUIInput;
        playerInputHandler.OnCancelInput -= ReturnToContextUIInput;
        playerInputHandler.OnCancelInput += ExitInventoryUIInput;
    }

    private void ClosedInventoryState(){
        playerInputHandler.OnCancelInput -= ReturnDefaultInventoryUIInput;
        playerInputHandler.OnCancelInput -= ReturnToContextUIInput;
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        GameManager.UpdateGameState(GameState.Game);
    }

    private void ReturnToContextUIInput(object sender, InputEventArgs e){
        if(!inputValid) return;

        if(e.inputActionPhase != InputActionPhase.Started) return;
        UpdateInventoryState(InventoryState.ContextUI);
    }

    private void ReturnDefaultInventoryUIInput(object sender, InputEventArgs e){
        if(!inputValid) return;

        if(e.inputActionPhase != InputActionPhase.Started) return;
        UpdateInventoryState(InventoryState.Default);
    }

    private void ExitInventoryUIInput(object sender, InputEventArgs e){
        if(!inputValid) return;

        if(e.inputActionPhase != InputActionPhase.Started) return;
        UpdateInventoryState(InventoryState.Closed);
    }

    public PlayerInventorySO GetInventory(){
        return playerInventory;
    }

    public void SetInventory(PlayerInventorySO playerInventorySO){
        playerInventory = playerInventorySO;
    }

    public PlayerInventoryRecipeListSO GetRecipeList(){
        return playerInventoryRecipeList;
    }

    public Vector2 GetNavigationInput(){
        return navigationInput;
    }
    
    public void SetRecipeList(PlayerInventoryRecipeListSO inventoryRecipeListSO){
        playerInventoryRecipeList = inventoryRecipeListSO;
    }   
}