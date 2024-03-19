using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryHandler : MonoBehaviour{
    [Header("Scriptable Object Required References")]
    [SerializeField] private PlayerInventorySO currentInventory;
    [SerializeField] private PlayerInventoryRecipeListSO currentInventoryRecipeList;

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

    private void Awake() {
        TryGetComponent(out playerInputHandler);
    }

    private void Start() {
        SetupInventory();
    }

    private void OnDestroy() {
        if(playerInputHandler == null) return;

        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
    }

    public void InventoryInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;

        UpdateInventoryState(InventoryState.Default);
    }

    public void SetupInventory(){
        OnSetupInventory?.Invoke(this, new SetupInventoryEventArgs(currentInventory));
    }

    public void UpdateInventoryState(InventoryState inventoryState){
        if(currentInventoryState == inventoryState) return;

        switch (inventoryState){
            case InventoryState.Closed: ClosedInventoryState();
                break;
            case InventoryState.Default: DefaultInventoryState();
                break;
            case InventoryState.ContextUI: ContextUIInventoryState();
                break;
            case InventoryState.Combine: CombineInventoryState();
                break;
            case InventoryState.Inspect: InspectInventoryState();
                break;
        }

        currentInventoryState = inventoryState;

        OnInventoryStateChanged?.Invoke(this, new InventoryStateChangedEventArgs(currentInventoryState));
    }

    private void InspectInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput += ReturnToContextUIInput;
    }

    private void CombineInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput += ReturnToContextUIInput;
    }

    private void ContextUIInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        playerInputHandler.OnCancelInput += ReturnToInventoryUIInput;
    }

    private void DefaultInventoryState(){
        GameManager.UpdateGameState(GameState.UI);
        playerInputHandler.OnCancelInput -= ReturnToInventoryUIInput;
        playerInputHandler.OnCancelInput += ExitInventoryUIInput;
    }

    private void ClosedInventoryState(){
        playerInputHandler.OnCancelInput -= ExitInventoryUIInput;
        GameManager.UpdateGameState(GameState.Game);
    }

    private void ReturnToContextUIInput(object sender, PlayerInputHandler.InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        UpdateInventoryState(InventoryState.ContextUI);
    }

    private void ReturnToInventoryUIInput(object sender, PlayerInputHandler.InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        UpdateInventoryState(InventoryState.Default);
    }

    private void ExitInventoryUIInput(object sender, PlayerInputHandler.InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        UpdateInventoryState(InventoryState.Closed);
    }

    public PlayerInventorySO GetInventory(){
        return currentInventory;
    }

    public void SetInventory(PlayerInventorySO playerInventorySO){
        currentInventory = playerInventorySO;
    }

    public PlayerInventoryRecipeListSO GetRecipeList(){
        return currentInventoryRecipeList;
    }

    public void SetRecipeList(PlayerInventoryRecipeListSO inventoryRecipeListSO){
        currentInventoryRecipeList = inventoryRecipeListSO;
    }
}