using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource Data/Flashlight Resource Data", fileName = "FlashlightResourceDataSO")]
public class FlashlightResourceDataSO : ResourceDataSO{
    [Header("Flashlight Resource Variables")]
    [SerializeField, Range(0.1f, 600)] private float maxBatteryTimeInSeconds;

    private float currentBatteryTimeInSeconds = 0;

    public EventHandler OnBatteryRestored;

    #if UNITY_EDITOR
    private new void OnEnable() {
        base.OnEnable();
        currentBatteryTimeInSeconds = currentStack == 0 ? 0f : maxBatteryTimeInSeconds;
    }
    #endif

    public override bool IsEmpty(){
        return currentBatteryTimeInSeconds == 0;
    }

    public override void AddItem(){
        base.AddItem();
        currentBatteryTimeInSeconds = maxBatteryTimeInSeconds;
        OnBatteryRestored?.Invoke(this, EventArgs.Empty);
    }

    public override void RemoveItem(){
        currentStack--;
        currentBatteryTimeInSeconds = 0;
    }

    public override bool IsFull(){
        return currentBatteryTimeInSeconds == maxBatteryTimeInSeconds;
    }

    public float GetCurrentBatteryTimeInSeconds(){
        return currentBatteryTimeInSeconds;
    }

    public void SetCurrentBatteryTimeInSeconds(float batteryTimeInSeconds){
        currentBatteryTimeInSeconds = batteryTimeInSeconds;
    }

    public float GetMaxBatteryTimeInSeconds(){
        return maxBatteryTimeInSeconds;
    }
}