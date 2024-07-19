public class TargetInLOS : AIStateTransitionConditionJob{
    private AILineOfSight aILineOfSight;

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        if(aILineOfSight == null){
            aILineOfSight = aIStateMachine.GetAILineOfSight();
        }

        return aILineOfSight.IsCurrentTargetInLOS();
    }
}
