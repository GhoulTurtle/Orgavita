using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Room Data", fileName = "NewRoomDataSO")]
public class RoomDataSO : ScriptableObject{
    public string roomName;
    public List<ObjectData> roomObjectDataList;
    public SceneField sceneReference;

    public string GetSceneName(){
        return sceneReference.sceneName;
    }
}