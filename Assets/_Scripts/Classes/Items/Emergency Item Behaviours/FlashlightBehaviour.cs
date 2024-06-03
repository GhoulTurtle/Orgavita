using UnityEngine;

public class FlashlightBehaviour : EquippedItemBehaviour{
   [Header("Required Reference")]
   [SerializeField] private Light lightReference;
   [SerializeField] private FlashlightResourceDataSO flashlightResourceData;

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler);
        lightReference.enabled = !flashlightResourceData.IsEmpty();
    }

    protected override void SubscribeToInputEvents(){
        playerInputHandler.OnEmergencyItemUse += EmergencyItemUseInput;
    }

    protected override void UnsubscribeFromInputEvents(){
        playerInputHandler.OnEmergencyItemUse -= EmergencyItemUseInput;
    }

    public override void EmergencyItemUseInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != UnityEngine.InputSystem.InputActionPhase.Performed || flashlightResourceData.IsEmpty()) return;
        lightReference.enabled = !lightReference.enabled;
        if(lightReference.enabled){
            flashlightResourceData.StartBatteryTime(this);
        }
        else{
            flashlightResourceData.StopBatteryTime(this);
        }
    }

    public override ResourceDataSO GetEquippedItemResourceData(){
        return flashlightResourceData;
    }

    public void BatteryDied(){
        Debug.Log("Battery Died :(");
        lightReference.enabled = false;
    }
}