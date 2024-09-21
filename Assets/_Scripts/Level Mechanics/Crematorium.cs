using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Crematorium : StateChangeInteractable{
    public override string InteractionPrompt => interactionPrompt;

    [Header("Required References")]
    [SerializeField] private CrematoriumUI crematoriumUI;
    [SerializeField] private List<CrematoriumSlider> sliderList = new List<CrematoriumSlider>();
    [SerializeField] private List<CrematoriumPressZone> pressZones = new List<CrematoriumPressZone>();

    [Header("Crematorium Variables")]
    [SerializeField] private string interactionPrompt = "Activate";
    [SerializeField] private float buttonCooldownTime = 0.1f;

    [Header("Events")]
    public UnityEvent OnInvalidButtonPress;
    public UnityEvent OnValidButtonPress; 

    private bool placedButton = false;
    private bool placedCoolent = false;

    private int currentSliderIndex = 0;

    private IEnumerator currentSliderDrainCoroutine;
    private IEnumerator currentSliderResetCoroutine;
    private IEnumerator currentButtonCooldownCoroutine;

    private Slider currentSlider;
    private CrematoriumPressZone currentPressZone;

    private const float SLIDER_RESET_SPEED = 25f;
    private int MAX_SLIDER_INDEX = 4;

    private float currentDrainRate;

    public override void EnterState(){
        base.EnterState();

        playerInputHandler.OnAcceptInput += AcceptInput;
    }

    public override void ExitState(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed) return;
        base.ExitState(sender, e);

        playerInputHandler.OnAcceptInput -= AcceptInput;
    }

    private void OnDestroy() {
        if(playerInputHandler != null){
            playerInputHandler.OnCancelInput -= ExitState;
            playerInputHandler.OnAcceptInput -= AcceptInput;
        }

        StopAllCoroutines();
    }

    public override void UnlockInteractable(){
        crematoriumUI.UpdateNotificationText(placedButton, placedCoolent);

        if(!placedButton || !placedCoolent) return;

        base.UnlockInteractable();
    }

    public void PlacedButton(){
        placedButton = true;

        UnlockInteractable();
    }

    public void PlacedCoolent(){
        placedCoolent = true;
    
        UnlockInteractable();
    }
    
    private void AcceptInput(object sender, InputEventArgs e){
        if(e.inputActionPhase != InputActionPhase.Performed || currentButtonCooldownCoroutine != null) return;

        currentButtonCooldownCoroutine = ButtonCooldownCoroutine();
        StartCoroutine(currentButtonCooldownCoroutine);

        if(isLocked || currentSliderResetCoroutine != null){
            OnInvalidButtonPress?.Invoke();
            return;
        }

        OnValidButtonPress?.Invoke();

        //If not draining then start draining
        if(currentSliderDrainCoroutine == null){
            currentSliderDrainCoroutine = DrainCurrentSliderCoroutine();
            StartCoroutine(currentSliderDrainCoroutine);
            return;
        }

        if(currentPressZone == null) return;

        if(!currentPressZone.IsPressValid(currentSlider.value)){
            FailedCurrentPressZone();
            return;
        }

        SuccessCurrentPressZone();
    }

    private void FailedCurrentPressZone(){
        //Update the current press zone to show which one failed
        crematoriumUI.UpdatePressZone(currentPressZone.pressZoneImage, PressZoneResultType.Failed);

        //Stop the drain coroutine and start the reset coroutine
        StopCoroutine(currentSliderDrainCoroutine);
        currentSliderDrainCoroutine = null;
        
        currentSliderResetCoroutine =  ResetCurrentSliderCoroutine();
        StartCoroutine(currentSliderResetCoroutine);
    }

    private void SuccessCurrentPressZone(){
        //Update the current press zone to show success
        crematoriumUI.UpdatePressZone(currentPressZone.pressZoneImage, PressZoneResultType.Success);

        currentPressZone = GetNextPressZone(currentSlider.value);

        //Boost the drain rate
        StartCoroutine(DrainRateBoostCoroutine());
    }

    private IEnumerator DrainRateBoostCoroutine(){
        CrematoriumSlider crematoriumSlider = sliderList[currentSliderIndex];

        currentDrainRate = crematoriumSlider.boostedDrainRate;
        yield return new WaitForSeconds(crematoriumSlider.boostedTimeInSeconds);
        currentDrainRate = crematoriumSlider.drainRate;
    }

    private IEnumerator ButtonCooldownCoroutine(){
        yield return new WaitForSeconds(buttonCooldownTime);
        currentButtonCooldownCoroutine = null;
    }

    private IEnumerator DrainCurrentSliderCoroutine(){
        //Update the slider header text
        crematoriumUI.UpdateSliderTitleText(currentSliderIndex, true, false);

        currentSlider = crematoriumUI.GetSliderByIndex(currentSliderIndex);
        currentDrainRate = sliderList[currentSliderIndex].drainRate;
        currentPressZone = GetNextPressZone(0);
        currentSlider.value = 0;

        while(currentSlider.value < currentSlider.maxValue){
            currentSlider.value += currentDrainRate * Time.deltaTime;

            //Check if the current value is higher then the max value
            if(currentPressZone != null){
                if(currentSlider.value > currentPressZone.maxPressAmount){
                    FailedCurrentPressZone();
                    yield break;
                }
            }

            yield return null;
        }

        currentSliderIndex++;
        crematoriumUI.UpdateSliderTitleText(currentSliderIndex, false, false);

        if(currentSliderIndex == MAX_SLIDER_INDEX){
            //Open the door and ignore all input
        }

        currentSliderDrainCoroutine = null;
    }

    private IEnumerator ResetCurrentSliderCoroutine(){
        crematoriumUI.UpdateSliderTitleText(currentSliderIndex, false, true);

        Slider currentSlider = crematoriumUI.GetSliderByIndex(currentSliderIndex);

        while(currentSlider.value > 0){
            currentSlider.value -= SLIDER_RESET_SPEED * Time.deltaTime;
            yield return null;
        }

        currentSlider.value = 0;
        currentPressZone = null;

        if(currentSliderResetCoroutine != null){
            currentSliderResetCoroutine = null;
        }

        crematoriumUI.UpdateSliderTitleText(currentSliderIndex, false, false);

        //Reset all the slider press zones
        for (int i = 0; i < pressZones.Count; i++){
            if(pressZones[i].associatedSliderIndex != currentSliderIndex) continue;
            crematoriumUI.UpdatePressZone(pressZones[i].pressZoneImage, PressZoneResultType.Default);
        }
    } 

    private CrematoriumPressZone GetNextPressZone(float amount){
        CrematoriumPressZone crematoriumPressZone = null;
        
        for (int i = 0; i < pressZones.Count; i++){
            if(pressZones[i].associatedSliderIndex != currentSliderIndex) continue;
            if(pressZones[i].maxPressAmount < amount) continue;
            if(pressZones[i] == currentPressZone) continue;

            if(crematoriumPressZone == null){
                crematoriumPressZone = pressZones[i];
            }

            if(currentPressZone == null){
                currentPressZone = crematoriumPressZone;
                continue;
            }

            //See if the next min value is closer to the value then the current one 
            float diff1 = Mathf.Abs(amount - currentPressZone.minPressAmount);
            float diff2 = Mathf.Abs(amount - crematoriumPressZone.minPressAmount);

            if(diff1 < diff2){
                continue;
            }

            if(diff2 < diff1){
                crematoriumPressZone = pressZones[i];
                continue;
            }

            continue;
        }

        return crematoriumPressZone;
    }
}