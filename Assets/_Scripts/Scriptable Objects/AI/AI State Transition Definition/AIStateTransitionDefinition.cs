using UnityEngine;

public abstract class AIStateTransitionDefinition : ScriptableObject{
    [Header("Transition Data")]
    public AIStateType fromAIState;
    public AIStateType toAIState;

    public virtual bool IsValidStateTransition(AIStateType aIState){
        if(fromAIState == aIState) return true;
        return false;
    }

    public abstract bool AttemptTriggerTransition(AIStateMachine stateMachine, AIStateType aIState);
}