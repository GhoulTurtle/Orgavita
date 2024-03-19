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
    public static IEnumerator UIStretchAnimationCoroutine(Transform transformToAnimate, Vector2 goalScale, float animationDuration, bool deactivateAfterAnimation = true){
        
        var _goalScale = new Vector3(goalScale.x, goalScale.y, 1);

        float current = 0;

        while(Vector3.Distance(transformToAnimate.localScale, _goalScale) > LERP_SNAP_DISTANCE){
            transformToAnimate.localScale = Vector3.Lerp(transformToAnimate.localScale, _goalScale, current / animationDuration);
            current += Time.deltaTime;
            yield return null;
        }

        transformToAnimate.localScale = _goalScale;

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
    public static IEnumerator UISinAnimationCoroutine(Transform transformToAnimate, float transformYOrigin, float animationSpeed, float animationDistance, float animationDuration = -1f){
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
    public static IEnumerator UICosAnimationCoroutine(Transform transformToAnimate, float transformXOrigin, float animationSpeed, float animationDistance, float animationDuration = -1f){
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

    /// <summary>
    /// Text animations. UNDER CONSTRUCTION
    /// </summary>
    /// <param name="textToAnimate"></param>
    /// <param name="sentenceDialogueEffect"></param>
    /// <returns></returns>

    public static IEnumerator AnimateTextCoroutine(TextMeshProUGUI textToAnimate, DialogueEffect sentenceDialogueEffect){
        yield return null;
    }

    public static IEnumerator UILerpingAnimationCoroutine(Transform transformToAnimate, Vector2 goalPosition, float animationDuration, bool deactivateAfterAnimation = true){
        var _goalScale = new Vector3(goalPosition.x, goalPosition.y, 1);
        
        float current = 0;

        while(Vector3.Distance(transformToAnimate.localScale, _goalScale) > LERP_SNAP_DISTANCE){
            transformToAnimate.localPosition = Vector3.Lerp(transformToAnimate.localPosition, _goalScale, current / animationDuration);
            current += Time.deltaTime;
            yield return null;
        }

        transformToAnimate.localPosition = _goalScale;

        if(deactivateAfterAnimation){
            transformToAnimate.gameObject.SetActive(false);
        }
    }
}