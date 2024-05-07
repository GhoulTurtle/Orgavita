using UnityEngine;

public class Terrain : MonoBehaviour{
    [Header("Terrain Variables")]
    [SerializeField] private TerrainType terrainType;

    public TerrainType GetTerrainType() { 
        return terrainType; 
    }
}
