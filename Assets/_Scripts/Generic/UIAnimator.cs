using System.Collections;
using TMPro;
using UnityEngine;

public static class UIAnimator{
    private const float LERP_SNAP_DISTANCE = 0.01f;

/// <summary>
/// A stretch animation that lerps from the current object scale to the goal scale over the animation duration.
/// </summary>
/// <param name="transformToAnimate"></param>
/// <param name="goalScale"></param>
/// <param name="animationDuration"></param>
/// <param name="deactivateAfterAnimation"></param>
/// <returns></returns>
    public static IEnumerator UIStretchAnimation(Transform transformToAnimate, Vector3 goalScale, float animationDuration, bool deactivateAfterAnimation = true){
        
        float current = 0;

        while(Vector3.Distance(transformToAnimate.localScale, goalScale) > LERP_SNAP_DISTANCE){
            transformToAnimate.localScale = Vector3.Lerp(transformToAnimate.localScale, goalScale, current / animationDuration);
            current += Time.deltaTime;
            yield return null;
        }

        transformToAnimate.localScale = goalScale;

        if(deactivateAfterAnimation){
            transformToAnimate.gameObject.SetActive(false);
        }
    }

/// <summary>
/// A sin animation that animates a transform local Y position following a sin wave. Can adjust the sin wave with the animationSpeed and animationDistance. 
/// Can have a set animation time or run until the coroutine is terminated.
/// </summary>
/// <param name="transformToAnimate"></param>
/// <param name="transformYOrigin"></param>
/// <param name="animationSpeed"></param>
/// <param name="animationDistance"></param>
/// <param name="infiniteAnimation"></param>
/// <param name="animationDuration"></param>
/// <returns></returns>
    public static IEnumerator UISinAnimation(Transform transformToAnimate, float transformYOrigin, float animationSpeed, float animationDistance, float animationDuration = -1f){
        while(animationDuration == -1f){
            transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, SinAmount(transformYOrigin, animationSpeed, animationDistance), transformToAnimate.localPosition.z);
            yield return null;
        }

        if(animationDuration == -1f) yield break;

        float animationTimer = animationDuration;
        while(animationTimer >= 0f){
            animationTimer -= Time.deltaTime;
            transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, SinAmount(transformYOrigin, animationSpeed, animationDistance), transformToAnimate.localPosition.z);
            yield return null;
        }

        transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, transformYOrigin, transformToAnimate.localPosition.z);
    }

    private static float SinAmount(float yOrigin, float sinSpread, float sinIntensity){
        return yOrigin + Mathf.Sin(Time.time * sinSpread) * sinIntensity;
    }

/// <summary>
/// A cos animation that animates a transform local X position following a cos wave. Can adjust the cos wave with the animationSpeed and animationDistance. 
/// Can have a set animation time or run until the coroutine is terminated.
/// </summary>
/// <param name="transformToAnimate"></param>
/// <param name="transformXOrigin"></param>
/// <param name="animationSpeed"></param>
/// <param name="animationDistance"></param>
/// <param name="infiniteAnimation"></param>
/// <param name="animationDuration"></param>
/// <returns></returns>
    public static IEnumerator UICosAnimation(Transform transformToAnimate, float transformXOrigin, float animationSpeed, float animationDistance, float animationDuration = -1f){
        while(animationDuration == -1f){
            transformToAnimate.localPosition = new Vector3(CosAmount(transformXOrigin, animationSpeed, animationDistance), transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);
            yield return null;
        }

        if(animationDuration == -1f) yield break;

        float animationTimer = animationDuration;
        while(animationTimer >= 0f){
            animationTimer -= Time.deltaTime;
            transformToAnimate.localPosition = new Vector3(CosAmount(transformXOrigin, animationSpeed, animationDistance), transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);            
            yield return null;
        }

        transformToAnimate.localPosition = new Vector3(transformXOrigin, transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);            
    }

    private static float CosAmount(float xOrigin, float cosSpread, float cosIntensity){
        return xOrigin + Mathf.Cos(Time.time * cosSpread) * cosIntensity;
    }

    public static IEnumerator AnimateText(TextMeshProUGUI textToAnimate, DialogueEffect sentenceDialogueEffect){
        yield return null;
        switch (sentenceDialogueEffect){
            case DialogueEffect.None:
                break;
            case DialogueEffect.Wobble:
                break;
            case DialogueEffect.Pulse:
                break;
            case DialogueEffect.Shake:
                break;
        }
    }
}