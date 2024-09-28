using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomDataSO))]
public class RoomDataEditor : Editor{
    public override void OnInspectorGUI(){
        // Get reference to the RoomData scriptable object
        RoomDataSO roomData = (RoomDataSO)target;

        // Start the custom inspector GUI block
        EditorGUILayout.BeginVertical();

        // Display and edit the Room Name
        EditorGUILayout.LabelField("Room Information", EditorStyles.boldLabel);
        roomData.roomName = EditorGUILayout.TextField("Room Name", roomData.roomName);

        // Display and edit the SceneField (sceneReference)
        EditorGUILayout.Space();  // Adds some spacing
        EditorGUILayout.LabelField("Scene Reference", EditorStyles.boldLabel);
        roomData.sceneReference.sceneAsset = EditorGUILayout.ObjectField(
            "Scene Asset", 
            roomData.sceneReference.sceneAsset, 
            typeof(SceneAsset), 
            false
        );

        // Automatically update the scene name based on the selected SceneAsset
        if (roomData.sceneReference.sceneAsset != null){
            roomData.sceneReference.sceneName = roomData.sceneReference.sceneAsset.name;
            EditorGUILayout.LabelField("Scene Name", roomData.sceneReference.sceneName);
        }
        else{
            EditorGUILayout.LabelField("Scene Name", "No scene selected");
        }

        // Display the Room Object Data List
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Room Objects", EditorStyles.boldLabel);

        // Display the list of objects in the room
        if (roomData.roomObjectDataList != null && roomData.roomObjectDataList.Count > 0){
            for (int i = 0; i < roomData.roomObjectDataList.Count; i++){
                //TO-DO: Make this a drop down to view the room object variables
                EditorGUILayout.LabelField($"Object {i + 1}: {roomData.roomObjectDataList[i].objectID}");
                
            }
        }
        else{
            EditorGUILayout.LabelField("No objects in the room.");
        }

        // Provide a button to clear or reset the list, or perform other operations
        if (GUILayout.Button("Clear Room Objects")){
            roomData.roomObjectDataList.Clear();
        }

        // End the custom inspector GUI block
        EditorGUILayout.EndVertical();

        // Apply any changes made in the editor
        if (GUI.changed){
            EditorUtility.SetDirty(roomData);
        }
    }
}
