using System.Collections;
using UnityEngine;

public class PatrolTimeExpired : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;

    private CoroutineContainer patrolTimeCoroutineContainer;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        }

        if(patrolTimeCoroutineContainer == null){
            patrolTimeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        }
    }

    public override void StartTransitionTimer(){
        float totalPatrolTime = Random.Range(aICharacterDataSO.patrolTimeInSeconds.minValue, aICharacterDataSO.patrolTimeInSeconds.maxValue);
        patrolTimeCoroutineContainer.SetCoroutine(PatrolTimerCoroutine(patrolTimeCoroutineContainer, totalPatrolTime));
        aIStateMachine.StartNewCoroutineContainer(patrolTimeCoroutineContainer);
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        if(patrolTimeCoroutineContainer.IsCoroutineRunning()){
            return false;
        }
        
        return true;
    }

    public override void ResetConditionJob(){
        if(patrolTimeCoroutineContainer.IsCoroutineRunning()){
            patrolTimeCoroutineContainer.Dispose();
        }
    }

    public bool IsPatrolTimerExpired(){
        return !patrolTimeCoroutineContainer.IsCoroutineRunning();
    }

    private IEnumerator PatrolTimerCoroutine(CoroutineContainer coroutineContainer, float totalPatrolTime){
        yield return new WaitForSeconds(totalPatrolTime);

        coroutineContainer.Dispose();
    }
}