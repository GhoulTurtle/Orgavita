using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that represents specific condition that is checked for a state transition in a state machine.
/// </summary>
public abstract class AIStateTransitionConditionJob{
    public abstract bool EvaluateTransitionCondition(AIStateMachine aIStateMachine);
}
