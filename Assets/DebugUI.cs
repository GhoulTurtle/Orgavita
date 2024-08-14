using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private PlayerDebugger playerDebugger;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI fpsCounter;

    private void Start() {
        if(playerDebugger == null || !playerDebugger.ShowFPS()){
            fpsCounter.text = "";
            return;
        }

        playerDebugger.OnFrameRateCalcUpdated += UpdateFPSCounter;
    }

    private void OnDestroy() {
        if(playerDebugger.ShowFPS()){
            playerDebugger.OnFrameRateCalcUpdated -= UpdateFPSCounter;
        }
    }

    private void UpdateFPSCounter(){
        fpsCounter.text = "FPS:" + playerDebugger.FramesPerSec;
    }
}