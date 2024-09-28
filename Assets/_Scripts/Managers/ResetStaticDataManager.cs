using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour{
    [Header("Data to Reset")]
    [SerializeField] private PlayerInventorySO playerInventorySO;
    [SerializeField] private QuickSelectDataSO quickSelectDataSO;
    [SerializeField] private List<ResourceDataSO> resourceDataSOList;

    private void Awake() {
        GameManager.ResetStaticData();
        playerInventorySO.GenerateNewInventory();
        for (int i = 0; i < resourceDataSOList.Count; i++){
            resourceDataSOList[i].ResetResourceData();
        }
        quickSelectDataSO.ClearQuickSelectData();

        RoomSceneManager.SetTransitionPoint(null);
    }

    private void OnApplicationQuit() {
        RoomSceneManager.UnsubscribeFromSceneLoadedCallback();
    }
}