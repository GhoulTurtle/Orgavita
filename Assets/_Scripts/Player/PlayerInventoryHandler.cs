using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryHandler : MonoBehaviour{
    [Header("Scriptable Object Required References")]
    [SerializeField] private PlayerInventorySO currentInventory;
    [SerializeField] private PlayerInventoryRecipeListSO currentInventoryRecipeList;

    public EventHandler<SetupInventoryEventArgs> OnSetupInventory;
    public class SetupInventoryEventArgs : EventArgs{
        public PlayerInventorySO playerInventorySO;
        public SetupInventoryEventArgs(PlayerInventorySO _playerInventorySO){
            playerInventorySO = _playerInventorySO;
        }
    }

    public EventHandler OnShowInventory;
    public EventHandler OnHideInventory;

    private PlayerInputHandler playerInputHandler;

    private void Awake() {
        TryGetComponent(out playerInputHandler);
    }

    private void Start() {
        SetupInventory();
    }

    private void OnDestroy() {
        if(playerInputHandler == null) return;

        playerInputHandler.OnCancelInput -= ExitInventoryInput;
    }

    public void InventoryInput(InputAction.CallbackContext context){
        if(context.phase != InputActionPhase.Performed) return;

        SubscribeToUIInput();
        GameManager.UpdateGameState(GameState.UI);
        OnShowInventory?.Invoke(this, EventArgs.Empty);
    }

    public void SetupInventory(){
        OnSetupInventory?.Invoke(this, new SetupInventoryEventArgs(currentInventory));
    }

    private void ExitInventoryInput(object sender, PlayerInputHandler.InputEventArgs e){
        UnSubscribeFromUIInput();
        GameManager.UpdateGameState(GameState.Game);
        OnHideInventory?.Invoke(this, EventArgs.Empty);
    }

    private void SubscribeToUIInput(){
        playerInputHandler.OnCancelInput += ExitInventoryInput;
    }

    private void UnSubscribeFromUIInput(){
        playerInputHandler.OnCancelInput -= ExitInventoryInput;
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
