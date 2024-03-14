using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChoiceInteractable))]
public class ChoiceInteractableEditor : Editor{
    private SerializedProperty choiceDialogueSO;

    private SerializedProperty ChoiceEvents;

    private void OnEnable() {
        choiceDialogueSO = serializedObject.FindProperty("choiceDialogueSO");
        ChoiceEvents = serializedObject.FindProperty("ChoiceEvents");
    }

    public override void OnInspectorGUI(){
        serializedObject.Update();
        ChoiceInteractable choiceInteractable = (ChoiceInteractable)target;

        EditorGUILayout.PropertyField(choiceDialogueSO, true);

        if(GUILayout.Button("Update Choice Unity Events")){
            choiceInteractable.UpdateUnityChoiceEvents();
        }        

        EditorGUILayout.PropertyField(ChoiceEvents, true);

        serializedObject.ApplyModifiedProperties();
    }
}
