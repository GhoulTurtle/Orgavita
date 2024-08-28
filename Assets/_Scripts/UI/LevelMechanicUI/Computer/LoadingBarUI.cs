using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingBarUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Slider loadingBarSliderUI;

    [Header("Loading Bar Variables")]
    [SerializeField] private float defaultMinTimeWaiting;
    [SerializeField] private float defaultMaxTimeWaiting;
    [SerializeField] private float stutterChance = 0.25f;
    [SerializeField] private float minStutterTimeInSeconds = 0.3f;
    [SerializeField] private float maxStutterTimeInSeconds = 3f;

    public UnityEvent OnStartUnlock;
    public UnityEvent OnFinishUnlock;   
    public Action OnFinishAction;

    private bool isLoading = false;

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void StartLoading(float minTimeWaiting = -1, float maxTimeWaiting = -1){
        if(isLoading) return;
        OnStartUnlock?.Invoke();
        float minTime = minTimeWaiting == -1 ? defaultMinTimeWaiting : minTimeWaiting;
        float maxTime = maxTimeWaiting == -1 ? defaultMinTimeWaiting : maxTimeWaiting;
        StartCoroutine(LoadingCoroutine(minTime, maxTime));
    }

    private IEnumerator LoadingCoroutine(float minTimeWaiting, float maxTimeWaiting){
        isLoading = true;

        loadingBarSliderUI.value = 0;
        float totalRange = Random.Range(minTimeWaiting, maxTimeWaiting);
        float elapsedTime = 0;
        float increment = loadingBarSliderUI.maxValue / totalRange;

        while(loadingBarSliderUI.value < loadingBarSliderUI.maxValue){
            yield return null;
            elapsedTime += Time.deltaTime;
            
            loadingBarSliderUI.value = Mathf.Clamp(increment * elapsedTime, 0, loadingBarSliderUI.maxValue);

            if (Random.value < stutterChance){
                float stutterDuration = Random.Range(minStutterTimeInSeconds, maxStutterTimeInSeconds);
                yield return new WaitForSeconds(stutterDuration);
            }
        }

        OnFinishUnlock?.Invoke();
        OnFinishAction?.Invoke();

        isLoading = false;
    }
}