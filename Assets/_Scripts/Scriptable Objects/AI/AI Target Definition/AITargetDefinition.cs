using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Target Definition", fileName = "NewAITargetDefinitionSO")]
public class AITargetDefinition : ScriptableObject{
   [Header("Target Definition")]
   public LayerMask targetLayerMask;
   
   [Header("Ally Definition")]
   public LayerMask allyLayerMask;

   [Header("Obstruction Definition")]
   public LayerMask obstructionLayerMask;

    public LayerMask GetTargetLayerMask(){
        return targetLayerMask;
    }

    public LayerMask GetAllyLayerMask(){
        return allyLayerMask;
    }

    public LayerMask GetObstructionLayerMask(){
        return obstructionLayerMask;
    }
}