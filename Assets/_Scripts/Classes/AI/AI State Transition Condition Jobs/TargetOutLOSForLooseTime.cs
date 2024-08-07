using System.Collections;
using UnityEngine;

public class TargetOutLOSForLooseTime : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;
    private AILineOfSight aILineOfSight;
    private AIAttack aIAttack;

    private CoroutineContainer looseTimeCoroutineContainer;

    private bool lostTarget = false;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aILineOfSight = aIStateMachine.GetAILineOfSight();
            aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
            aIAttack = aIStateMachine.GetAIAttack();
        }

        if(looseTimeCoroutineContainer == null){
            looseTimeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        if(aILineOfSight.IsTargetValid(aIAttack.GetCurrentTargetTransform())){
            looseTimeCoroutineContainer.Dispose();
            lostTarget = false;
            return false;
        }

        if(!looseTimeCoroutineContainer.IsCoroutineRunning() && !lostTarget){
            //Start the loose timer
            looseTimeCoroutineContainer.SetCoroutine(LooseTimeCoroutine(looseTimeCoroutineContainer, aICharacterDataSO.looseTimeInSeconds));
            aIStateMachine.StartNewCoroutineContainer(looseTimeCoroutineContainer);
            return false;
        }

        return lostTarget;
    }

    public override void ResetConditionJob(){
        if(looseTimeCoroutineContainer.IsCoroutineRunning()){
            looseTimeCoroutineContainer.Dispose();
        }

        lostTarget = false;
    }

    private IEnumerator LooseTimeCoroutine(CoroutineContainer coroutineContainer, float looseTime){
        yield return new WaitForSeconds(looseTime);

        lostTarget = true;
        coroutineContainer.Dispose();
    }
}