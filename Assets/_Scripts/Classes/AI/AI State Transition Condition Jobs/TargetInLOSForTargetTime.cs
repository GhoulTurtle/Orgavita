using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInLOSForTargetTime : AIStateTransitionConditionJob{
    public override void SetupConditionJob(AIStateMachine aIStateMachine){
        
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return false;
    }
}
