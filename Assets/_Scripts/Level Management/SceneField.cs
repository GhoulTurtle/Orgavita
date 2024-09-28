using UnityEngine;

[System.Serializable]
public class SceneField{
	public Object sceneAsset;

	public string sceneName = "";

	// makes it work with the existing Unity methods (LoadLevel/LoadScene)
	public static implicit operator string(SceneField sceneField){
		return sceneField.sceneName;
	}
}