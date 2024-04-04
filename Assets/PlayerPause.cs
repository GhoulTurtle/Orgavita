using UnityEngine;

public class PlayerPause : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Awake() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void PauseInput(){
        if(isPaused){
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else{
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
