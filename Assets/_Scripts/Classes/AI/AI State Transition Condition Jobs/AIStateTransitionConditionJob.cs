/// <summary>
/// A class that represents specific condition that is checked for a state transition in a state machine.
/// </summary>
public abstract class AIStateTransitionConditionJob{
    public virtual void StartTransitionTimer(){

    }
    public abstract bool EvaluateTransitionCondition(AIStateMachine _aIStateMachine);
    public virtual void SetupConditionJob(AIStateMachine aIStateMachine){

    }
    public abstract void ResetConditionJob();
}
