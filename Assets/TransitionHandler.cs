using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class TransitionHandler : MonoBehaviour{
    [Header("Required Reference")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Transition Variables")]
    [SerializeField] private FadeTime defaultFadeTime;

    private IEnumerator fadeUIAnimationCoroutine;
    private IEnumerator fadeAudioAnimationCoroutine;

    public static TransitionHandler Instance;

    private void Start() {
        if(Instance == null){
            Instance = this;
            canvasGroup.alpha = 0f;        
        }
        else{
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void StartFadeOut(Action OnFadeOutCompletedCallback, FadeTime fadeAnimationTime){
        StopCurrentFadeAnimation();
        
        float animationTime = fadeAnimationTime.FadeOutTime == -1 ? defaultFadeTime.FadeOutTime : fadeAnimationTime.FadeOutTime;

        fadeUIAnimationCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(canvasGroup, animationTime, 1f, 0f, true, OnFadeOutCompletedCallback);
        StartCoroutine(fadeUIAnimationCoroutine);

        fadeAudioAnimationCoroutine = AudioConductor.AudioFadeAnimationCoroutine(audioMixer, AudioGroupType.Master, 0, animationTime);
        StartCoroutine(fadeAudioAnimationCoroutine);
    }

    public void StartFadeIn(Action OnFadeInCompletedCallback, FadeTime fadeAnimationTime){
        StopCurrentFadeAnimation();

        float animationTime = fadeAnimationTime.FadeInTime == -1 ? defaultFadeTime.FadeInTime : fadeAnimationTime.FadeInTime;

        fadeUIAnimationCoroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(canvasGroup, animationTime, 0f, 1f, false, OnFadeInCompletedCallback);
        StartCoroutine(fadeUIAnimationCoroutine);

        fadeAudioAnimationCoroutine = AudioConductor.AudioFadeAnimationCoroutine(audioMixer, AudioGroupType.Master, -80f, animationTime, false);
        StartCoroutine(fadeAudioAnimationCoroutine); 
    }

    private void StopCurrentFadeAnimation(){
        if(fadeUIAnimationCoroutine != null){
            StopCoroutine(fadeUIAnimationCoroutine);
            fadeUIAnimationCoroutine = null;
        }

        if(fadeAudioAnimationCoroutine != null){
            StopCoroutine(fadeAudioAnimationCoroutine);
            fadeAudioAnimationCoroutine = null;
        }
    }
}
