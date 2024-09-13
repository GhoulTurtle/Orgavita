using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
/// Will start all text animations coroutines defined in a TextEffectDefinition list, on the animationRunner
/// </summary>
/// <param name="animationRunner"></param>
/// <param name="textToAnimate"></param>
/// <param name="textString"></param>
/// <param name="textEffectDefinitions"></param>
    public static void StartTextAnimations(MonoBehaviour animationRunner, TMP_Text textToAnimate, TextContentProfile textContentProfile, out List<IEnumerator> runningCoroutines, bool scaledTime = true){
        runningCoroutines = new List<IEnumerator>();

        if(textContentProfile.textEffectDefinitionList.Count == 0) return;

        for (int i = 0; i < textContentProfile.textEffectDefinitionList.Count; i++){
            IEnumerator animation = null;

            switch (textContentProfile.textEffectDefinitionList[i].textEffect){
                case TextEffect.None:
                    break;
                case TextEffect.PopShrink: 
                    break;
                case TextEffect.Wobble: 
                    animation = WobbleTextAnimationCoroutine(textContentProfile.textContent, textToAnimate, textContentProfile.textEffectDefinitionList[i], scaledTime);
                    break;
                case TextEffect.Pulse:
                    break;
                case TextEffect.Shake:
                    break;
            }

            if(animation != null){
                animationRunner.StartCoroutine(animation);
                runningCoroutines.Add(animation);
            }
        }
    }

    public static void StartTextAnimations(MonoBehaviour animationRunner, TMP_Text textToAnimate, TextContentProfile textContentProfile, bool scaledTime = true){
        if(textContentProfile.textEffectDefinitionList.Count == 0) return;

        for (int i = 0; i < textContentProfile.textEffectDefinitionList.Count; i++){
            IEnumerator animation = null;

            switch (textContentProfile.textEffectDefinitionList[i].textEffect){
                case TextEffect.None:
                    break;
                case TextEffect.PopShrink: 
                    break;
                case TextEffect.Wobble: 
                    animation = WobbleTextAnimationCoroutine(textContentProfile.textContent, textToAnimate, textContentProfile.textEffectDefinitionList[i], scaledTime);
                    break;
                case TextEffect.Pulse:
                    break;
                case TextEffect.Shake:
                    break;
            }

            if(animation != null){
                animationRunner.StartCoroutine(animation);
            }
        }
    }

    public static IEnumerator PopShrinkTextAnimationCoroutine(TMP_Text text, TextEffectDefinition textEffectDefinition, bool scaledTime = true){
        float duration = 0.5f;
        float elapsedTime = 0f;

        text.ForceMeshUpdate();
        Mesh mesh = text.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] originalVertices = (Vector3[])vertices.Clone();

        // Calculate vertex indices for the specified text range
        List<int> vertexIndices = GetVertexIndicesForRange(text, textEffectDefinition.startIndex, textEffectDefinition.endIndex);

        while (true){
            elapsedTime += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            float t = Mathf.PingPong(elapsedTime / duration, 1f);
            float scaleFactor = Mathf.Lerp(1f, 1.5f, t); // Scaling from 1 to 1.5

            // Update vertices based on scale factor
            foreach (int index in vertexIndices){
                vertices[index] = originalVertices[index] * scaleFactor;
            }

            mesh.vertices = vertices;
            text.canvasRenderer.SetMesh(mesh);

            yield return null;
        }
    }

    public static IEnumerator WobbleTextAnimationCoroutine(string textContent, TMP_Text textToAnimate, TextEffectDefinition textEffectDefinition, bool scaledTime = true){
        //Calculate the word indexes to wobble each word
        List<int> wordIndexes = new List<int>{0};
        List<int> wordLengths = new List<int>();

        string textEffectRange = textContent.Substring(textEffectDefinition.startIndex, textEffectDefinition.endIndex - textEffectDefinition.startIndex);

        for (int index = textEffectRange.IndexOf(' '); index > -1; index = textEffectRange.IndexOf(' ', index + 1)){
            wordLengths.Add(index - wordIndexes[wordIndexes.Count - 1]);
            wordIndexes.Add(index + 1);
        }

        wordLengths.Add(textEffectRange.Length - wordIndexes[wordIndexes.Count - 1]);

        while(true){
            textToAnimate.ForceMeshUpdate();

            Mesh mesh = textToAnimate.mesh;
            Vector3[] vertices  = textToAnimate.mesh.vertices;

            for (int i = 0; i < wordIndexes.Count; i++){
                int wordIndex = wordIndexes[i];

                Vector3 offset;

                if(scaledTime){
                    offset = Wobble(Time.time + i);
                }
                else{
                    offset = Wobble(Time.unscaledTime + i);
                }

                for (int j = 0; j < wordLengths[i]; j++){
                    TMP_CharacterInfo c = textToAnimate.textInfo.characterInfo[wordIndex+j];

                    int index = c.vertexIndex;

                    vertices[index] += offset;
                    vertices[index + 1] += offset;
                    vertices[index + 2] += offset;
                    vertices[index + 3] += offset;
                }
            }

            mesh.vertices = vertices;
            textToAnimate.canvasRenderer.SetMesh(mesh);

            yield return null;
        }
    }

    private static Vector2 Wobble(float time){
        return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.5f));
    }

    public static IEnumerator PulseTextAnimationCoroutine(TMP_Text text, TextEffectDefinition textEffectDefinition, bool scaledTime = true){
        float duration = 0.5f;
        float elapsedTime = 0f;

        text.ForceMeshUpdate();
        Mesh mesh = text.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] originalVertices = (Vector3[])vertices.Clone();

        // Calculate vertex indices for the specified text range
        List<int> vertexIndices = GetVertexIndicesForRange(text, textEffectDefinition.startIndex, textEffectDefinition.endIndex);

        while (true){
            elapsedTime += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            float t = Mathf.PingPong(elapsedTime / duration, 1f);
            float pulseFactor = Mathf.Lerp(1f, 1.2f, t); // Pulsing between 1 and 1.2

            // Update vertices based on pulse factor
            foreach (int index in vertexIndices){
                vertices[index] = originalVertices[index] * pulseFactor;
            }

            mesh.vertices = vertices;
            text.canvasRenderer.SetMesh(mesh);

            yield return null;
        }
    }

    public static IEnumerator ShakeTextAnimationCoroutine(TMP_Text text, TextEffectDefinition textEffectDefinition, bool scaledTime = true){
        float duration = 0.5f;
        float elapsedTime = 0f;
        float shakeIntensity = 0.1f; // Adjust the intensity as needed

        text.ForceMeshUpdate();
        Mesh mesh = text.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] originalVertices = (Vector3[])vertices.Clone();

        // Calculate vertex indices for the specified text range
        List<int> vertexIndices = GetVertexIndicesForRange(text, textEffectDefinition.startIndex, textEffectDefinition.endIndex);

        while (true){
            elapsedTime += scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            float t = Mathf.PingPong(elapsedTime / duration, 1f);
            float shakeAmount = Mathf.Lerp(0f, shakeIntensity, t);

            // Apply shake offset to vertices
            foreach (int index in vertexIndices){
                vertices[index] = originalVertices[index] + new Vector3(
                    Random.Range(-shakeAmount, shakeAmount),
                    Random.Range(-shakeAmount, shakeAmount),
                    0f
                );
            }

            mesh.vertices = vertices;
            text.canvasRenderer.SetMesh(mesh);

            yield return null;
        }
    }

    private static List<int> GetVertexIndicesForRange(TMP_Text text, int startIndex, int endIndex){
        List<int> indices = new List<int>();
        text.ForceMeshUpdate();
        
        TMP_TextInfo textInfo = text.textInfo;

        // Iterate over each character in the text to find the corresponding vertices
        for (int i = 0; i < textInfo.characterCount; i++){
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (i >= startIndex && i <= endIndex){
                int vertexIndex = charInfo.vertexIndex;
                indices.Add(vertexIndex);
                indices.Add(vertexIndex + 1);
                indices.Add(vertexIndex + 2);
                indices.Add(vertexIndex + 3);
            }
        }

        return indices;
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