using System.Collections.Generic;
using UnityEngine;

public class ModelMaterialHandler : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private List<ModelMaterialEntry> modelMaterialList = new List<ModelMaterialEntry>();

    [Header("Material Options")]
    [SerializeField] private bool shareOneMaterial = false;
    [SerializeField] private Material sharedGameMaterial;
    [SerializeField] private Material sharedUIMaterial;
    [SerializeField] private Material sharedCutsceneMaterial;


    private void Awake() {
        GameManager.OnGameStateChange += EvaulateGameState;
    }

    private void OnDestroy() {
        GameManager.OnGameStateChange -= EvaulateGameState;
    }

    private void EvaulateGameState(object sender, GameManager.GameStateEventArgs e){
        UpdateModelMaterials(e.State);
    }

    private void UpdateModelMaterials(GameState gameState){
        for (int i = 0; i < modelMaterialList.Count; i++){
            Material stateMaterial = null;

            switch (gameState){
                case GameState.Game: stateMaterial = shareOneMaterial ? sharedGameMaterial : modelMaterialList[i].gameMaterial;
                    break;
                case GameState.UI: stateMaterial = shareOneMaterial ? sharedUIMaterial : modelMaterialList[i].uIMaterial;
                    break;
                case GameState.Cutscene: stateMaterial = shareOneMaterial ? sharedCutsceneMaterial : modelMaterialList[i].cutsceneMaterial;
                    break;
            }

            if(stateMaterial == null) continue;

            modelMaterialList[i].meshRenderer.material = stateMaterial;
        }
    }
}
