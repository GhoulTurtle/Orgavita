using UnityEngine;

public abstract class EmergencyItemBehaviour : MonoBehaviour{
    [SerializeField] private PlayerItem emergencyPlayerItem;

    private PlayerEquippedItemHandler playerEquippedItemHandler;

    private void Awake() {
        emergencyPlayerItem = new PlayerItem(transform);
        emergencyPlayerItem.OnItemStateChanged += EvaulateItemStateChange;
    }

    private void OnDestroy() {
        emergencyPlayerItem.OnItemStateChanged -= EvaulateItemStateChange;
        StopAllCoroutines();
    }

    public void SaveData(){
        
    }

    public void SetupItemBehaviour(PlayerEquippedItemHandler _playerEquippedItemHandler){
        playerEquippedItemHandler = _playerEquippedItemHandler;
    }

    public PlayerItem GetPlayerItem() {
        return emergencyPlayerItem; 
    }

    private void EvaulateItemStateChange(object sender, PlayerItem.ItemStateChangedEventArgs e){
        
    }
}
