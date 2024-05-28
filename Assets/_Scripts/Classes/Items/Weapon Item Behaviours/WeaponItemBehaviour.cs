using UnityEngine;

public abstract class WeaponItemBehaviour : MonoBehaviour{
    [SerializeField] private PlayerItem weaponPlayerItem;

    private PlayerEquippedItemHandler playerEquippedItemHandler;

    private void Awake() {
        weaponPlayerItem = new PlayerItem(transform);
        weaponPlayerItem.OnItemStateChanged += EvaulateItemStateChange;
    }

    private void OnDestroy() {
        weaponPlayerItem.OnItemStateChanged -= EvaulateItemStateChange;
        StopAllCoroutines();
    }

    public void SaveData(){

    }

    public void SetupItemBehaviour(PlayerEquippedItemHandler _playerEquippedItemHandler){
        playerEquippedItemHandler = _playerEquippedItemHandler;
    }

    public PlayerItem GetPlayerItem() {
        return weaponPlayerItem; 
    }

    private void EvaulateItemStateChange(object sender, PlayerItem.ItemStateChangedEventArgs e){
        
    }
}
