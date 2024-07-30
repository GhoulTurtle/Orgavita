using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOutAttackRange : AIStateTransitionConditionJob{
    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return false;
    }

    public override void ResetConditionJob(){

    } 
}
