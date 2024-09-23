using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueEventHandler))]
public class DialogueEventHandlerEditor : Editor{
    public override void OnInspectorGUI(){
        DrawDefaultInspector();

        DialogueEventHandler responseEvents = (DialogueEventHandler)target;

        if (GUILayout.Button("Refresh")){
            responseEvents.OnValidate();
        }
    }
}
