using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOutLOSForLooseTime : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;
    private AILineOfSight aILineOfSight;
    
    private CoroutineContainer looseTimeCoroutineContainer;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aILineOfSight = aIStateMachine.GetAILineOfSight();
            aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        }

        if(looseTimeCoroutineContainer == null){
            looseTimeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        }
    }

    public override void StartTransitionTimer(){
        looseTimeCoroutineContainer.SetCoroutine(LooseTimeCoroutine(looseTimeCoroutineContainer, aICharacterDataSO.looseTimeInSeconds));
        aIStateMachine.StartNewCoroutineContainer(looseTimeCoroutineContainer);
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        return false;
    }

    public override void ResetConditionJob(){
        if(looseTimeCoroutineContainer.IsCoroutineRunning()){
            looseTimeCoroutineContainer.Dispose();
        }
    }

    private IEnumerator LooseTimeCoroutine(CoroutineContainer coroutineContainer, float looseTime){
        yield return new WaitForSeconds(looseTime);

        coroutineContainer.Dispose();
    }
}
