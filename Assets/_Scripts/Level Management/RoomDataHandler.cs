using System.Linq;
using UnityEngine;

public class RoomDataHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private RoomDataSO roomDataSO;

    public static RoomDataHandler Instance;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start() {
        ISaveable[] saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToArray();

        for (int i = 0; i < saveableObjects.Length; i++){
            for (int j = 0; j < roomDataSO.roomObjectDataList.Count; j++){
                if(roomDataSO.roomObjectDataList[j].GetObjectID() == saveableObjects[i].GetObjectID()){
                    saveableObjects[i].LoadData(roomDataSO.roomObjectDataList[j]);
                    break;
                }
            }
        }
    }

    private void OnDestroy() {
        if(Instance == this){
            Instance = null;
        }
    }
    
    [ContextMenu("Update Room Data Objects")]
    public void GetAllSavableObjects(){
        ISaveable[] saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToArray();

        roomDataSO.roomObjectDataList.Clear();

        for (int i = 0; i < saveableObjects.Length; i++){
            Debug.Log(saveableObjects[i]);

            ObjectData objectData = saveableObjects[i].SaveData();

            roomDataSO.roomObjectDataList.Add(objectData);
        }

        roomDataSO.GenerateObjectDatasGUID();
    }


}
