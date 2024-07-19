using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInAttackRange : AIStateTransitionConditionJob{
    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return false;
    }
}
