using UnityEngine;
using UnityEngine.InputSystem;

public class CrossbowBehaviour : GunWeaponEquippedItemBehaviour{
    [Header("Crossbow References")]
    [SerializeField] private WeaponProjectile arrowProjectile;
    [SerializeField] private Vector3 arrowSpawnPos;

    private CrossbowWeaponDataSO crossbowWeaponDataSO;

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler, PlayerInventoryHandler _playerInventoryHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler, _playerInventoryHandler);
        crossbowWeaponDataSO = (CrossbowWeaponDataSO)weaponData;
    }

    public override void WeaponUseInput(object sender, InputEventArgs e){
        if(currentWeaponState == WeaponState.Inspecting) return;
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        if(fireRateCoroutineContainer != null) return;

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

        Vector3 worldArrowSpawnPoint = transform.TransformPoint(arrowSpawnPos);

        Quaternion shootQuaternion = Quaternion.LookRotation(cameraTransform.forward);

        WeaponProjectile spawnedProjectile = Instantiate(arrowProjectile, worldArrowSpawnPoint, shootQuaternion);
        spawnedProjectile.SetupProjectile(cameraTransform.forward, crossbowWeaponDataSO, validHitLayermask);
        
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

    private void OnDrawGizmos() {
        if(!showGizmos || cameraTransform == null) return;
        Gizmos.color = gizmosColor;

        float bloomAngle = currentWeaponState == WeaponState.Aiming ? weaponData.steadiedBloomAngle : weaponData.baseBloomAngle;

        GizmoShapes.DrawCone(cameraTransform.position, cameraTransform.forward, bloomAngle, gizmosLength, gizmosCircleSides);

        Vector3 worldArrowSpawnPoint = transform.TransformPoint(arrowSpawnPos);

        Gizmos.DrawSphere(worldArrowSpawnPoint, 0.1f);
    }
}
