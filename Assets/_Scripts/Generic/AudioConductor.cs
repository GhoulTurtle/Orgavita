using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Helper class that can apply effects/animations to audio mixers/sources.
/// </summary>
public static class AudioConductor{
    private const string MASTER_VOLUME = "Master_Volume";
    private const string MUSIC_VOLUME = "Music_Volume";
    private const string SFX_VOLUME = "SFX_Volume";
    private const string AMBIENCE_VOLUME = "Ambience_Volume";

    public static IEnumerator AudioFadeAnimationCoroutine(AudioMixer audioMixer, AudioGroupType audioGroup, float endDecibels, float animationDuration, bool scaledDeltaTime = true, Action OnFadeAnimationFinished = null){
        float current = 0;

        string audioGroupString = null;
        switch (audioGroup){
            case AudioGroupType.Master: audioGroupString = MASTER_VOLUME;
                break;
            case AudioGroupType.Music: audioGroupString = MUSIC_VOLUME;
                break;
            case AudioGroupType.SFX: audioGroupString = SFX_VOLUME;
                break;
            case AudioGroupType.Ambience: audioGroupString = AMBIENCE_VOLUME;
                break;
        }

        while(current <= animationDuration){
            if(scaledDeltaTime){
                current += Time.deltaTime;
            }
            else{
                current += Time.unscaledDeltaTime;
            }
            audioMixer.GetFloat(audioGroupString, out float value);
            audioMixer.SetFloat(audioGroupString, Mathf.Lerp(value, endDecibels, current / animationDuration));
            yield return null;
        }

        audioMixer.SetFloat(audioGroupString, endDecibels);

        if(OnFadeAnimationFinished != null){
            OnFadeAnimationFinished?.Invoke();
        }
    }
}
