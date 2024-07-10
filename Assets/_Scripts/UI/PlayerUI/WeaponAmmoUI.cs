using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAmmoUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private ResourceDataSO weaponResourceData;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private Slider ammoCountSlider;
    [SerializeField] private Image ammoSliderFillImage;

    [Header("Weapon Ammo UI Variables")]
    [SerializeField] private Gradient ammoSliderGradient;

    private void Awake() {
        if(weaponResourceData != null){
            weaponResourceData.OnMaxStackUpdated += UpdateWeaponAmmoUI;
            weaponResourceData.OnResourceUpdated += (amount) => UpdateWeaponAmmoUI();
        }
    }

    private void Start() {
        if(weaponResourceData != null){
            UpdateWeaponAmmoUI();
        }
    }

    private void OnDestroy() {
        if(weaponResourceData != null){
            weaponResourceData.OnMaxStackUpdated -= UpdateWeaponAmmoUI;
            weaponResourceData.OnResourceUpdated -= (amount) => UpdateWeaponAmmoUI();
        }
    }

    private void UpdateWeaponAmmoUI(){
        ammoCountText.text = weaponResourceData.GetCurrentStackCount() + "/" + weaponResourceData.GetMaxStackCount();

        float sliderValue = (float)weaponResourceData.GetCurrentStackCount() / weaponResourceData.GetMaxStackCount();

        ammoCountSlider.value = sliderValue;

        ammoSliderFillImage.color = ammoSliderGradient.Evaluate(sliderValue);
    }
}
