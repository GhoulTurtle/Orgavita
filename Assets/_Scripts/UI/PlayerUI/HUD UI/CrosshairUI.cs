using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerEquippedItemHandler playerEquippedItemHandler;

    [Header("UI References")]
    [SerializeField] private GameObject playerDotCrosshair;
    [SerializeField] private List<RectTransform> playerWeaponCrosshair = new List<RectTransform>();

    [Header("Crosshair Animation Variables")]
    [SerializeField] private float weaponCrosshairAnimationTimeInSeconds = 0.5f;

    private EquippedItemBehaviour equippedItemBehaviour;
    private WeaponDataSO equippedItemWeaponData;

    private void Start() {
        playerEquippedItemHandler.OnWeaponItemBehaviourSpawned += WeaponSpawned;
        playerEquippedItemHandler.OnWeaponItemBehaviourDespawned += WeaponDespawned;
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    private void WeaponDespawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        if(equippedItemBehaviour != null){
            equippedItemBehaviour.OnWeaponAltUse -= OnWeaponAimed;
            equippedItemBehaviour.OnWeaponAltCancel -= OnStopWeaponAimed;

            equippedItemBehaviour = null;
            playerDotCrosshair.SetActive(true);
            foreach (RectTransform crosshairGameobject in playerWeaponCrosshair){
                crosshairGameobject.gameObject.SetActive(false);
            }
        } 

        equippedItemWeaponData = null;
    }

    private void WeaponSpawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        equippedItemBehaviour = e.equippedItemBehaviour;
        equippedItemWeaponData = equippedItemBehaviour.GetEquippedWeaponData();

        equippedItemBehaviour.OnWeaponAltUse += OnWeaponAimed;
        equippedItemBehaviour.OnWeaponAltCancel += OnStopWeaponAimed;

        playerDotCrosshair.SetActive(false);
        foreach (RectTransform crosshairGameobject in playerWeaponCrosshair){
            crosshairGameobject.gameObject.SetActive(true);
        }

        if(equippedItemWeaponData != null){
            CalculateCrosshair(equippedItemWeaponData.baseBloomAngle);
        }
    }

    public void OnWeaponFired(){
        WeaponFiredCrosshairAnimation();
    }

    public void OnWeaponAimed(object sender, EventArgs e){
        if(equippedItemWeaponData != null){
            CalculateCrosshair(equippedItemWeaponData.steadiedBloomAngle);
        }
    }

    public void OnStopWeaponAimed(object sender, EventArgs e){
        if(equippedItemWeaponData != null){
            CalculateCrosshair(equippedItemWeaponData.baseBloomAngle);
        }
    }

    private void WeaponFiredCrosshairAnimation(){

    }

    private void CalculateCrosshair(float bloomAngle){
        //TO-DO: Refactor to work with different resolutions, and be cleaner
        StopAllCoroutines();

        for (int i = 0; i < playerWeaponCrosshair.Count; i++){
            playerWeaponCrosshair[i].localPosition = Vector3.zero;
            switch(i){
                case 0:
                    StartCoroutine(UIAnimator.LerpingAnimationCoroutine(playerWeaponCrosshair[i], new Vector3(0, playerWeaponCrosshair[i].localPosition.y + bloomAngle * 12, 0), weaponCrosshairAnimationTimeInSeconds, false));
                break;
                case 1: 
                    StartCoroutine(UIAnimator.LerpingAnimationCoroutine(playerWeaponCrosshair[i], new Vector3(playerWeaponCrosshair[i].localPosition.x + bloomAngle * 12, 0, 0), weaponCrosshairAnimationTimeInSeconds, false));
                break;
                case 2: 
                    StartCoroutine(UIAnimator.LerpingAnimationCoroutine(playerWeaponCrosshair[i], new Vector3(0, playerWeaponCrosshair[i].localPosition.y - bloomAngle * 12, 0), weaponCrosshairAnimationTimeInSeconds, false));
                break;
                case 3: 
                    StartCoroutine(UIAnimator.LerpingAnimationCoroutine(playerWeaponCrosshair[i], new Vector3(playerWeaponCrosshair[i].localPosition.x - bloomAngle * 12, 0, 0), weaponCrosshairAnimationTimeInSeconds, false));
                break;
                default:
                break;
            }
        }
    }
}