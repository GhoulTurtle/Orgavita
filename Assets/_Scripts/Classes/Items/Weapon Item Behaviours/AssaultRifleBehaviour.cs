using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AssaultRifleBehaviour : GunWeaponEquippedItemBehaviour{
    [Header("Assault Rifle Unity Events")]
    [SerializeField] private UnityEvent OnWeaponFiringModeSwitchUnityEvent;
    private EventHandler<WeaponFiringModeSwitchEventArgs> OnWeaponFiringModeSwitch;
    private class WeaponFiringModeSwitchEventArgs : EventArgs{
        public WeaponFiringMode weaponFiringMode;
        public WeaponFiringModeSwitchEventArgs(WeaponFiringMode _weaponFiringMode){
            weaponFiringMode = _weaponFiringMode;
        }
    } 

    private WeaponFiringMode currentWeaponFiringMode = WeaponFiringMode.Automatic;

    private AssaultRifleWeaponDataSO assaultRifleWeaponDataSO;

    private IEnumerator autoFireCoroutine;
    private IEnumerator autoBurstFireCoroutine;

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler, PlayerInventoryHandler _playerInventoryHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler, _playerInventoryHandler);
        assaultRifleWeaponDataSO = (AssaultRifleWeaponDataSO)weaponData;
    }

    public override void SaveData(){
        base.SaveData();
        StopAllCoroutines();
    }

    public override void WeaponUseInput(object sender, InputEventArgs e){
        if (currentWeaponState == WeaponState.Inspecting && e.inputActionPhase == InputActionPhase.Performed){
            SwitchWeaponFiringMode();
            return;
        }

        if(currentWeaponState == WeaponState.Inspecting && e.inputActionPhase != InputActionPhase.Performed) return;

        if (weaponResourceData.IsEmpty()){
            if(currentWeaponState == WeaponState.Reloading) return;

            if (playerInventory.HasItemInInventory(weaponResourceData.GetValidItemData())){
                StartReloadAction();
            }
            else{
                if(e.inputActionPhase != InputActionPhase.Performed) return;
                OnEmptyGunTriggered?.Invoke();
            }
            return;
        }


        if(currentWeaponFiringMode == WeaponFiringMode.Automatic){
            if(e.inputActionPhase == InputActionPhase.Started){
                StartFireAuto();
                if(currentWeaponState == WeaponState.Reloading){
                    StopReloadAction();
                }
            }
            else if(e.inputActionPhase == InputActionPhase.Canceled){
                StopFireAuto();
            }
        }
        else{
            if(e.inputActionPhase != InputActionPhase.Performed || fireRateCoroutineContainer != null || autoBurstFireCoroutine != null) return;
            
            if(currentWeaponState == WeaponState.Reloading){
                StopReloadAction();
            }
            
            FireBurst();
        }
    }

    private void FireBurst(){
        autoBurstFireCoroutine = BurstFireCoroutine();
        StartCoroutine(autoBurstFireCoroutine);
    }

    private void StartFireAuto(){
        autoFireCoroutine = AutoFireCoroutine();
        StartCoroutine(autoFireCoroutine);
    }

    private void StopFireAuto(){
        if(autoFireCoroutine != null){
            StopCoroutine(autoFireCoroutine);
            autoFireCoroutine = null;
        }
    }

    public override void WeaponAltUseInput(object sender, InputEventArgs e){
        AimWeapon(e.inputActionPhase);
    }

    public override void ReloadInput(object sender, InputEventArgs e){
        if (e.inputActionPhase != InputActionPhase.Performed || currentWeaponState == WeaponState.Reloading || weaponResourceData.IsFull()) return;
        if (!playerInventory.HasItemInInventory(weaponResourceData.GetValidItemData())) return;

        StopFireAuto();

        StartReloadAction();
    }

    public override void InspectInput(object sender, InputEventArgs e){
        if(currentWeaponState == WeaponState.Aiming) return;
        StopFireAuto();
        InspectWeapon(e.inputActionPhase);
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

    private void SwitchWeaponFiringMode(){
        if(currentWeaponFiringMode == WeaponFiringMode.Automatic){
            currentWeaponFiringMode = WeaponFiringMode.Burst;
        }
        else{
            currentWeaponFiringMode = WeaponFiringMode.Automatic;
        }

        OnWeaponFiringModeSwitch?.Invoke(this, new WeaponFiringModeSwitchEventArgs(currentWeaponFiringMode));
        OnWeaponFiringModeSwitchUnityEvent?.Invoke();
    }

    private IEnumerator BurstFireCoroutine(){
        for (int i = 0; i < assaultRifleWeaponDataSO.burstShotAmount; i++){
            if(weaponResourceData.IsEmpty()){
                BurstFireEnded();
                break;
            }

            float bloomAngle = currentWeaponState == WeaponState.Aiming ? weaponData.steadiedBloomAngle : weaponData.baseBloomAngle;

            Vector3 bloom = WeaponHelper.CalculateBloom(bloomAngle, cameraTransform.forward);

            Ray firedRay = new(cameraTransform.position, bloom);  
            HandleGunShotImpact(firedRay); 
            
            OnWeaponUse?.Invoke(this, EventArgs.Empty);
            OnGunFired?.Invoke();
            
            SpawnBulletCasing();
            AttemptTriggerKickback();
            UseWeaponResource();
            yield return new WaitForSeconds(assaultRifleWeaponDataSO.burstShotRateCooldownInSeconds);
        }

        BurstFireEnded();
    }

    private void BurstFireEnded(){
        if(autoBurstFireCoroutine != null){
            StopCoroutine(autoBurstFireCoroutine);
            autoBurstFireCoroutine = null;
        }

        FireRateCooldown();
    }

    private IEnumerator AutoFireCoroutine(){
        while(!weaponResourceData.IsEmpty()){
            if(fireRateCoroutineContainer == null){
                float bloomAngle = currentWeaponState == WeaponState.Aiming ? weaponData.steadiedBloomAngle : weaponData.baseBloomAngle;
                Vector3 bloom = WeaponHelper.CalculateBloom(bloomAngle, cameraTransform.forward);
                Ray firedRay = new(cameraTransform.position, bloom);

                OnWeaponUse?.Invoke(this, EventArgs.Empty);
                OnGunFired?.Invoke();

                HandleGunShotImpact(firedRay);
                SpawnBulletCasing();
                FireRateCooldown();
                AttemptTriggerKickback();
                UseWeaponResource();
            }

            yield return null;
        }
    }

    protected override void FireRateCooldown(){
        float fireRateCooldown = currentWeaponFiringMode == WeaponFiringMode.Automatic ? weaponData.weaponFireRateInSeconds : assaultRifleWeaponDataSO.burstFireRateInSeconds;

        fireRateCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, fireRateCooldown);
        fireRateCoroutineContainer.OnCoroutineDisposed += FireRateCooldownFinished;
    }

    protected override void HandleGunShotImpact(Ray ray){
        Debug.DrawRay(ray.origin, ray.direction, Color.white, 5f);
        FadeObject fadeObject;

        if(!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, validHitLayermask, QueryTriggerInteraction.Ignore)) return;

        if(hitInfo.collider.TryGetComponent(out IDamagable damagable)){
            float damageAmount = currentWeaponFiringMode == WeaponFiringMode.Automatic ? weaponData.weaponAttackDamage : assaultRifleWeaponDataSO.burstDamageAmount;
            damagable.TakeDamage(damageAmount, hitInfo.point);
            //TO-DO: Figure out what we hit then spawn the right vfx and play the right impact sound.
            return;
        }

        fadeObject = SpawnBulletDecal(weaponBulletDecalHole, hitInfo.point, hitInfo.normal);

        if(hitInfo.collider.TryGetComponent(out Terrain terrain)){
            TerrainType terrainType = terrain.GetTerrainType();

            OnTerrainImpacted?.Invoke(terrainType, fadeObject.GetFadeObjectAudioSource());
        }

        OnTerrainImpacted?.Invoke(TerrainType.None, fadeObject.GetFadeObjectAudioSource());
    }

    protected override void TriggerKickback(){
        float kickbackAmount = currentWeaponFiringMode == WeaponFiringMode.Automatic ? weaponData.kickBackAmount : assaultRifleWeaponDataSO.burstKickBackAmount;

        OnKickbackApplied?.Invoke(this, new KickbackAppliedEventArgs(kickbackAmount));   
    }
}