/// <summary>
/// Code taken from Unite 2016 Talk "Overthrowing the MonoBehaviour Tyranny in a Glorious Scriptable Object Revolution"
/// Original code belongs to Richard Fine, taken from GitHub - https://github.com/richard-fine/scriptable-object-demo/tree/main
/// </summary>

//Last Editor: Caleb Richardson
//Last Edited: March 5

using UnityEngine;

public abstract class AudioEvent : ScriptableObject{
	public abstract void Play(AudioSource source);

	public abstract void PlayOneShot(AudioSource source);
}
