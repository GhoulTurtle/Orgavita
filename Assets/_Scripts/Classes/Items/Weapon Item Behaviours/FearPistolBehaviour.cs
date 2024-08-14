using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FearPistolBehaviour : GunWeaponEquippedItemBehaviour{
    [Header("Fear Pistol Required References")]
    [SerializeField] private TerrainDataList validBounceableTerrainDataList;

    [Header("Fear Pistol Visual References")]
    [SerializeField] private FadeObject fearPistolBulletDecalDent;

    [Header("Fear Pistol Unity Events")]
    [SerializeField] private UnityEvent<AudioSource> OnBulletBounced;

    private FearPistolWeaponDataSO fearPistolWeaponDataSO;

    private int rayBounceCounter = 0;

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler, PlayerInventoryHandler _playerInventoryHandler){        
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler, _playerInventoryHandler);
        fearPistolWeaponDataSO = (FearPistolWeaponDataSO)weaponData;
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

    public override void WeaponAltUseInput(object sender, InputEventArgs e){
        AimWeapon(e.inputActionPhase);
    }
    
    public override void ReloadInput(object sender, InputEventArgs e){
        if (e.inputActionPhase != InputActionPhase.Performed || currentWeaponState == WeaponState.Reloading || weaponResourceData.IsFull()) return;
        if (!playerInventory.HasItemInInventory(weaponResourceData.GetValidItemData())) return;

        StartReloadAction();
    }

    public override void InspectInput(object sender, InputEventArgs e){
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

    protected override void HandleGunShotImpact(Ray ray){
        Debug.DrawRay(ray.origin, ray.direction, Color.white, 5f);
        FadeObject fadeObject;

        if(!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, validHitLayermask, QueryTriggerInteraction.Ignore)){
            ResetBounceCounter();
            return;
        }

        if(hitInfo.collider.TryGetComponent(out IDamagable damagable)){
            damagable.TakeDamage(weaponData.weaponAttackDamage, playerReference, hitInfo.point);
            //TO-DO: Figure out what we hit then spawn the right vfx and play the right impact sound.
            return;
        }

        if(hitInfo.collider.TryGetComponent(out Terrain terrain)){
            TerrainType terrainType = terrain.GetTerrainType();

            if(rayBounceCounter < fearPistolWeaponDataSO.maxBounceCount && validBounceableTerrainDataList.IsEnteredTerrainValid(terrainType) && rayBounceCounter < fearPistolWeaponDataSO.maxBounceCount){
                BounceFearShot(hitInfo, ray);
                //TO-DO: Play spark vfx
                fadeObject = SpawnBulletDecal(fearPistolBulletDecalDent, hitInfo.point, hitInfo.normal);

                OnBulletBounced?.Invoke(fadeObject.GetFadeObjectAudioSource());
                return;
            }

            if(rayBounceCounter >= fearPistolWeaponDataSO.maxBounceCount){
                ResetBounceCounter();
            }

            fadeObject = SpawnBulletDecal(weaponBulletDecalHole, hitInfo.point, hitInfo.normal);
            OnTerrainImpacted?.Invoke(terrainType, fadeObject.GetFadeObjectAudioSource());
        }

        fadeObject = SpawnBulletDecal(weaponBulletDecalHole, hitInfo.point, hitInfo.normal);
        OnTerrainImpacted?.Invoke(TerrainType.None, fadeObject.GetFadeObjectAudioSource());
    }

    private void BounceFearShot(RaycastHit hitInfo, Ray ray){
        rayBounceCounter++;

        Vector3 rayPoint = hitInfo.point;
        Vector3 rayNorm = hitInfo.normal;
        Vector3 rayDir = ray.direction;

        Vector3 reflectedVector = rayDir - 2 * Vector3.Dot(rayDir, rayNorm) * rayNorm;

        ray.origin = rayPoint;
        ray.direction = reflectedVector;

        HandleGunShotImpact(ray);
    }

    private void ResetBounceCounter(){
        rayBounceCounter = 0;
    }
}