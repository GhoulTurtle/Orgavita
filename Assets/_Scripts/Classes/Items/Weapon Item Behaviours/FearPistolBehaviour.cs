using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FearPistolBehaviour : EquippedItemBehaviour{
    [Header("Required References")]
    [SerializeField] private ResourceDataSO fearPistolResourceData;
    [SerializeField] private WeaponDataSO fearPistolWeaponData;

    [Header("Pistol Behaviour Events")]
    [SerializeField] private UnityEvent OnEmptyGunTriggered;

    [Header("Debugging")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmosColor = Color.green;
    [SerializeField, Range(0.5f, 3f)] private float gizmosLength = 1f;
    [SerializeField, Range(3, 100f)] private int gizmosCircleSides = 36;

    private Transform cameraTransform;
    
    public override void SaveData(){
        playerInputHandler.OnHolsterWeapon -= HolsterWeaponInput;
    }

    public override void SetupItemBehaviour(InventoryItem _inventoryItem, PlayerInputHandler _playerInputHandler){
        base.SetupItemBehaviour(_inventoryItem, _playerInputHandler);
        playerInputHandler.OnHolsterWeapon += HolsterWeaponInput;
        cameraTransform = Camera.main.transform; //TO-DO: Refactor to not grab the main camera everytime. Might not be a big deal but could cause potential issues later
    }

    public override void HolsterWeaponInput(object sender, InputEventArgs e){
        base.HolsterWeaponInput(sender, e);
    }

    public override void WeaponUseInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        if(fearPistolResourceData.IsEmpty()){
            OnEmptyGunTriggered?.Invoke();
            //Reload weapon if ammo is in inventory
            return;
        }

        Vector3 bloom = GunCalculation.CalculateBloom(fearPistolWeaponData.baseBloomAngle, cameraTransform.position, cameraTransform.forward);

        Debug.DrawRay(cameraTransform.position, bloom, Color.white, 5f);
    }

    public override void WeaponAltUseInput(object sender, InputEventArgs e){

    }
    
    public override void ReloadInput(object sender, InputEventArgs e){

    }

    public override void InspectInput(object sender, InputEventArgs e){
        
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

    public override ResourceDataSO GetEquippedItemResourceData(){
        return fearPistolResourceData;
    }

    private void OnDrawGizmos() {
        if(!showGizmos || cameraTransform == null) return;
        Gizmos.color = gizmosColor;

        GizmoShapes.DrawCone(cameraTransform.position, cameraTransform.forward, fearPistolWeaponData.baseBloomAngle, gizmosLength, gizmosCircleSides);
    }
}