using UnityEngine;

public class PlayerStepController : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private AudioSource playerStepAudioSource;
    [SerializeField] private TerrainAudioDataList playerStepDataList;
    [SerializeField] private PlayerFirstCamLook playerCameraController;
    
    private void Awake() {
        if(playerCameraController != null){
            playerCameraController.OnTerrainStep += EvaulateCurrentTerrainType;
        }
    }

    private void OnDestroy() {
        if(playerCameraController != null){
            playerCameraController.OnTerrainStep -= EvaulateCurrentTerrainType;
        }
    }

    private void EvaulateCurrentTerrainType(object sender, PlayerFirstCamLook.TerrainStepEventArgs e){
        AudioEvent terrainStepAudioEvent = playerStepDataList.GetAudioEventFromTerrainType(e.terrainType);
        if(terrainStepAudioEvent == null) return;

        terrainStepAudioEvent.PlayOneShot(playerStepAudioSource, transform.position);    
    }
}