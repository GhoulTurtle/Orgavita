using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LightFlickerController : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private Light lightToFlicker;
    [SerializeField] private bool isFlickering; 

    [Header("Light Flicker Controller ")]
    [SerializeField] private float minWaitTimeInSeconds;
    [SerializeField] private float maxWaitTimeInSeconds;

    [Header("Events")]
    public UnityEvent OnLightGoOut;
    public UnityEvent OnLightTurnOn;

    private IEnumerator currentFlickeringCoroutine;

    private void Awake() {
        if(lightToFlicker.enabled && isFlickering){
            StartFlickeringCoroutine();
        }
    }

    private void OnDestroy() {
        StopAllCoroutines();
    }

    public void LightGoOut(){
        StopFlickeringCoroutine();
        lightToFlicker.enabled = false;
        OnLightGoOut?.Invoke();
    }

    public void LightTurnOn(){
        lightToFlicker.enabled = true;
        OnLightTurnOn?.Invoke();
        StartFlickeringCoroutine();
    }

    public void ToggleIsFlickering(bool state){
        isFlickering = state;

        if(isFlickering){
            StartFlickeringCoroutine();
            return;
        }

        StopFlickeringCoroutine();
    }

    public void StartControlledLightFlicker(float timeToFlicker){
        StopFlickeringCoroutine();

        currentFlickeringCoroutine = ControlledLightFlickerCoroutine(timeToFlicker);

        StartCoroutine(currentFlickeringCoroutine);
    }

    private void StartFlickeringCoroutine(){
        if(isFlickering){
            currentFlickeringCoroutine = LightFlickerCoroutine();
            StartCoroutine(currentFlickeringCoroutine);
        }
    }

    private void StopFlickeringCoroutine(){
        if(currentFlickeringCoroutine != null){
            StopCoroutine(currentFlickeringCoroutine);
            currentFlickeringCoroutine = null;
        }
    }

    private IEnumerator ControlledLightFlickerCoroutine(float timeToFlicker){
        lightToFlicker.enabled = true;
        yield return new WaitForSeconds(timeToFlicker);
        lightToFlicker.enabled = false;
    }

    private IEnumerator LightFlickerCoroutine(){
        lightToFlicker.enabled = true;
        OnLightTurnOn?.Invoke();
        float timeDelay = Random.Range(minWaitTimeInSeconds, maxWaitTimeInSeconds);
        yield return new WaitForSeconds(timeDelay);
        
        lightToFlicker.enabled = false;
        OnLightGoOut?.Invoke();
        float timeDelay2 = Random.Range(minWaitTimeInSeconds, maxWaitTimeInSeconds);
        yield return new WaitForSeconds(timeDelay2);

        currentFlickeringCoroutine = null;
        StartFlickeringCoroutine();
    }
}
