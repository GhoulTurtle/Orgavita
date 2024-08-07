public class ReachedMovementTarget : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AIMover aIMover;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aIMover = aIStateMachine.GetAIMover();
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return aIMover.IsAgentAtMovementTarget();
    }

    public override void ResetConditionJob(){

    }
}
