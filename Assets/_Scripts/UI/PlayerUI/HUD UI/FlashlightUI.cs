using System;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerEquippedItemHandler playerEquippedItemHandler;

    [Header("UI References")]
    [SerializeField] private GameObject playerFlashlightUI;
    [SerializeField] private Slider flashlightUIChargeSlider;

    private FlashlightBehaviour currentFlashlightBehaviour;
    private FlashlightResourceDataSO currentFlashlightResourceData;

    private void Awake() {
        playerEquippedItemHandler.OnEmergencyItemBehaviourSpawned += SetupFlashlightUI;
        playerEquippedItemHandler.OnEmergencyItemBehaviourDespawned += UnsubscribeFromFlashlightEvents;
    }

    private void OnDestroy() {
        playerEquippedItemHandler.OnEmergencyItemBehaviourSpawned -= SetupFlashlightUI;
        playerEquippedItemHandler.OnEmergencyItemBehaviourDespawned -= UnsubscribeFromFlashlightEvents;
        UnsubscribeFromFlashlightEvents(null, null);
    }

    private void Start() {
        playerFlashlightUI.SetActive(false);
    }

    private void SetupFlashlightUI(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        if(e.equippedItemBehaviour is FlashlightBehaviour flashlightBehaviour){
            currentFlashlightBehaviour = flashlightBehaviour;

            currentFlashlightBehaviour.OnCurrentBatteryTimeChanged += CurrentBatteryTimeChanged;

            ShowFlashlightUI();
        }
    }

    private void UnsubscribeFromFlashlightEvents(object sender, EventArgs e){
        if(currentFlashlightBehaviour != null){
            currentFlashlightBehaviour.OnCurrentBatteryTimeChanged -= CurrentBatteryTimeChanged;
            currentFlashlightBehaviour = null;
            HideFlashlightUI();
        }

        if(currentFlashlightResourceData != null){
            currentFlashlightResourceData = null;
        }
    }

    private void ShowFlashlightUI(){
        playerFlashlightUI.SetActive(true);
        var resourceData = currentFlashlightBehaviour.GetEquippedItemResourceData();
        currentFlashlightResourceData = (FlashlightResourceDataSO)resourceData;
        UpdateFlashlightSlider();
    }

    private void HideFlashlightUI(){
        playerFlashlightUI.SetActive(false);
    }

    private void CurrentBatteryTimeChanged(object sender, EventArgs e){
        UpdateFlashlightSlider();
    }
    
    private void UpdateFlashlightSlider(){
        if(currentFlashlightResourceData == null) return;

        if(currentFlashlightResourceData.IsEmpty()){
            flashlightUIChargeSlider.value = 0;
            return;
        }

        flashlightUIChargeSlider.value = currentFlashlightResourceData.GetCurrentBatteryTimeInSeconds() / currentFlashlightResourceData.GetMaxBatteryTimeInSeconds(); 
    }
}