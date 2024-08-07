using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data List/AI State Data List", fileName = "AIStateDataList")]
public class AIStateDataList : ScriptableObject{
    [Header("AI State Data")]
    public List<AIStateType> aIStates = new List<AIStateType>();
    public List<AIStateTransitionDefinition> aIStateTransitions = new List<AIStateTransitionDefinition>();

    public void GetAIStateDictionary(Dictionary<AIStateType, BaseState<AIStateType>> stateDictionary, StateMachine<AIStateType> stateMachine, out AIStateType firstState){
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
                case AIStateType.Idle: 
                    AIIdleState aIIdleState = new(AIStateType.Idle);
                    aIIdleState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Idle, aIIdleState);
                    break;
                case AIStateType.Patrol:
                    AIPatrolState aIPatrolState = new(AIStateType.Patrol);
                    aIPatrolState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Patrol, aIPatrolState);
                    break;
                case AIStateType.Chase: 
                    AIChaseState aIChaseState = new(AIStateType.Chase);
                    aIChaseState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Chase, aIChaseState);
                    break;
                case AIStateType.Search: 
                    AISearchState aISearchState = new(AIStateType.Search);
                    aISearchState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Search, aISearchState);
                    break;
                case AIStateType.Attack: 
                    AIAttackState aIAttackState = new(AIStateType.Attack);
                    aIAttackState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Attack, aIAttackState);
                    break;
                case AIStateType.Follow: 
                    AIFollowState aIFollowState = new(AIStateType.Follow);
                    aIFollowState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Follow, aIFollowState);
                    break;
                case AIStateType.Dialogue: 
                    AIDialogueState aIDialogueState = new(AIStateType.Dialogue);
                    aIDialogueState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Dialogue, aIDialogueState);
                    break;
                case AIStateType.Cutscene: 
                    AICutsceneState aICutsceneState = new(AIStateType.Cutscene);
                    aICutsceneState.SetupState(stateMachine);
                    stateDictionary.Add(AIStateType.Cutscene, aICutsceneState);
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
                
                //Reset the transition jobs
                List<AIStateTransitionType> transitions = new List<AIStateTransitionType>();
                List<AIStateTransitionType> aIStateTransitionTypes = new List<AIStateTransitionType>();
                List<AIStateTransitionConditionEntry> conditionEntries = aIStateTransitions[i].GetAIStateTransitionConditionEntries();

                for (int j = 0; j < conditionEntries.Count; j++){
                    aIStateTransitionTypes = conditionEntries[j].GetAIStateTransitionTypes();
                    for(int k = 0; k < aIStateTransitionTypes.Count; k++){
                        if(transitions.Contains(aIStateTransitionTypes[k])) continue;
                        transitions.Add(aIStateTransitionTypes[k]);
                    }
                }

                aIStateMachine.ResetTransitionConditionJobs(transitions);            

                //Setup timers for the next state transition jobs
                StartTransitionTimers(aIStateMachine, nextAIState);

                Debug.Log("Transition Triggered! Going to the: " + nextAIState + " state!");
                break;
            }
        }

        aIStateMachine.UpdateState(nextAIState);
    }

    public void StartTransitionTimers(AIStateMachine aIStateMachine, AIStateType aIStateType){
        List<AIStateTransitionType> transitions = new List<AIStateTransitionType>();
        List<AIStateTransitionType> aIStateTransitionTypes = new List<AIStateTransitionType>();
        List<AIStateTransitionConditionEntry> conditionEntries = new List<AIStateTransitionConditionEntry>();

        for (int i = 0; i < aIStateTransitions.Count; i++){
            if(!aIStateTransitions[i].IsValidStateTransition(aIStateType)) continue;

            conditionEntries = aIStateTransitions[i].GetAIStateTransitionConditionEntries();
            for(int z = 0; z < conditionEntries.Count; z++){
                aIStateTransitionTypes = conditionEntries[z].GetAIStateTransitionTypes();
                for (int x = 0; x < aIStateTransitionTypes.Count; x++){
                    if(transitions.Contains(aIStateTransitionTypes[x])) continue;
                    transitions.Add(aIStateTransitionTypes[x]);
                }
            }
        }

        aIStateMachine.StartTransitionTimers(transitions);
    }
}