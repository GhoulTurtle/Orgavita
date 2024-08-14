using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GunWeaponEquippedItemBehaviour : EquippedItemBehaviour{
    [Header("Required References")]
    [SerializeField] protected ResourceDataSO weaponResourceData;
    [SerializeField] protected WeaponDataSO weaponData; 
    [SerializeField] protected TerrainAudioDataList impactSoundTerrainAudioDataList;
    [SerializeField] protected LayerMask validHitLayermask;

    [Header("Bullet Casing Spawn Variables")]
    [SerializeField] private Vector3 localBulletCasingSpawnPoint;
    [SerializeField] private Vector3 localBulletCasingForceDir;
    [SerializeField, MinMaxRange(0f, 300f)] private RangedFloat bulletCasingForceStrength;

    [Header("Visual References")]
    [SerializeField] protected FadeObject weaponBulletDecalHole;
    [SerializeField] protected RigidbodyDetail fearPistolBulletCasing;

    [Header("Debugging")]
    [SerializeField] protected bool showGizmos = true;
    [SerializeField] protected Color gizmosColor = Color.green;
    [SerializeField, Range(0.5f, 3f)] protected float gizmosLength = 1f;
    [SerializeField, Range(3, 100f)] protected int gizmosCircleSides = 36;

    [Header("Weapon Unity Events")]
    [SerializeField] protected UnityEvent<TerrainType, AudioSource> OnTerrainImpacted;  
    [SerializeField] protected UnityEvent OnEmptyGunTriggered;
    [SerializeField] protected UnityEvent OnGunFired;
    [SerializeField] protected UnityEvent OnGunAltFire;
    [SerializeField] protected UnityEvent OnGunReloadedStarted;
    [SerializeField] protected UnityEvent OnGunReloadedStopped;
    [SerializeField] protected UnityEvent OnGunInspected;  

    [Header("Can Not Switch Popup Variables")]
    [SerializeField] private float popupPrintSpeed = 0.01f;
    [SerializeField] private float popupWaitTime = 1.5f;
    [SerializeField] private float popupFadeTime = 1f;

    protected CoroutineContainer kickbackCoroutineContainer;
    protected CoroutineContainer fireRateCoroutineContainer;
    protected CoroutineContainer reloadCoroutineContainer;

    protected int kickbackCounter;

    protected IDamagable playerReference;

    public override void SaveData(){
        DisposeCoroutineContainers();
        playerInputHandler.OnHolsterWeapon -= HolsterWeaponInput;
        playerInventoryHandler.OnInventoryStateChanged -= EvaulateInventoryStateChanged;
        weaponResourceData.OnResourceUpdated -= OnResourceUpdated;
    }

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler, PlayerInventoryHandler _playerInventoryHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler, _playerInventoryHandler);
        inventoryItem = _inventoryItem;
        if(playerReference == null){
            playerInputHandler.TryGetComponent(out playerReference);
        }
        playerInputHandler.OnHolsterWeapon += HolsterWeaponInput;
        playerInventoryHandler.OnInventoryStateChanged += EvaulateInventoryStateChanged;
        weaponResourceData.OnResourceUpdated += OnResourceUpdated;
    }


    public override void HolsterWeaponInput(object sender, InputEventArgs e){
        if(e.callbackContext.phase != InputActionPhase.Performed) return;

        if(!CanSwitchFromWeapon()){
            Dialogue canNotSwitchDialogue = new(){
                Sentence = "Can't holster right now.",
                SentenceColor = Color.red
            };
            PopupUI.Instance.PrintText(canNotSwitchDialogue, popupPrintSpeed, true, popupWaitTime, popupFadeTime);
            return;
        }
        StopReloadAction();
        DisposeCoroutineContainers();

        base.HolsterWeaponInput(sender, e);
    }

    public override void ChangeItemState(EquippableItemState newState){
        if(currentWeaponState == WeaponState.Aiming){
            ChangeWeaponState(WeaponState.Default);
        }

        StopReloadAction();
        DisposeCoroutineContainers();

        base.ChangeItemState(newState);
    }

    public override void EvaulateInventoryStateChanged(object sender, PlayerInventoryHandler.InventoryStateChangedEventArgs e){
        if(e.inventoryState != InventoryState.Closed && currentWeaponState == WeaponState.Reloading){
            StopReloadAction();
        }
    }

    public override ResourceDataSO GetEquippedItemResourceData(){
        return weaponResourceData;
    }

    public override WeaponDataSO GetEquippedWeaponData(){
        return weaponData;
    }

    public bool CanSwitchFromWeapon(){
        return fireRateCoroutineContainer == null;
    }

    protected void AimWeapon(InputActionPhase inputActionPhase){
        if(currentWeaponState == WeaponState.Inspecting) return;

        if(inputActionPhase == InputActionPhase.Performed){
            if(currentWeaponState == WeaponState.Reloading){
                StopReloadAction();
            }

            ChangeWeaponState(WeaponState.Aiming);
            OnWeaponAltUse?.Invoke(this, EventArgs.Empty);
            OnGunAltFire?.Invoke();
        }
        else if(inputActionPhase == InputActionPhase.Canceled && currentWeaponState != WeaponState.Reloading && currentWeaponState == WeaponState.Aiming){
            ChangeWeaponState(WeaponState.Default);
            OnWeaponAltCancel?.Invoke(this, EventArgs.Empty);
        }
    }

    protected void InspectWeapon(InputActionPhase inputActionPhase){
        if(currentWeaponState == WeaponState.Reloading || currentWeaponState == WeaponState.Aiming) return;

        if(inputActionPhase == InputActionPhase.Performed){
            if(currentWeaponState == WeaponState.Aiming){
                OnWeaponAltCancel?.Invoke(this, EventArgs.Empty);
            }
            ChangeWeaponState(WeaponState.Inspecting);
            OnInspectUse?.Invoke(this, EventArgs.Empty);
            OnGunInspected?.Invoke();
        }
        else if(inputActionPhase == InputActionPhase.Canceled){
            ChangeWeaponState(WeaponState.Default);
            OnInspectCanceled?.Invoke(this, EventArgs.Empty);
        }
    }

    protected virtual void FireRateCooldown(){
        fireRateCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, weaponData.weaponFireRateInSeconds);
        fireRateCoroutineContainer.OnCoroutineDisposed += FireRateCooldownFinished;
    }

    protected void FireRateCooldownFinished(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        fireRateCoroutineContainer.OnCoroutineDisposed -= FireRateCooldownFinished;
        fireRateCoroutineContainer = null;
    }

    protected virtual void TriggerKickback(){
        OnKickbackApplied?.Invoke(this, new KickbackAppliedEventArgs(weaponData.kickBackAmount));
    }

    protected virtual void StartReloadAction(){
        OnWeaponAltCancel?.Invoke(this, EventArgs.Empty);
        
        ChangeWeaponState(WeaponState.Reloading);
        
        reloadCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, weaponData.weaponReloadTimeInSeconds);
        reloadCoroutineContainer.OnCoroutineDisposed += ReloadActionFinished;

        OnGunReloadedStarted?.Invoke();
        OnReload?.Invoke(this, EventArgs.Empty);
    }

    protected void StopReloadAction(){
        if(reloadCoroutineContainer != null){
            ChangeWeaponState(WeaponState.Default);
            reloadCoroutineContainer.OnCoroutineDisposed -= ReloadActionFinished;   
            reloadCoroutineContainer = null;
            OnGunReloadedStopped?.Invoke();
        }
    }

    protected virtual void ReloadActionFinished(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        playerInventory.AttemptRemoveItemAmountFromInventory(weaponResourceData.GetValidItemData(), weaponResourceData.GetMissingStackCount(), out int amountRemoved);

        if(amountRemoved > 0){
            weaponResourceData.AddItemStack(amountRemoved);
        }
        
        StopReloadAction();
    }

    protected virtual void AttemptTriggerKickback(){
        if(weaponData.minKickBackShotAmount == 0) return;

        kickbackCounter++;

        if(kickbackCounter >= weaponData.minKickBackShotAmount){
            TriggerKickback();
        }

        if(kickbackCoroutineContainer != null){
            RemoveKickbackTimer();
        }

        CreateKickbackTimer();
    }

    protected void OnResourceUpdated(int obj){
        if(currentWeaponState != WeaponState.Reloading && obj >= 1){
            OnGunReloadedStopped?.Invoke();
        }
    }

    protected void RemoveKickbackTimer(){
        kickbackCoroutineContainer.TryStopCoroutine();
        kickbackCoroutineContainer.OnCoroutineDisposed -= KickbackWindowClosed;
        kickbackCoroutineContainer = null;
    }

    protected void CreateKickbackTimer(){
        kickbackCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, weaponData.kickBackWindowInSeconds);
        kickbackCoroutineContainer.OnCoroutineDisposed += KickbackWindowClosed;
    }

    protected void KickbackWindowClosed(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        kickbackCounter = 0;
        kickbackCoroutineContainer.OnCoroutineDisposed -= KickbackWindowClosed;
        kickbackCoroutineContainer = null;
    }

    protected override void DisposeCoroutineContainers(){
        if(fireRateCoroutineContainer != null){
            fireRateCoroutineContainer.OnCoroutineDisposed -= FireRateCooldownFinished;
            fireRateCoroutineContainer = null;
        }

        if(kickbackCoroutineContainer != null){
            kickbackCoroutineContainer.OnCoroutineDisposed -= KickbackWindowClosed;
            kickbackCoroutineContainer = null;
        }

        if(reloadCoroutineContainer != null){
            ChangeWeaponState(WeaponState.Default);
            reloadCoroutineContainer.OnCoroutineDisposed -= ReloadActionFinished;   
            reloadCoroutineContainer = null;
        }
    }

    protected void UseWeaponResource(){
        weaponResourceData.RemoveItem();    
    }

    protected void SpawnBulletCasing(){
        //Update bullet casing spawn point to be relative to the parent object
        Vector3 worldbulletCasingSpawnPoint = transform.TransformPoint(localBulletCasingSpawnPoint);
        Vector3 worldbulletCasingForceDir = transform.TransformDirection(localBulletCasingForceDir);

        float randomBulletCasingLaunchStrength = Random.Range(bulletCasingForceStrength.minValue, bulletCasingForceStrength.maxValue);

        CreateNewBulletCasing(fearPistolBulletCasing, worldbulletCasingSpawnPoint, worldbulletCasingForceDir, randomBulletCasingLaunchStrength);
    }

    protected RigidbodyDetail CreateNewBulletCasing(RigidbodyDetail casingPrefab, Vector3 spawnPoint, Vector3 forceDir, float forceStrength){
        RigidbodyDetail spawnedCasing = Instantiate(casingPrefab, spawnPoint, transform.rotation);

        spawnedCasing.ApplyImpulseForce(forceDir, forceStrength);

        return spawnedCasing;
    }

    protected FadeObject SpawnBulletDecal(FadeObject decalPrefab, Vector3 hitPoint, Vector3 hitNormal){
        Quaternion rot = Quaternion.LookRotation(-hitNormal);
        FadeObject spawnedDecal = Instantiate(decalPrefab, hitPoint + hitNormal * 0.01f, rot);

        spawnedDecal.transform.Rotate(90, 0, 180);
        
        spawnedDecal.StartFadeTime();

        return spawnedDecal;
    }

    protected virtual void HandleGunShotImpact(Ray ray){
        Debug.DrawRay(ray.origin, ray.direction, Color.white, 5f);
        FadeObject fadeObject;

        if(!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, validHitLayermask, QueryTriggerInteraction.Ignore)) return;

        if(hitInfo.collider.TryGetComponent(out IDamagable damagable)){
            damagable.TakeDamage(weaponData.weaponAttackDamage, playerReference, hitInfo.point);
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

    private void OnDrawGizmos() {
        if(!showGizmos || cameraTransform == null) return;
        Gizmos.color = gizmosColor;

        float bloomAngle = currentWeaponState == WeaponState.Aiming ? weaponData.steadiedBloomAngle : weaponData.baseBloomAngle;

        GizmoShapes.DrawCone(cameraTransform.position, cameraTransform.forward, bloomAngle, gizmosLength, gizmosCircleSides);

        Vector3 bulletCasingSpawnPos = transform.TransformPoint(localBulletCasingSpawnPoint);

        Gizmos.DrawSphere(bulletCasingSpawnPos, 0.1f);
    }
}