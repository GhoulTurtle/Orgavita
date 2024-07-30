using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object that represents a AI state transition from one state to another.
/// </summary>
[CreateAssetMenu(menuName = "AI/AI State Transition Definition", fileName = "NewAIStateTransitionDefinitionSO")]
public class AIStateTransitionDefinition : ScriptableObject{
    [Header("Transition Data")]
    public AIStateType fromAIState;
    public AIStateType toAIState;
    public List<AIStateTransitionConditionEntry> aIStateTransitionConditions = new List<AIStateTransitionConditionEntry>();

    public virtual bool IsValidStateTransition(AIStateType aIState){
        if(fromAIState == aIState) return true;
        return false;
    }

    public virtual bool AttemptTransition(AIStateMachine stateMachine){
        if(aIStateTransitionConditions.Count == 0){
            Debug.LogWarning("No transition conditions are inputted for the aIStateTransitionConditions list.");
            return false;
        }

        bool validTransition = true;

        for (int i = 0; i < aIStateTransitionConditions.Count; i++){
            validTransition = aIStateTransitionConditions[i].AttemptTransition(stateMachine);
            if(!validTransition) return false;
        }

        return validTransition;
    }

    public void GenerateTransitionJobs(AIStateMachine aIStateMachine){
        for (int i = 0; i < aIStateTransitionConditions.Count; i++){
            aIStateTransitionConditions[i].AddTransitionJobsToDictionary(aIStateMachine);
        }
    }

    public List<AIStateTransitionConditionEntry> GetAIStateTransitionConditionEntries(){
        return aIStateTransitionConditions;
    }

    public AIStateType GetToAIState(){
        return toAIState;
    }
}