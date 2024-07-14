using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ShotgunBehaviour : GunWeaponEquippedItemBehaviour{
    private ShotgunWeaponDataSO shotgunWeaponDataSO;

    public UnityEvent OnShellReloaded;

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler, PlayerInventoryHandler _playerInventoryHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler, _playerInventoryHandler);
        shotgunWeaponDataSO = (ShotgunWeaponDataSO)weaponData;
    }

    public override void WeaponUseInput(object sender, InputEventArgs e){
        if (currentWeaponState == WeaponState.Inspecting) return;
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        if (fireRateCoroutineContainer != null) return;

        if (weaponResourceData.IsEmpty()){
            if(currentWeaponState == WeaponState.Reloading) return;

            if (playerInventory.HasItemInInventory(weaponResourceData.GetValidItemData())){
                StartReloadAction();
            }
            else{
                OnEmptyGunTriggered?.Invoke();
            }
            return;
        }

        if(currentWeaponState == WeaponState.Reloading){
            StopReloadAction();
        }

        for (int i = 0; i < shotgunWeaponDataSO.pelletsPerShot; i++){
            float bloomAngle = currentWeaponState == WeaponState.Aiming ? weaponData.steadiedBloomAngle : weaponData.baseBloomAngle;

            Vector3 bloom = WeaponHelper.CalculateBloom(bloomAngle, cameraTransform.forward);
            Ray firedRay = new(cameraTransform.position, bloom);  
            HandleGunShotImpact(firedRay); 
        }

        OnWeaponUse?.Invoke(this, EventArgs.Empty);
        OnGunFired?.Invoke();

        SpawnBulletCasing();
        FireRateCooldown();
        AttemptTriggerKickback();
        UseWeaponResource();
    }

    protected override void HandleGunShotImpact(Ray ray){
        base.HandleGunShotImpact(ray);
    }

    public override void WeaponAltUseInput(object sender, InputEventArgs e){
        AimGun(e.inputActionPhase);
    }

    public override void ReloadInput(object sender, InputEventArgs e){
        if (e.inputActionPhase != InputActionPhase.Performed || currentWeaponState == WeaponState.Reloading || weaponResourceData.IsFull()) return;
        if (!playerInventory.HasItemInInventory(weaponResourceData.GetValidItemData())) return;

        StartReloadAction();
    }

    protected override void ReloadActionFinished(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        if(weaponResourceData.IsFull()){
            StopReloadAction();
            return;
        }

        playerInventory.AttemptRemoveItemAmountFromInventory(weaponResourceData.GetValidItemData(), 1, out int amountRemoved);

        if(amountRemoved > 0){
            weaponResourceData.AddItemStack(amountRemoved);
            OnShellReloaded?.Invoke();
        }
        else{
            StopReloadAction();
            return;
        }
        
        reloadCoroutineContainer.OnCoroutineDisposed -= ReloadActionFinished;   
        reloadCoroutineContainer = null;

        reloadCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, weaponData.weaponReloadTimeInSeconds);
        reloadCoroutineContainer.OnCoroutineDisposed += ReloadActionFinished;
    }

    public override void InspectInput(object sender, InputEventArgs e){
        InspectGun(e.inputActionPhase);
    }

    protected override void SubscribeToInputEvents(){
        playerInputHandler.OnWeaponUse += WeaponUseInput;
        playerInputHandler.OnAltWeaponUse += WeaponAltUseInput;
        playerInputHandler.OnReload += ReloadInput;
        playerInputHandler.OnInspect += InspectInput;
    }

    protected override void UnsubscribeFromInputEvents(){
        playerInputHandler.OnWeaponUse -= WeaponUseInput;
        playerInputHandler.OnAltWeaponUse -= WeaponAltUseInput;
        playerInputHandler.OnReload -= ReloadInput;
        playerInputHandler.OnInspect -= InspectInput;
    }


}
