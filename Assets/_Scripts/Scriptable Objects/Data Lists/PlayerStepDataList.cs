using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/Player Steps", fileName = "NewPlayerStepDataList")]
public class PlayerStepDataList : ScriptableObject{
    [Header("Player Step Data List")]
    [SerializeField] private List<PlayerStepDataEntry> PlayerStepDataEntries = new List<PlayerStepDataEntry>();

    public AudioEvent GetAudioEventFromTerrain(TerrainType terrainType){
        if(terrainType == TerrainType.None) return null;
        
        for (int i = 0; i < PlayerStepDataEntries.Count; i++){
            if(PlayerStepDataEntries[i].associatedTerrainType == terrainType){
                return PlayerStepDataEntries[i].associatedStepAudioEvent;
            }
        }

        return null;
    }
}