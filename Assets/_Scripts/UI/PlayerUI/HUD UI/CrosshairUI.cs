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

    private const float LERP_SNAP_DISTANCE = 0.01f;

    private void Start() {
        playerEquippedItemHandler.OnWeaponItemBehaviourSpawned += WeaponSpawned;
        playerEquippedItemHandler.OnWeaponItemBehaviourDespawned += WeaponDespawned;
    }

    private void OnDestroy() {
        StopAllCoroutines();
        playerEquippedItemHandler.OnWeaponItemBehaviourSpawned -= WeaponSpawned;
        playerEquippedItemHandler.OnWeaponItemBehaviourDespawned -= WeaponDespawned;

        if(equippedItemBehaviour != null){
            equippedItemBehaviour.OnWeaponUse -= OnWeaponFired;
            equippedItemBehaviour.OnWeaponAltUse -= OnWeaponAimed;
            equippedItemBehaviour.OnWeaponAltCancel -= OnStopWeaponAimed;
            equippedItemBehaviour.OnEquippedItemStateChanged -= EvaulateEquippedStateChanged; 
        }
    }

    private void WeaponSpawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        equippedItemBehaviour = e.equippedItemBehaviour;
        equippedItemWeaponData = equippedItemBehaviour.GetEquippedWeaponData();

        equippedItemBehaviour.OnWeaponUse += OnWeaponFired;
        equippedItemBehaviour.OnWeaponAltUse += OnWeaponAimed;
        equippedItemBehaviour.OnWeaponAltCancel += OnStopWeaponAimed;
        equippedItemBehaviour.OnEquippedItemStateChanged += EvaulateEquippedStateChanged; 

        if(equippedItemWeaponData != null){
            CalculateCrosshair(equippedItemWeaponData.baseBloomAngle);
        }

        UpdateCrosshairBasedOnEquippedItemState(equippedItemBehaviour.GetPlayerItemState());
    }

    private void WeaponDespawned(object sender, PlayerEquippedItemHandler.ItemBehaviourSpawnedEventArgs e){
        if(equippedItemBehaviour != null){
            equippedItemBehaviour.OnWeaponUse -= OnWeaponFired;
            equippedItemBehaviour.OnWeaponAltUse -= OnWeaponAimed;
            equippedItemBehaviour.OnWeaponAltCancel -= OnStopWeaponAimed;
            equippedItemBehaviour.OnEquippedItemStateChanged -= EvaulateEquippedStateChanged; 

            equippedItemBehaviour = null;
        } 

        equippedItemWeaponData = null;
    }

    private void UpdateCrosshairBasedOnEquippedItemState(EquippableItemState itemState){
        switch (itemState){
            case EquippableItemState.None:
                break;
            case EquippableItemState.Active: TurnOnCrossCrosshair();
                break;
            case EquippableItemState.Holstered: TurnOnDotCrosshair();
                break;
            case EquippableItemState.Used: TurnOnDotCrosshair();
                break;
            case EquippableItemState.Passive: TurnOnDotCrosshair();
                break;
        }
    }

    private void EvaulateEquippedStateChanged(object sender, EquippedItemBehaviour.EquippedItemStateChangedEventArgs e){
        UpdateCrosshairBasedOnEquippedItemState(e.state);
    }

    public void OnWeaponFired(object sender, EventArgs e){
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
        StopAllCoroutines();

        float currentWeaponBloomAngle = equippedItemBehaviour.GetWeaponState() == WeaponState.Aiming ? equippedItemWeaponData.steadiedBloomAngle : equippedItemWeaponData.baseBloomAngle;

        for (int i = 0; i < playerWeaponCrosshair.Count; i++){
            playerWeaponCrosshair[i].localPosition = Vector3.zero;
            switch(i){
                case 0:
                    StartCoroutine(CrosshairShootAnimation(playerWeaponCrosshair[i], new Vector3(0, playerWeaponCrosshair[i].localPosition.y + equippedItemWeaponData.shootCrosshairMagnitude * 12, 0), new Vector3(0, playerWeaponCrosshair[i].localPosition.y + currentWeaponBloomAngle * 12, 0)));
                break;
                case 1: 
                    StartCoroutine(CrosshairShootAnimation(playerWeaponCrosshair[i], new Vector3(playerWeaponCrosshair[i].localPosition.x + equippedItemWeaponData.shootCrosshairMagnitude * 12, 0, 0), new Vector3(playerWeaponCrosshair[i].localPosition.x + currentWeaponBloomAngle * 12, 0, 0)));
                break;
                case 2: 
                    StartCoroutine(CrosshairShootAnimation(playerWeaponCrosshair[i], new Vector3(0, playerWeaponCrosshair[i].localPosition.y - equippedItemWeaponData.shootCrosshairMagnitude * 12, 0), new Vector3(0, playerWeaponCrosshair[i].localPosition.y - currentWeaponBloomAngle * 12, 0)));
                break;
                case 3: 
                    StartCoroutine(CrosshairShootAnimation(playerWeaponCrosshair[i], new Vector3(playerWeaponCrosshair[i].localPosition.x - equippedItemWeaponData.shootCrosshairMagnitude * 12, 0, 0), new Vector3(playerWeaponCrosshair[i].localPosition.x - currentWeaponBloomAngle * 12, 0, 0)));                
                    break;
                default:
                break;
            }
        }
    }

    private void TurnOnDotCrosshair(){
        playerDotCrosshair.SetActive(true);
        foreach (RectTransform crosshairGameobject in playerWeaponCrosshair){
            crosshairGameobject.gameObject.SetActive(false);
        }
    }

    private void TurnOnCrossCrosshair(){
        playerDotCrosshair.SetActive(false);
        foreach (RectTransform crosshairGameobject in playerWeaponCrosshair){
            crosshairGameobject.gameObject.SetActive(true);
        }
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

    private IEnumerator CrosshairShootAnimation(RectTransform crosshairTransform, Vector3 goalPosition, Vector3 returnPosition){
        float current = 0;
        while(Vector3.Distance(crosshairTransform.localPosition, goalPosition) > LERP_SNAP_DISTANCE){
            crosshairTransform.localPosition = Vector3.Lerp(crosshairTransform.localPosition, goalPosition, current / equippedItemWeaponData.shootCrosshairAnimationTimeInSeconds);
            current += Time.deltaTime;
            yield return null;
        }

        crosshairTransform.localPosition = goalPosition;

        current = 0;
        while(Vector3.Distance(crosshairTransform.localPosition, returnPosition) > LERP_SNAP_DISTANCE){
            crosshairTransform.localPosition = Vector3.Lerp(crosshairTransform.localPosition, returnPosition, current / equippedItemWeaponData.shootCrosshairAnimationTimeInSeconds);
            current += Time.deltaTime;
            yield return null;
        }

        crosshairTransform.localPosition = returnPosition;
    }
}
