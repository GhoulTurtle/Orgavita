using System;
using System.Collections;
using UnityEngine;

public class PlayerDebugger : MonoBehaviour{
    [Header("Debugger Variables")]
    [SerializeField] private bool setTargetFrameRate = true;
    [SerializeField] private bool showFrameRate = true;
    [SerializeField] private int targetFrameRate = 1;
    [SerializeField] private float frameRateUpdateFrequency = 0.5f;

    public int FramesPerSec { get; protected set; }

    public Action OnFrameRateCalcUpdated;
 
    private void Awake() {  
        if(setTargetFrameRate){
            Application.targetFrameRate = targetFrameRate;
        }
    }

    private void Start(){
        if(showFrameRate){
            StartCoroutine(FPS());
        }
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public bool ShowFPS(){
        return showFrameRate;
    }

    private IEnumerator FPS(){
        while(true){
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSecondsRealtime(frameRateUpdateFrequency);

            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
            OnFrameRateCalcUpdated?.Invoke();
        }
    }
}
