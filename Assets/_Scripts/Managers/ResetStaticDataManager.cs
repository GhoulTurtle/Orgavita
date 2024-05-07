using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour{
    [SerializeField] private PlayerInventorySO playerInventorySO;

    private void Awake() {
        GameManager.ResetStaticData();
        playerInventorySO.GenerateNewInventory();
    }
}