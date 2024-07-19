public class TargetInLOS : AIStateTransitionConditionJob{
    private AILineOfSight aILineOfSight;

    public override void SetupConditionJob(AIStateMachine aIStateMachine){
        if(aILineOfSight == null){
            aILineOfSight = aIStateMachine.GetAILineOfSight();
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return aILineOfSight.IsCurrentTargetInLOS();
    }
}