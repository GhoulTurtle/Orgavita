using UnityEngine;

public class TargetInAttackRange : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AIAttack aIAttack;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aIAttack = aIStateMachine.GetAIAttack();
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        float distanceFromTarget = Vector3.Distance(aIStateMachine.transform.position, aIAttack.GetCurrentTargetTransform().position);
        return aIAttack.IsInAttackRange(distanceFromTarget);
    }
    
    public override void ResetConditionJob(){

    }
}
