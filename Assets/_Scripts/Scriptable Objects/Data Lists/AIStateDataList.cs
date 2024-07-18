using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/AI State Data List", fileName = "AIStateDataList")]
public class AIStateDataList : ScriptableObject{
    [Header("AI State Data")]
    public List<AIStateType> aIStates= new List<AIStateType>();

    public Dictionary<AIStateType, AIState> GetAIStateDictionary(){
        if(aIStates.Count == 0){
            Debug.LogWarning("No valid state types are inputted into the aIStates list.");
            return null;
        }
        
        Dictionary<AIStateType, AIState> aIStateDictionary = new Dictionary<AIStateType, AIState>(); 

        for (int i = 0; i < aIStates.Count; i++){
            if(aIStateDictionary.ContainsKey(aIStates[i])) continue;

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

        return aIStateDictionary;
    }

}
