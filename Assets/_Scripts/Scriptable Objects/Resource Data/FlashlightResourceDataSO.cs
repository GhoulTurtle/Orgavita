using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource Data/Flashlight Resource Data", fileName = "FlashlightResourceDataSO")]
public class FlashlightResourceDataSO : ResourceDataSO{
    [Header("Flashlight Resource Variables")]
    [SerializeField, Range(0.1f, 600)] private float maxBatteryTimeInSeconds;
    
    private float currentBatteryTimeInSeconds = 0;

    #if UNITY_EDITOR
    private new void OnEnable() {
        base.OnEnable();
        currentBatteryTimeInSeconds = currentStack == 0 ? 0f : maxBatteryTimeInSeconds;
    }
    #endif

    public void StartBatteryTime(FlashlightBehaviour flashlightBehaviour){
        flashlightBehaviour.StartCoroutine(BatteryTimerCoroutine(flashlightBehaviour));
    }

    public void StopBatteryTime(FlashlightBehaviour flashlightBehaviour){
        flashlightBehaviour.StopAllCoroutines();
    }

    public override bool IsEmpty(){
        return currentBatteryTimeInSeconds == 0;
    }

    public override void AddItem(){
        base.AddItem();
        currentBatteryTimeInSeconds = maxBatteryTimeInSeconds;
    }

    public override void RemoveItem(){
        currentStack--;
        currentBatteryTimeInSeconds = 0;
    }

    public override bool IsFull(){
        return currentBatteryTimeInSeconds == maxBatteryTimeInSeconds;
    }

    private IEnumerator BatteryTimerCoroutine(FlashlightBehaviour flashlightBehaviour){
        while(currentBatteryTimeInSeconds > 0){
            yield return new WaitForSeconds(1f);
            currentBatteryTimeInSeconds -= 1f;
        }
        
        flashlightBehaviour.BatteryDied();
        RemoveItem();
    }
}