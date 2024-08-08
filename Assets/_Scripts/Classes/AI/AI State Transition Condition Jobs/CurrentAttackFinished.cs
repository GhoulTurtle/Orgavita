public class CurrentAttackFinished : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AIAttack aIAttack;
    
    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aIAttack = aIStateMachine.GetAIAttack();
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine _aIStateMachine){
        return !aIAttack.GetIsAttacking();
    }

    public override void ResetConditionJob(){

    }
}