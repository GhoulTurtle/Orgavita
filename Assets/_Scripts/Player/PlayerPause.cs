using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerPause : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Required References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private AudioListener playerAudioListener;

    private void Awake() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }

    private void OnDestroy() {
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= UnPausedInput; 
        }
    }

    public void PausedInput(InputAction.CallbackContext context){
        if (context.phase != InputActionPhase.Performed || GameManager.CurrentState != GameState.Game) return;

        PauseGame();
    }

    public void InSubMenu(){
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= UnPausedInput; 
        }
    }

    public void InPauseMenu(){       
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput += UnPausedInput; 
        }
    }

    public void UnPausedInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed || GameManager.CurrentState != GameState.UI) return;

        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= UnPausedInput; 
        }

        UnPauseGame();
    }

    public void UnPauseGame(){
        GameManager.UpdateGameState(GameState.Game);
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;

        AudioListener.pause = false;
    }

    private void PauseGame(){
        GameManager.UpdateGameState(GameState.UI);
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;

        AudioListener.pause = true;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ReturnToMainMenu(){
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame(){
        Time.timeScale = 1;
        Application.Quit();
    }
}