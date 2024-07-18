using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/AI State Data List", fileName = "AIStateDataList")]
public class AIStateDataList : ScriptableObject{
    [Header("AI State Data")]
    public List<AIStateType> aIStates = new List<AIStateType>();
    public List<AIStateTransitionDefinition> aIStateTransitions = new List<AIStateTransitionDefinition>();

    public void GetAIStateDictionary(Dictionary<AIStateType, BaseState<AIStateType>> stateDictionary){
        if(aIStates.Count == 0){
            Debug.LogWarning("No valid state types are inputted into the aIStates list.");
            return;
        }

        if(aIStateTransitions.Count == 0){
            Debug.LogWarning("No valid transitions are inputted into the aIStateTransitions list.");
            return;
        }

        for (int i = 0; i < aIStates.Count; i++){
            if(stateDictionary.ContainsKey(aIStates[i])){
                aIStates.Remove(aIStates[i]);
                continue;
            }

            switch (aIStates[i]){
                case AIStateType.Idle: 
                    break;
                case AIStateType.Patrol:
                    break;
                case AIStateType.Chase:
                    break;
                case AIStateType.Search:
                    break;
                case AIStateType.Attack:
                    break;
                case AIStateType.Follow:
                    break;
            }
        }
    }
}
