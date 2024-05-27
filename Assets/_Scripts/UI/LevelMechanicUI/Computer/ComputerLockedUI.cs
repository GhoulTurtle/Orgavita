using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ComputerLockedUI : MonoBehaviour{
    [Header("UI References")]
    [SerializeField] private Transform lockedUIParent;
    [SerializeField] private Slider loadingSliderUI;

    [Header("Loading Variables")]
    [SerializeField] private float minTimeWaiting;        
    [SerializeField] private float maxTimeWaiting;        

    public UnityEvent OnStartUnlock;
    public UnityEvent OnFinishUnlock;

    private void Awake() {
        loadingSliderUI.gameObject.SetActive(false);
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void StartUnlock(){
        OnStartUnlock?.Invoke();
        loadingSliderUI.gameObject.SetActive(true);
        StartCoroutine(UnlockLoadingCoroutine());
    }

    private IEnumerator UnlockLoadingCoroutine(){
        while(loadingSliderUI.value != loadingSliderUI.maxValue){
            loadingSliderUI.value++;

            float randomWaitTime = Random.Range(minTimeWaiting, maxTimeWaiting);

            yield return new WaitForSeconds(randomWaitTime);
        }

        OnFinishUnlock?.Invoke();
        lockedUIParent.gameObject.SetActive(false);
    }
}