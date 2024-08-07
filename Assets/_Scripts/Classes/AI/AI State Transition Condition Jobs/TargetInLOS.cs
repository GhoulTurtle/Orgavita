public class TargetInLOS : AIStateTransitionConditionJob{
    private AILineOfSight aILineOfSight;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aILineOfSight == null){
            aILineOfSight = _aIStateMachine.GetAILineOfSight();
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return aILineOfSight.IsValidTargetInLOS();
    }

    public override void ResetConditionJob(){
        
    }
}