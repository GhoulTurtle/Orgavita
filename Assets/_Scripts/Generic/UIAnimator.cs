using System;
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
    public static IEnumerator StretchAnimationCoroutine(Transform transformToAnimate, Vector2 goalScale, float animationDuration, bool deactivateAfterAnimation = true, bool scaledDeltaTime = true){
        
        var _goalScale = new Vector3(goalScale.x, goalScale.y, 1);

        float current = 0;

        while(Vector3.Distance(transformToAnimate.localScale, _goalScale) > LERP_SNAP_DISTANCE){
            transformToAnimate.localScale = Vector3.Lerp(transformToAnimate.localScale, _goalScale, current / animationDuration);
            if(scaledDeltaTime){
                current += Time.deltaTime;
            }
            else{
                current += Time.unscaledDeltaTime;
            }
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
    public static IEnumerator SinAnimationCoroutine(Transform transformToAnimate, float transformYOrigin, float animationSpeed, float animationDistance, float animationDuration = -1f, bool scaledTime = true){
        while(animationDuration == -1f){
            transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, SinAmount(transformYOrigin, animationSpeed, animationDistance, scaledTime), transformToAnimate.localPosition.z);
            yield return null;
        }

        if(animationDuration == -1f) yield break;

        float animationTimer = animationDuration;
        while(animationTimer >= 0f){
            if(scaledTime){
                animationTimer -= Time.deltaTime;
            }
            else{
                animationTimer -= Time.unscaledDeltaTime;
            }
            transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, SinAmount(transformYOrigin, animationSpeed, animationDistance, scaledTime), transformToAnimate.localPosition.z);
            yield return null;
        }

        transformToAnimate.localPosition = new Vector3(transformToAnimate.localPosition.x, transformYOrigin, transformToAnimate.localPosition.z);
    }

    private static float SinAmount(float yOrigin, float sinSpread, float sinIntensity, bool scaledTime = true){
        if(scaledTime){
            return yOrigin + Mathf.Sin(Time.time * sinSpread) * sinIntensity;
        }
        
        return yOrigin + Mathf.Sin(Time.unscaledTime * sinSpread) * sinIntensity;
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
    public static IEnumerator CosAnimationCoroutine(Transform transformToAnimate, float transformXOrigin, float animationSpeed, float animationDistance, float animationDuration = -1f, bool scaledTime = true){
        while(animationDuration == -1f){
            transformToAnimate.localPosition = new Vector3(CosAmount(transformXOrigin, animationSpeed, animationDistance, scaledTime), transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);
            yield return null;
        }

        if(animationDuration == -1f) yield break;

        float animationTimer = animationDuration;
        while(animationTimer >= 0f){
            if(scaledTime){
                animationTimer -= Time.deltaTime;
            }
            else{
                animationTimer -= Time.unscaledDeltaTime;
            }
            transformToAnimate.localPosition = new Vector3(CosAmount(transformXOrigin, animationSpeed, animationDistance, scaledTime), transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);            
            yield return null;
        }

        transformToAnimate.localPosition = new Vector3(transformXOrigin, transformToAnimate.localPosition.y, transformToAnimate.localPosition.z);            
    }

    private static float CosAmount(float xOrigin, float cosSpread, float cosIntensity, bool scaledTime = true){
        if(scaledTime){
            return xOrigin + Mathf.Cos(Time.time * cosSpread) * cosIntensity;
        }

        return xOrigin + Mathf.Cos(Time.unscaledTime * cosSpread) * cosIntensity;
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

    /// <summary>
    /// A lerp animation that lerps the textToAnimate margins to the goalMargins over the animation duration. 
    /// </summary>
    /// <param name="textToAnimate"></param>
    /// <param name="goalMargins"></param>
    /// <param name="animationDuration"></param>
    /// <returns></returns>
    public static IEnumerator LerpingTextMarginAnimationCoroutine(TextMeshProUGUI textToAnimate, Vector4 goalMargins, float animationDuration, bool scaledDeltaTime = true){
        float current = 0;

        while(Vector4.Distance(textToAnimate.margin, goalMargins) > LERP_SNAP_DISTANCE){
            textToAnimate.margin = Vector4.Lerp(textToAnimate.margin, goalMargins, current / animationDuration);
            if(scaledDeltaTime){
                current += Time.deltaTime;
            }
            else{
                current += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        textToAnimate.margin = goalMargins;
    }

    /// <summary>
    /// A lerp animation that lerps from the current object position to the goal position over the animation duration.
    /// </summary>
    /// <param name="transformToAnimate"></param>
    /// <param name="goalPosition"></param>
    /// <param name="animationDuration"></param>
    /// <param name="deactivateAfterAnimation"></param>
    /// <returns></returns>
    public static IEnumerator LerpingAnimationCoroutine(Transform transformToAnimate, Vector2 goalPosition, float animationDuration, bool deactivateAfterAnimation = true, bool scaledDeltaTime = true){
        var _goalPosition = new Vector3(goalPosition.x, goalPosition.y, 1);
        
        float current = 0;

        while(Vector3.Distance(transformToAnimate.localPosition, _goalPosition) > LERP_SNAP_DISTANCE){
            transformToAnimate.localPosition = Vector3.Lerp(transformToAnimate.localPosition, _goalPosition, current / animationDuration);
            if(scaledDeltaTime){
                current += Time.deltaTime;
            }
            else{
                current += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        transformToAnimate.localPosition = _goalPosition;

        if(deactivateAfterAnimation){
            transformToAnimate.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// A lerp animation that lerps a canvas group's alpha value.
    /// </summary>
    public static IEnumerator CanvasGroupAlphaFadeCoroutine(CanvasGroup canvasGroup, float animationDuration, float alphaStart, float alphaEnd = 0, bool scaledDeltaTime = true, Action onFadeAnimationFinishedCallback = null){
        float startingAlpha = Mathf.Clamp01(alphaStart);
        
        canvasGroup.alpha = startingAlpha;

        float current = 0;
        while(current <= animationDuration){
            if(scaledDeltaTime){
                current += Time.deltaTime / animationDuration;
            }
            else{
                current += Time.unscaledDeltaTime / animationDuration;
            }
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, alphaEnd, current / animationDuration);            
            yield return null;
        }

        canvasGroup.alpha  = alphaEnd;

        if(onFadeAnimationFinishedCallback != null){
            onFadeAnimationFinishedCallback?.Invoke();
        }
    }
}