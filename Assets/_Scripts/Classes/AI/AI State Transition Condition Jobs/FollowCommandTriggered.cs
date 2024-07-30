using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCommandTriggered : AIStateTransitionConditionJob{
    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return false;
    }

    public override void ResetConditionJob(){

    }
}
