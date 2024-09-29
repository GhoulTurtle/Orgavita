using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FlashlightBehaviour : EquippedItemBehaviour{
   [Header("Required Reference")]
   [SerializeField] private Light lightReference;
   [SerializeField] private FlashlightResourceDataSO flashlightResourceData;

    [Header("Flashlight Variables")]
    [SerializeField] private float fullyChargedIntensity = 30f;
    [SerializeField] private float minChargedIntensity = 10f;
    [SerializeField, Range(0f, 1f)] private float lowBatteryEffectsStartAmount = 0.8f; 
    [SerializeField] private AnimationCurve flashlightIntensityFalloff;
    [SerializeField] private AnimationCurve flashlightFlickeringCurve;
    [SerializeField] private float baseFlickerChance = 7f;
    [SerializeField] private float flashlightMinFlickerTime = 0.2f;
    [SerializeField] private float flashlightMaxFlickerTime = 0.2f;

    public EventHandler OnFlashlightTurnedOnEvent;
    public EventHandler OnFlashlightTurnedOffEvent;

    public EventHandler OnCurrentBatteryTimeChanged;

    public UnityEvent OnFlashlightTurnedOn;
    public UnityEvent OnFlashlightTurnedOff;
    public UnityEvent OnFlashlightInteractWhileBatteryDead;
    public UnityEvent OnFlashlightReloaded;
    public UnityEvent OnFlashlightFlickered;

    private IEnumerator flashlightFlickerCoroutine;

    public override void SaveData(){
        flashlightResourceData.OnBatteryRestored -= BatteryRestored;
        StopAllCoroutines();
        UnsubscribeFromInputEvents();
    }

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler, PlayerInventoryHandler _playerInventoryHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler, _playerInventoryHandler);
        
        flashlightResourceData.OnBatteryRestored += BatteryRestored;

        lightReference.enabled = !flashlightResourceData.IsEmpty();
        if(lightReference.enabled){
            StartBatteryTimer();
            OnFlashlightTurnedOnEvent?.Invoke(this, EventArgs.Empty);
            OnFlashlightTurnedOn?.Invoke();
        }
    }

    protected override void SubscribeToInputEvents(){
        playerInputHandler.OnEmergencyItemUse += ToolUseInput;
    }

    protected override void UnsubscribeFromInputEvents(){
        playerInputHandler.OnEmergencyItemUse -= ToolUseInput;
    }

    public override void ToolUseInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != UnityEngine.InputSystem.InputActionPhase.Performed) return;
        if(flashlightResourceData.IsEmpty()){ 
            if(!AttemptReloadFlashlight()){
                OnFlashlightInteractWhileBatteryDead?.Invoke();
            }
            return;
        } 

        lightReference.enabled = !lightReference.enabled;
        if(lightReference.enabled){
            StartBatteryTimer();
            OnFlashlightTurnedOnEvent?.Invoke(this, EventArgs.Empty);
            OnFlashlightTurnedOn?.Invoke();
        }
        else{
            StopAllCoroutines();
            OnFlashlightTurnedOffEvent?.Invoke(this, EventArgs.Empty);
            OnFlashlightTurnedOff?.Invoke();
        }
    }

    public override ResourceDataSO GetEquippedItemResourceData(){
        return flashlightResourceData;
    }

    public void BatteryDied(){
        StopFlickerCoroutine();
        lightReference.enabled = false;
    }

    public void StartBatteryTimer(){
        StartCoroutine(BatteryTimerCoroutine());
    }

    private void BatteryRestored(object sender, EventArgs e){
        OnFlashlightReloaded?.Invoke();
        OnCurrentBatteryTimeChanged?.Invoke(this, EventArgs.Empty);
        CalculateIntensity();
    }

    private void CalculateIntensity(){
        float normalizedBatteryTime = flashlightResourceData.GetNormalizedBatteryTime();
        if(normalizedBatteryTime > lowBatteryEffectsStartAmount){
            lightReference.intensity = fullyChargedIntensity;
            return;
        }
        float intensityFalloff = flashlightIntensityFalloff.Evaluate(normalizedBatteryTime);

        float newIntensity = Mathf.Lerp(minChargedIntensity, fullyChargedIntensity, intensityFalloff);
        lightReference.intensity = newIntensity;
    }

    private bool AttemptReloadFlashlight(){
        if(playerInventory.HasItemInInventory(flashlightResourceData.GetValidItemData())){
            StartReloadFlashlight();
            OnFlashlightReloaded?.Invoke();
            return true;
        }
        return false;
    }

    private void StartReloadFlashlight(){
        playerInventory.AttemptRemoveItemAmountFromInventory(flashlightResourceData.GetValidItemData(), flashlightResourceData.GetMissingStackCount(), out int amountRemoved);

        flashlightResourceData.AddItemStack(amountRemoved);
    }

    private void AttemptToFlickerFlashlight(){
        float normalizedBatteryTime = flashlightResourceData.GetNormalizedBatteryTime();
        if(normalizedBatteryTime > lowBatteryEffectsStartAmount){
            return;
        }
        float flickerChance = flashlightFlickeringCurve.Evaluate(normalizedBatteryTime);
        
        flickerChance = baseFlickerChance / flickerChance;
        if(flickerChance > 100f) flickerChance = 100f;
        
        float randomRoll = Random.Range(0f, 100f); 

        if(randomRoll <= flickerChance){
            StopFlickerCoroutine(); 
            flashlightFlickerCoroutine = FlashlightFlickerCoroutine();
            StartCoroutine(flashlightFlickerCoroutine);   
        }
    }

    private void StopFlickerCoroutine(){
        if(flashlightFlickerCoroutine != null){
            StopCoroutine(flashlightFlickerCoroutine);
            flashlightFlickerCoroutine = null;
        }
    }

    private IEnumerator BatteryTimerCoroutine(){
        CalculateIntensity();
        while(flashlightResourceData.GetCurrentBatteryTimeInSeconds() > 0){
            yield return new WaitForSeconds(1f);
            float currentTime = flashlightResourceData.GetCurrentBatteryTimeInSeconds();
            flashlightResourceData.SetCurrentBatteryTimeInSeconds(currentTime - 1f);
            OnCurrentBatteryTimeChanged?.Invoke(this, EventArgs.Empty);
            CalculateIntensity();

            AttemptToFlickerFlashlight();
        }

        BatteryDied();
        flashlightResourceData.RemoveItem();
    }

    private IEnumerator FlashlightFlickerCoroutine(){
        lightReference.enabled = false;
        OnFlashlightFlickered?.Invoke();
        yield return new WaitForSeconds(Random.Range(flashlightMinFlickerTime, flashlightMaxFlickerTime));
        lightReference.enabled = true;
    }
}