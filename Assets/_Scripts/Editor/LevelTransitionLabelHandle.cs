using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelTransition))]
public class LevelTransitionLabelHandle : Editor{
    private static GUIStyle labelStyle;

    private void OnEnable() {
        labelStyle = new GUIStyle();    
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = 25;
    }

    private void OnSceneGUI() {
        LevelTransition levelTransition = (LevelTransition)target;

        Handles.BeginGUI();
        Handles.Label(levelTransition.transform.position + new Vector3(0, 3, 0), "Transition Point: " + levelTransition.TransitionPointToSpawn.ToString(), labelStyle);
        Handles.EndGUI();
    }
}
