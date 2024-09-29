using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private Health playerHealth;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusImage;
    [SerializeField] private CanvasGroup damageOverlayCanvasGroup;
    [SerializeField] private CanvasGroup healingOverlayCanvasGroup;
    [SerializeField] private GameObject healingUIParent;

    [Header("Health State Variables")]
    [SerializeField] private Color healthyColor = Color.blue;
    [SerializeField] private Color injuredColor = Color.cyan;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    [Header("Health Overlay Variables")]
    [SerializeField] private float damageOverlayFadeInTimeInSeconds = 0.2f;
    [SerializeField] private float damageOverlayFadeOutTimeInSeconds = 3f;
    [SerializeField] private float damageOverlayWarningAlpha = 0.5f;
    [SerializeField] private float damageOverlayCriticalPulseAnimationSpeedInSeconds = 0.75f;
    [SerializeField, Range(0f, 1f)] private float healingOverTimeOverlayAlpha = 0.5f;
    [SerializeField] private float healingOvertimePulseAnimationSpeedInSeconds = 0.75f; 
    [SerializeField] private float healingOverlayFadeInTimeInSeconds = 0.2f;
    [SerializeField] private float healingOverlayFadeOutTimeInSeconds = 0.2f;

    private IEnumerator healingOverlayAnimationCoroutine;
    private IEnumerator damageOverlayAnimationCoroutine;

    private void Awake() {
        playerHealth.OnHealthStateChanged += UpdateHealthUI;
        playerHealth.OnStartHealingOverTime += ShowHealingOvertimeUI;
        playerHealth.OnFinishedHealingOverTime += HideHealingOvertimeUI;
        playerHealth.OnHealedEvent += ShowInstantHealUI;
        playerHealth.OnDamagedEvent += ShowDamagedUI;

        healingUIParent.SetActive(false);

        healingOverlayCanvasGroup.alpha = 0f;
        damageOverlayCanvasGroup.alpha = 0f;
    }

    private void OnDestroy() {
        playerHealth.OnHealthStateChanged -= UpdateHealthUI;
        playerHealth.OnStartHealingOverTime -= ShowHealingOvertimeUI;
        playerHealth.OnFinishedHealingOverTime -= HideHealingOvertimeUI;
        playerHealth.OnHealedEvent -= ShowInstantHealUI;
        playerHealth.OnDamagedEvent -= ShowDamagedUI;

        StopAllCoroutines();
    }

    private void UpdateHealthUI(object sender, Health.HealthStateChangedEventArgs e){
        switch (e.healthState){
            case HealthState.Healthy:
                statusText.text = "HEALTHY";
                statusImage.color = healthyColor;
                break;
            case HealthState.Injured:
                statusText.text = "INJURED";
                statusImage.color = injuredColor;
                break;
            case HealthState.Warning:
                statusText.text = "WARNING";
                statusImage.color = warningColor;
                break;
            case HealthState.Critical:
                statusText.text = "CRITICAL";
                statusImage.color = criticalColor;
                break;
        }
    }

    private void ShowHealingOvertimeUI(){
        healingUIParent.SetActive(true);
        StartHealingOverlayFadeInAnimation();
    }

    private void ShowInstantHealUI(object sender, EventArgs e){
        StartHealingOverlayFadeInAnimation();
    }

    private void ShowDamagedUI(){
        StartDamageOverlayFadeInAnimation();
    }

    private void OnHealingFadeInFinished(){
        if(playerHealth.IsActiveHealOverTimeJob()){
            //Reduce the alpha to the healingOverTimeOverlayAlpha
            StartHealingOverlayOvertimePulseAlphaAnimation();
        }
        else{
            StartHealingOverlayFadeOutAnimation();
        }
    }

    private void OnDamageFadeInFinished(){
        switch (playerHealth.GetCurrentHealthState()){
            case HealthState.Healthy: StartDamageOverlayFadeOutAnimation();
                break;
            case HealthState.Injured: StartDamageOverlayFadeOutAnimation();
                break;
            case HealthState.Warning:
                break;
            case HealthState.Critical:
                break;
        }
    }

    private void StartHealingOverlayFadeInAnimation(){
        StartOverlayFadeInAnimation(false, healingOverlayFadeInTimeInSeconds);
    }

    private void StartHealingOverlayFadeOutAnimation(){
        StartOverlayFadeOutAnimation(false, healingOverlayFadeOutTimeInSeconds);
    }

    private void StartHealingOverlayOvertimePulseAlphaAnimation(){
        StopHealingOverlayAnimation();
        healingOverlayAnimationCoroutine = UIAnimator.CanvasGroupAlphaPulseCoroutine(healingOverlayCanvasGroup, 1f, healingOverTimeOverlayAlpha, healingOvertimePulseAnimationSpeedInSeconds);

        StartCoroutine(healingOverlayAnimationCoroutine);
    }

    private void StartDamageOverlayFadeInAnimation(){
        StartOverlayFadeInAnimation(transform, damageOverlayFadeInTimeInSeconds);
    }

    private void StartDamageOverlayFadeOutAnimation(){
        StartOverlayFadeOutAnimation(true, damageOverlayFadeOutTimeInSeconds);
    }

    private void StartOverlayFadeInAnimation(bool isDamage, float animationTime){
        if(!isDamage && playerHealth.IsActiveHealOverTimeJob()) return;

        if(isDamage){
            StopDamageOverlayAnimation();
        }
        else{
            StopHealingOverlayAnimation();
        }

        IEnumerator coroutine = isDamage ? damageOverlayAnimationCoroutine : healingOverlayAnimationCoroutine;
        CanvasGroup canvasGroup = isDamage ? damageOverlayCanvasGroup : healingOverlayCanvasGroup;
        Action action = isDamage ? OnDamageFadeInFinished : OnHealingFadeInFinished;

        coroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(canvasGroup, animationTime, canvasGroup.alpha, 1f, true, action);
        
        StartCoroutine(coroutine);
    }

    private void StartOverlayFadeOutAnimation(bool isDamage, float animationTime){
        if(isDamage){
            StopDamageOverlayAnimation();
        }
        else{
            StopHealingOverlayAnimation();
        }

        IEnumerator coroutine = isDamage ? damageOverlayAnimationCoroutine : healingOverlayAnimationCoroutine;
        CanvasGroup canvasGroup = isDamage ? damageOverlayCanvasGroup : healingOverlayCanvasGroup;

        coroutine = UIAnimator.CanvasGroupAlphaFadeCoroutine(canvasGroup, animationTime, canvasGroup.alpha, 0f);
        StartCoroutine(coroutine);
    }

    private void StopHealingOverlayAnimation(){
        if(healingOverlayAnimationCoroutine != null){
            StopCoroutine(healingOverlayAnimationCoroutine);
            healingOverlayAnimationCoroutine = null;
        }
    }

    private void StopDamageOverlayAnimation(){
        if(damageOverlayAnimationCoroutine != null){
            StopCoroutine(damageOverlayAnimationCoroutine);
            damageOverlayAnimationCoroutine = null;
        }
    }

    private void HideHealingOvertimeUI(){
        healingUIParent.SetActive(false);
        StartHealingOverlayFadeOutAnimation();
    }
}