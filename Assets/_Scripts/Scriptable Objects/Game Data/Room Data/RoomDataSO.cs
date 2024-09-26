using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Room Data", fileName = "NewRoomDataSO")]
public class RoomDataSO : ScriptableObject{
    public string roomName;
    public SceneField sceneReference;
    public List<ObjectData> roomObjectDataList;

    public string GetSceneName(){
        return sceneReference.SceneName;
    }

    [ContextMenu("Generate Object Data GUIDs")]
    public void GenerateObjectDatasGUID(){
        List<string> guidList = new List<string>();
        for (int i = 0; i < roomObjectDataList.Count; i++){
            string guid = roomObjectDataList[i].GetObjectID();
            while(guidList.Contains(guid)){
                guid = roomObjectDataList[i].GenerateNewObjectGUID();
            }
            guidList.Add(guid);
        }
    }
}