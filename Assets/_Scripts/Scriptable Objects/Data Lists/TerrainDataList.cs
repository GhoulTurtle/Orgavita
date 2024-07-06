using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/Terrain Data List", fileName = "NewTerrainDataList")]
public class TerrainDataList : ScriptableObject{
    [SerializeField] private List<TerrainType> terrainList;
    public bool IsEnteredTerrainValid(TerrainType terrainType){
        return terrainList.Contains(terrainType);
    }
}
