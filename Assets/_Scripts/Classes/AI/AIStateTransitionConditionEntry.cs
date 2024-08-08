using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that represents a check operation for a AI state transition.
/// </summary>
[System.Serializable]
public class AIStateTransitionConditionEntry{
    public List<AIStateTransitionType> aIStateTransitionTypes = new List<AIStateTransitionType>();
    public CheckOperationType checkOperationType = CheckOperationType.None;

    public bool AttemptTransition(AIStateMachine aIStateMachine){
        if(aIStateTransitionTypes.Count == 0){
            Debug.LogWarning("No transition types are inputted into aIStateTransitionTypes.");    
            return false;
        }
        
        bool validTransition = true;

        for (int i = 0; i < aIStateTransitionTypes.Count; i++){
            AIStateTransitionConditionJob aIStateTransitionConditionJob = aIStateMachine.AttemptGetTransitionConditionJob(aIStateTransitionTypes[i]);
            if(aIStateTransitionConditionJob == null){
                aIStateTransitionConditionJob = AddNewTransitionJob(aIStateTransitionTypes[i], aIStateMachine);
            }
            
            bool conditionResult = aIStateTransitionConditionJob.EvaluateTransitionCondition(aIStateMachine);

            switch (checkOperationType){
                case CheckOperationType.None: validTransition = conditionResult;
                    if(!validTransition) return false;
                    break;
                case CheckOperationType.And: validTransition &= conditionResult;
                    if(!validTransition) return false;
                    break;
                case CheckOperationType.Or: 
                    validTransition |= conditionResult;
                    if(validTransition) return true;
                    break;
                case CheckOperationType.Not: 
                    validTransition = !conditionResult;
                    if(!validTransition) return false;
                break;
            }

            if(!validTransition) break;
        }
        return validTransition;
    }

    public void AddTransitionJobsToDictionary(AIStateMachine aIStateMachine){
        for (int i = 0; i < aIStateTransitionTypes.Count; i++){
            if(aIStateMachine.AttemptGetTransitionConditionJob(aIStateTransitionTypes[i]) == null){
                AddNewTransitionJob(aIStateTransitionTypes[i], aIStateMachine);
            }
        }
    }

    public List<AIStateTransitionType> GetAIStateTransitionTypes(){
        return aIStateTransitionTypes;
    }

    private AIStateTransitionConditionJob AddNewTransitionJob(AIStateTransitionType aIStateTransitionType, AIStateMachine aIStateMachine){
        AIStateTransitionConditionJob aIStateTransitionConditionJob = null;
        
        switch (aIStateTransitionType){
            case AIStateTransitionType.TargetInLOSForTargetTime: aIStateTransitionConditionJob = new TargetInLOSForTargetTime();
                break;
            case AIStateTransitionType.TargetOutLOSForLooseTime: aIStateTransitionConditionJob = new TargetOutLOSForLooseTime();
                break;
            case AIStateTransitionType.SearchTimeExpired: aIStateTransitionConditionJob = new SearchTimeExpired();
                break;
            case AIStateTransitionType.TargetInLOS: aIStateTransitionConditionJob = new TargetInLOS();
                break;
            case AIStateTransitionType.TargetInAttackRange: aIStateTransitionConditionJob = new TargetInAttackRange();
                break;
            case AIStateTransitionType.IdleTimeExpired: aIStateTransitionConditionJob = new IdleTimeExpired();
                break;
            case AIStateTransitionType.PatrolTimeExpired: aIStateTransitionConditionJob = new PatrolTimeExpired();
                break;
            case AIStateTransitionType.ReachedMovementTarget: aIStateTransitionConditionJob = new ReachedMovementTarget();
                break;
            case AIStateTransitionType.FollowCommandTriggered: aIStateTransitionConditionJob = new FollowCommandTriggered();
                break;
            case AIStateTransitionType.StopFollowCommandTriggered: aIStateTransitionConditionJob = new StopFollowCommandTriggered();
                break;
        }

        aIStateMachine.AddStateTransitionConditionJob(aIStateTransitionType, aIStateTransitionConditionJob);

        return aIStateTransitionConditionJob;
    }
}