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
    [SerializeField, MinMaxRange(0.1f, 600f)] private RangedFloat fadeTimeValues = new RangedFloat();

    public void StartFadeTime(){
        var fadeTime = Random.Range(fadeTimeValues.minValue, fadeTimeValues.maxValue);
        
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
