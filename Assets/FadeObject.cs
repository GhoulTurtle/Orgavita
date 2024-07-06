using System.Collections;
using UnityEngine;

/// <summary>
/// A object that is needing to be fade out of the scene.
/// TO-DO: Make it actually fade, currently just destorys the gameobject that the fade object component is attached to after a random amount of time.
/// </summary>
public class FadeObject : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AudioSource fadeObjectAudioSource;

    [Header("Fade Variables")]
    [SerializeField, Range(0.1f, 180f)] private float minFadeTime;
    [SerializeField, Range(0.1f, 180f)] private float maxFadeTime;

    public void StartFadeTime(){
        var fadeTime = Random.Range(minFadeTime, maxFadeTime);
        
        StartCoroutine(FadeCoroutine(fadeTime));
    }

    public AudioSource GetFadeObjectAudioSource(){
        if(fadeObjectAudioSource == null){
            fadeObjectAudioSource = gameObject.AddComponent<AudioSource>();
        }

        return fadeObjectAudioSource;
    }

    private IEnumerator FadeCoroutine(float fadeTime){
        yield return new WaitForSeconds(fadeTime);
        Destroy(gameObject);
    }
}
