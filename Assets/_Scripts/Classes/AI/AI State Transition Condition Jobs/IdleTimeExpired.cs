using System.Collections;
using UnityEngine;

public class IdleTimeExpired : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;

    private CoroutineContainer idleTimeCoroutineContainer;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        }

        if(idleTimeCoroutineContainer == null){
            idleTimeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        }
    }

    public override void StartTransitionTimer(){
        float totalIdleTime = Random.Range(aICharacterDataSO.idleTimeInSeconds.minValue, aICharacterDataSO.idleTimeInSeconds.maxValue);
        idleTimeCoroutineContainer.SetCoroutine(IdleTimeCoroutine(idleTimeCoroutineContainer, totalIdleTime));
        aIStateMachine.StartNewCoroutineContainer(idleTimeCoroutineContainer);
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        if(idleTimeCoroutineContainer.IsCoroutineRunning()){
            return false;
        }
        
        return true;
    }

    public override void ResetConditionJob(){
        if(idleTimeCoroutineContainer.IsCoroutineRunning()){
            idleTimeCoroutineContainer.Dispose();
        }
    }

    private IEnumerator IdleTimeCoroutine(CoroutineContainer coroutineContainer, float totalIdleTime){
        yield return new WaitForSeconds(totalIdleTime);

        coroutineContainer.Dispose();
    }
}
