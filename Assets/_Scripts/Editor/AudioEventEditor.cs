

/// <summary>
/// Code taken from Unite 2016 Talk "Overthrowing the MonoBehaviour Tyranny in a Glorious Scriptable Object Revolution"
/// Original code belongs to Richard Fine, taken from GitHub - https://github.com/richard-fine/scriptable-object-demo/tree/main
/// </summary>

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor
{

	[SerializeField] private AudioSource _previewer;

	public void OnEnable(){
		_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	public void OnDisable(){
		DestroyImmediate(_previewer.gameObject);
	}

	public override void OnInspectorGUI(){
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Preview")){
			((AudioEvent) target).Play(_previewer);
		}
		EditorGUI.EndDisabledGroup();
	}
}
