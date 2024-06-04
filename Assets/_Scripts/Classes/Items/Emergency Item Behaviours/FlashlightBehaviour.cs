using System;
using System.Collections;
using UnityEngine;

public class FlashlightBehaviour : EquippedItemBehaviour{
   [Header("Required Reference")]
   [SerializeField] private Light lightReference;
   [SerializeField] private FlashlightResourceDataSO flashlightResourceData;

    public EventHandler OnFlashlightTurnedOn;
    public EventHandler OnFlashlightTurnedOff;

    public EventHandler OnCurrentBatteryTimeChanged;

    private void OnDestroy() {
        flashlightResourceData.OnBatteryRestored -= (sender, e) => OnCurrentBatteryTimeChanged?.Invoke(this, EventArgs.Empty);
        StopAllCoroutines();
        UnsubscribeFromInputEvents();
    }

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler);
        
        flashlightResourceData.OnBatteryRestored += (sender, e) => OnCurrentBatteryTimeChanged?.Invoke(this, EventArgs.Empty);

        lightReference.enabled = !flashlightResourceData.IsEmpty();
        if(lightReference.enabled){
            StartBatteryTimer();
            OnFlashlightTurnedOn?.Invoke(this, EventArgs.Empty);
        }
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
            StartBatteryTimer();
            OnFlashlightTurnedOn?.Invoke(this, EventArgs.Empty);
        }
        else{
            StopAllCoroutines();
            OnFlashlightTurnedOff?.Invoke(this, EventArgs.Empty);
        }
    }

    public override ResourceDataSO GetEquippedItemResourceData(){
        return flashlightResourceData;
    }

    public void BatteryDied(){
        Debug.Log("Battery Died :(");
        lightReference.enabled = false;
    }

    public void StartBatteryTimer(){
        StartCoroutine(BatteryTimerCoroutine());
    }

    private IEnumerator BatteryTimerCoroutine(){
        while(flashlightResourceData.GetCurrentBatteryTimeInSeconds() > 0){
            yield return new WaitForSeconds(1f);
            float currentTime = flashlightResourceData.GetCurrentBatteryTimeInSeconds();
            flashlightResourceData.SetCurrentBatteryTimeInSeconds(currentTime - 1f);
            OnCurrentBatteryTimeChanged?.Invoke(this, EventArgs.Empty);
        }
        
        BatteryDied();
        flashlightResourceData.RemoveItem();
    }
}