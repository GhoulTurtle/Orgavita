using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/AI State Data List", fileName = "AIStateDataList")]
public class AIStateDataList : ScriptableObject{
    [Header("AI State Data")]
    public List<AIStateType> aIStates = new List<AIStateType>();
    public List<AIStateTransitionDefinition> aIStateTransitions = new List<AIStateTransitionDefinition>();

    public void GetAIStateDictionary(Dictionary<AIStateType, BaseState<AIStateType>> stateDictionary, out AIStateType firstState){
        if(aIStates.Count == 0){
            Debug.LogWarning("No valid state types are inputted into the aIStates list.");
            firstState = AIStateType.Idle;
            return;
        }

        if(aIStateTransitions.Count == 0){
            Debug.LogWarning("No valid transitions are inputted into the aIStateTransitions list.");
            firstState = AIStateType.Idle;
            return;
        }

        for (int i = 0; i < aIStates.Count; i++){
            if(stateDictionary.ContainsKey(aIStates[i])){
                aIStates.Remove(aIStates[i]);
                continue;
            }

            switch (aIStates[i]){
                case AIStateType.Idle: stateDictionary.Add(AIStateType.Idle, new AIIdleState(AIStateType.Idle));
                    break;
                case AIStateType.Patrol:stateDictionary.Add(AIStateType.Patrol, new AIPatrolState(AIStateType.Patrol));
                    break;
                case AIStateType.Chase: stateDictionary.Add(AIStateType.Chase, new AIChaseState(AIStateType.Chase));
                    break;
                case AIStateType.Search: stateDictionary.Add(AIStateType.Search, new AISearchState(AIStateType.Search));
                    break;
                case AIStateType.Attack: stateDictionary.Add(AIStateType.Attack, new AIAttackState(AIStateType.Attack));
                    break;
                case AIStateType.Follow: stateDictionary.Add(AIStateType.Follow, new AIFollowState(AIStateType.Follow));
                    break;
                case AIStateType.Dialogue: stateDictionary.Add(AIStateType.Dialogue, new AIDialogueState(AIStateType.Dialogue));
                    break;
                case AIStateType.Cutscene: stateDictionary.Add(AIStateType.Cutscene, new AICutsceneState(AIStateType.Cutscene));
                    break;
            }
        }

        firstState = aIStates[0];
    }

    public void GenerateAIStateTransitionJobDictionary(AIStateMachine aIStateMachine){
        for (int i = 0; i < aIStateTransitions.Count; i++){
            aIStateTransitions[i].GenerateTransitionJobs(aIStateMachine);
        }
    }

    public void NextStateTransition(AIStateMachine aIStateMachine){
        AIStateType currentAIState = aIStateMachine.GetCurrentState();

        AIStateType nextAIState = currentAIState;
        for (int i = 0; i < aIStateTransitions.Count; i++){
            if(!aIStateTransitions[i].IsValidStateTransition(currentAIState)) continue;
            if(!aIStateTransitions[i].AttemptTransition(aIStateMachine)) continue;
            else{
                nextAIState = aIStateTransitions[i].GetToAIState();
                Debug.Log("Transition Triggered! Going to the: " + nextAIState + " state!");
                break;
            }
        }

        aIStateMachine.UpdateState(nextAIState);
    }
}