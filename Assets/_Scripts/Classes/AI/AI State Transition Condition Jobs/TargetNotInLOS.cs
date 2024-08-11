public class TargetNotInLOS : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AIAttack aIAttack;
    private AILineOfSight aILineOfSight;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aILineOfSight = aIStateMachine.GetAILineOfSight();
            aIAttack = aIStateMachine.GetAIAttack();
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return !aILineOfSight.IsTargetValid(aIAttack.GetCurrentTargetTransform());
    }

    public override void ResetConditionJob(){
        
    }
}