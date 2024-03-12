using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour{
    public void LoadLevel(string levelName){
        SceneManager.LoadScene(levelName);
    }

    public void Quit(){
        Application.Quit();
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
    }

}
