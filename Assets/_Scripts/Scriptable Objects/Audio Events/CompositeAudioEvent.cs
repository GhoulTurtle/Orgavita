/// <summary>
/// Code taken from Unite 2016 Talk "Overthrowing the MonoBehaviour Tyranny in a Glorious Scriptable Object Revolution"
/// Original code belongs to Richard Fine, taken from GitHub - https://github.com/richard-fine/scriptable-object-demo/tree/main
/// </summary>

//Last Editor: Caleb Richardson
//Last Edited: March 5

using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName="Audio Events/Composite")]
public class CompositeAudioEvent : AudioEvent{
	[Serializable]
	public struct CompositeEntry{
		public AudioEvent Event;
		public float Weight;
	}

	public CompositeEntry[] Entries;

	public override void Play(AudioSource source){
		float totalWeight = 0;
		for (int i = 0; i < Entries.Length; ++i)
			totalWeight += Entries[i].Weight;

			float pick = Random.Range(0, totalWeight);
			for (int i = 0; i < Entries.Length; ++i){
				if (pick > Entries[i].Weight){
					pick -= Entries[i].Weight;
					continue;
				}

			Entries[i].Event.Play(source);
			return;
		}
	}

    public override void PlayOneShot(AudioSource source, Vector3 position){
		float totalWeight = 0;
		for (int i = 0; i < Entries.Length; ++i)
			totalWeight += Entries[i].Weight;

			float pick = Random.Range(0, totalWeight);
			for (int i = 0; i < Entries.Length; ++i){
				if (pick > Entries[i].Weight){
					pick -= Entries[i].Weight;
					continue;
				}

			Entries[i].Event.PlayOneShot(source, position);
			return;
		}
    }
}