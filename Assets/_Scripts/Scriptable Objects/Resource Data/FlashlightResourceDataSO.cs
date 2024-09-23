using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource Data/Flashlight Resource Data", fileName = "FlashlightResourceDataSO")]
public class FlashlightResourceDataSO : ResourceDataSO{
    [Range(0.01f, 1f)]
    [SerializeField] private float defaultBatteryPercent = 0.5f;

    [Header("Flashlight Resource Variables")]
    [SerializeField, Range(0.1f, 1000)] private float maxBatteryTimeInSeconds;


    private float currentBatteryTimeInSeconds = 0;

    public EventHandler OnBatteryRestored;

    #if UNITY_EDITOR
    private new void OnEnable() {
        ResetResourceData();
    }
    #endif

    public override bool IsEmpty(){
        return currentBatteryTimeInSeconds == 0;
    }

    public override void AddItemStack(int stackAmount){
        base.AddItemStack(stackAmount);
        currentBatteryTimeInSeconds = maxBatteryTimeInSeconds;
        OnBatteryRestored?.Invoke(this, EventArgs.Empty);
        OnResourceUpdated?.Invoke(currentStack);
    }

    public override void RemoveItem(){
        currentStack--;
        currentBatteryTimeInSeconds = 0;

        OnResourceUpdated?.Invoke(currentStack);
    }

    public override void ResetResourceData(){
        base.ResetResourceData();
        currentBatteryTimeInSeconds = currentStack == 0 ? 0f : maxBatteryTimeInSeconds * defaultBatteryPercent;
    }

    public override bool IsFull(){
        return currentBatteryTimeInSeconds == maxBatteryTimeInSeconds;
    }

    public int GetCurrentBatteryPercentage(){
        float scaledValue = currentBatteryTimeInSeconds / maxBatteryTimeInSeconds * 100;
        return Mathf.RoundToInt(scaledValue);
    }

    public float GetCurrentBatteryTimeInSeconds(){
        return currentBatteryTimeInSeconds;
    }

    public void SetCurrentBatteryTimeInSeconds(float batteryTimeInSeconds){
        currentBatteryTimeInSeconds = batteryTimeInSeconds;
        OnResourceUpdated?.Invoke(currentStack);
    }

    public float GetMaxBatteryTimeInSeconds(){
        return maxBatteryTimeInSeconds;
    }

    public override int GetMissingStackCount(){
        if(currentBatteryTimeInSeconds != maxBatteryTimeInSeconds){
            return 1;
        }
        
        return 0;
    }
}