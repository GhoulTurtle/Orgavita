using UnityEngine;

[System.Serializable]
public class TerrainAudioData{
    [SerializeField] private TerrainType associatedTerrainType;
    [SerializeField] private AudioEvent associatedAudioEvent;

    public bool IsEnteredTerrainMatching(TerrainType terrainType, out AudioEvent audioEvent){
        if(terrainType == associatedTerrainType){
            audioEvent = associatedAudioEvent;
        }
        else{
            audioEvent = null;
        }
        return terrainType == associatedTerrainType;
    }
}