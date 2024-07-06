using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FearPistolBehaviour : EquippedItemBehaviour{
    [Header("Required References")]
    [SerializeField] private ResourceDataSO fearPistolResourceData;
    [SerializeField] private FearPistolWeaponDataSO fearPistolWeaponData;
    [SerializeField] private TerrainDataList validBounceableTerrainDataList;
    [SerializeField] private TerrainAudioDataList impactSoundTerrainAudioDataList;
    [SerializeField] private LayerMask validHitLayermask;
    
    [Header("Visual References")]
    [SerializeField] private FadeObject fearPistolBulletDecalHole;
    [SerializeField] private FadeObject fearPistolBulletDecalDent;

    [Header("Pistol Behaviour Events")]
    [SerializeField] private UnityEvent<TerrainType, AudioSource> OnTerrainImpacted;
    [SerializeField] private UnityEvent<AudioSource> OnBulletBounced;
    [SerializeField] private UnityEvent OnEmptyGunTriggered;
    [SerializeField] private UnityEvent OnGunFired;
    [SerializeField] private UnityEvent OnGunAimed;
    [SerializeField] private UnityEvent OnGunInspected;

    [Header("Debugging")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmosColor = Color.green;
    [SerializeField, Range(0.5f, 3f)] private float gizmosLength = 1f;
    [SerializeField, Range(3, 100f)] private int gizmosCircleSides = 36;

    private CoroutineContainer kickbackCoroutineContainer;
    private CoroutineContainer fireRateCoroutineContainer;
    private CoroutineContainer reloadCoroutineContainer;
    
    private int kickbackCounter;

    public override void SaveData(){
        DisposeCoroutineContainers();
        playerInputHandler.OnHolsterWeapon -= HolsterWeaponInput;
    }

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler);
        playerInputHandler.OnHolsterWeapon += HolsterWeaponInput;
    }

    public override void HolsterWeaponInput(object sender, InputEventArgs e){
        DisposeCoroutineContainers();

        base.HolsterWeaponInput(sender, e);
    }

    public override void WeaponUseInput(object sender, InputEventArgs e){
        if (currentWeaponState == WeaponState.Inspecting || currentWeaponState == WeaponState.Reloading) return;
        if (e.inputActionPhase != InputActionPhase.Performed) return;
        if (fireRateCoroutineContainer != null) return;

        if (fearPistolResourceData.IsEmpty())
        {
            OnEmptyGunTriggered?.Invoke();
            //Reload weapon if ammo is in inventory
            return;
        }

        float bloomAngle = currentWeaponState == WeaponState.Aiming ? fearPistolWeaponData.steadiedBloomAngle : fearPistolWeaponData.baseBloomAngle;

        Vector3 bloom = WeaponHelper.CalculateBloom(bloomAngle, cameraTransform.position, cameraTransform.forward);
        Ray firedRay = new(cameraTransform.position, bloom);

        OnWeaponUse?.Invoke(this, EventArgs.Empty);
        OnGunFired?.Invoke();

        HandleGunShotImpact(firedRay, 0);
        FireRateCooldown();
        AttemptTriggerKickback();
    }

    public override void WeaponAltUseInput(object sender, InputEventArgs e){
        if(currentWeaponState == WeaponState.Inspecting || currentWeaponState == WeaponState.Reloading) return;

        if(e.inputActionPhase == InputActionPhase.Performed){
            ChangeWeaponState(WeaponState.Aiming);
            OnWeaponAltUse?.Invoke(this, EventArgs.Empty);
            OnGunAimed?.Invoke();
        }
        else if(e.inputActionPhase == InputActionPhase.Canceled){
            ChangeWeaponState(WeaponState.Default);
            OnWeaponAltCancel?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public override void ReloadInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed || currentWeaponState == WeaponState.Reloading || fearPistolResourceData.IsFull()) return;
        ChangeWeaponState(WeaponState.Reloading);
        reloadCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, fearPistolWeaponData.weaponReloadTimeInSeconds);

        reloadCoroutineContainer.OnCoroutineDisposed += ReloadActionFinished;

        OnReload?.Invoke(this, EventArgs.Empty);
    }

    public override void InspectInput(object sender, InputEventArgs e){
        if(currentWeaponState == WeaponState.Reloading) return;
        if(e.inputActionPhase == InputActionPhase.Performed){
            ChangeWeaponState(WeaponState.Inspecting);
            OnInspectUse?.Invoke(this, EventArgs.Empty);
            OnGunInspected?.Invoke();
        }
        else if(e.inputActionPhase == InputActionPhase.Canceled){
            ChangeWeaponState(WeaponState.Default);
            OnInspectCanceled?.Invoke(this, EventArgs.Empty);
        }
    }

    public override ResourceDataSO GetEquippedItemResourceData(){
        return fearPistolResourceData;
    }

    public override WeaponDataSO GetEquippedWeaponData(){
        return fearPistolWeaponData;
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

    private void HandleGunShotImpact(Ray ray, int rayBounceCounter){
        Debug.DrawRay(ray.origin, ray.direction, Color.white, 5f);
        FadeObject fadeObject;

        if(!Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, validHitLayermask, QueryTriggerInteraction.Ignore)) return;

        if(hitInfo.collider.TryGetComponent(out IDamagable damagable)){
            damagable.TakeDamage(fearPistolWeaponData.weaponAttackDamage);
            //TO-DO: Figure out what we hit then spawn the right vfx and play the right impact sound.
            return;
        }

        if(hitInfo.collider.TryGetComponent(out Terrain terrain)){
            TerrainType terrainType = terrain.GetTerrainType();
            if(rayBounceCounter < fearPistolWeaponData.maxBounceCount && validBounceableTerrainDataList.IsEnteredTerrainValid(terrainType)){
                BounceFearShot(hitInfo, ray, rayBounceCounter);
                //TO-DO: Play fear pistol bounce sfx, play spark vfx
                fadeObject = SpawnBulletDecal(fearPistolBulletDecalDent, hitInfo.point, hitInfo.normal);

                OnBulletBounced?.Invoke(fadeObject.GetFadeObjectAudioSource());
                return;
            }

            fadeObject = SpawnBulletDecal(fearPistolBulletDecalHole, hitInfo.point, hitInfo.normal);
            OnTerrainImpacted?.Invoke(terrainType, fadeObject.GetFadeObjectAudioSource());
        }

        fadeObject = SpawnBulletDecal(fearPistolBulletDecalHole, hitInfo.point, hitInfo.normal);
        OnTerrainImpacted?.Invoke(TerrainType.None, fadeObject.GetFadeObjectAudioSource());
    }

    private FadeObject SpawnBulletDecal(FadeObject decalPrefab, Vector3 hitPoint, Vector3 hitNormal){
        Quaternion rot = Quaternion.LookRotation(-hitNormal);
        FadeObject spawnedDecal = Instantiate(decalPrefab, hitPoint + hitNormal * 0.01f, rot);

        spawnedDecal.transform.Rotate(90, 0, 180);
        
        spawnedDecal.StartFadeTime();

        return spawnedDecal;
    }

    private void TriggerTerrainImpactEvent(FadeObject decalObject, TerrainType terrainType){
        AudioSource decalAudioSource = decalObject.GetFadeObjectAudioSource();
        if(decalAudioSource != null){
            OnTerrainImpacted?.Invoke(terrainType, decalAudioSource);
        }   
    }

    private void BounceFearShot(RaycastHit hitInfo, Ray ray, int rayBounceCounter){
        rayBounceCounter++;

        Vector3 rayPoint = hitInfo.point;
        Vector3 rayNorm = hitInfo.normal;
        Vector3 rayDir = ray.direction;

        Vector3 reflectedVector = rayDir - 2 * Vector3.Dot(rayDir, rayNorm) * rayNorm;

        ray.origin = rayPoint;
        ray.direction = reflectedVector;

        HandleGunShotImpact(ray, rayBounceCounter);
    }

    private void FireRateCooldown(){
        fireRateCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, fearPistolWeaponData.weaponFireRateInSeconds);
        fireRateCoroutineContainer.OnCoroutineDisposed += FireRateCooldownFinished;
    }

    private void AttemptTriggerKickback(){
        if(fearPistolWeaponData.minKickBackShotAmount == 0) return;

        kickbackCounter++;

        if(kickbackCounter >= fearPistolWeaponData.minKickBackShotAmount){
            TriggerKickback();
        }

        if(kickbackCoroutineContainer != null){
            RemoveKickbackTimer();
        }

        CreateKickbackTimer();
    }

    private void RemoveKickbackTimer(){
        kickbackCoroutineContainer.TryStopCoroutine();
        kickbackCoroutineContainer.OnCoroutineDisposed -= KickbackWindowClosed;
        kickbackCoroutineContainer = null;
    }

    private void CreateKickbackTimer(){
        kickbackCoroutineContainer = WeaponHelper.StartNewWeaponCoroutine(this, fearPistolWeaponData.kickBackWindowInSeconds);
        kickbackCoroutineContainer.OnCoroutineDisposed += KickbackWindowClosed;
    }

    private void TriggerKickback(){
        OnKickbackApplied?.Invoke(this, new KickbackAppliedEventArgs(fearPistolWeaponData.kickBackAmount));
    }

    private void FireRateCooldownFinished(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        fireRateCoroutineContainer.OnCoroutineDisposed -= FireRateCooldownFinished;
        fireRateCoroutineContainer = null;
    }

    private void KickbackWindowClosed(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        kickbackCounter = 0;
        kickbackCoroutineContainer.OnCoroutineDisposed -= KickbackWindowClosed;
        kickbackCoroutineContainer = null;
    }

    private void ReloadActionFinished(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        ChangeWeaponState(WeaponState.Default);
        //TO-DO: Fill resource ammo here.

        reloadCoroutineContainer.OnCoroutineDisposed -= ReloadActionFinished;   
        reloadCoroutineContainer = null;
    }

    private void OnDrawGizmos() {
        if(!showGizmos || cameraTransform == null) return;
        Gizmos.color = gizmosColor;

        float bloomAngle = currentWeaponState == WeaponState.Aiming ? fearPistolWeaponData.steadiedBloomAngle : fearPistolWeaponData.baseBloomAngle;

        GizmoShapes.DrawCone(cameraTransform.position, cameraTransform.forward, bloomAngle, gizmosLength, gizmosCircleSides);
    }
}