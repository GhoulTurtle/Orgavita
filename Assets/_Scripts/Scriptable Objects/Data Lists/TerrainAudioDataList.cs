using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/Terrain Audio Data List", fileName = "NewTerrainAudioDataList")]
public class TerrainAudioDataList : ScriptableObject{
    [Header("Terrain Audio Data List")]
    [SerializeField] private List<TerrainAudioData> terrainAudioDataList = new List<TerrainAudioData>();

    public AudioEvent GetAudioEventFromTerrainType(TerrainType terrainType){
        if(terrainType == TerrainType.None) return null;
        
        for (int i = 0; i < terrainAudioDataList.Count; i++){
            if(terrainAudioDataList[i].IsEnteredTerrainMatching(terrainType, out AudioEvent audioEvent)){
                return audioEvent;
            }
        }

        return null;
    }

    public void PlayAudioFromTerrain(TerrainType terrainType, AudioSource audioSource){
        AudioEvent audioEvent = GetAudioEventFromTerrainType(terrainType);
        if(audioEvent != null){
            audioEvent.PlayOneShot(audioSource, audioSource.transform.position);
        }
    }
}