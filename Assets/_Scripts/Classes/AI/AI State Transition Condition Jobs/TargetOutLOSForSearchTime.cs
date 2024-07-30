using System.Collections;
using UnityEngine;

public class TargetOutLOSForSearchTime : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;
    private AILineOfSight aILineOfSight;

    private CoroutineContainer searchTimeCoroutineContainer;

    private float currentSearchProgress;
    private float currentTotalSearchTime;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aILineOfSight = aIStateMachine.GetAILineOfSight();
            aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        }

        if(searchTimeCoroutineContainer == null){
            searchTimeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        }
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        if(!searchTimeCoroutineContainer.IsCoroutineRunning()){
            currentTotalSearchTime = Random.Range(aICharacterDataSO.searchTimeInSeconds.minValue, aICharacterDataSO.searchTimeInSeconds.maxValue);
            searchTimeCoroutineContainer.SetCoroutine(SearchTimeCoroutine(searchTimeCoroutineContainer));
            aIStateMachine.StartNewCoroutineContainer(searchTimeCoroutineContainer);
        }
        
        //Might be wrong, needs testing.
        if(searchTimeCoroutineContainer.IsCoroutineRunning()){
            return aILineOfSight.ValidTargetInLOS();
        }
        else{
            return true;
        }
    }

    public override void ResetConditionJob(){
        if(searchTimeCoroutineContainer.IsCoroutineRunning()){
            searchTimeCoroutineContainer.Dispose();
        }
    }

    public float GetCurrentSearchTimeProgress(){
        return currentSearchProgress / currentTotalSearchTime;
    }

    private IEnumerator SearchTimeCoroutine(CoroutineContainer coroutineContainer){
        currentSearchProgress = 0f;

        while(currentSearchProgress <= currentTotalSearchTime){
            currentSearchProgress += Time.deltaTime;
            yield return null;
        }

        coroutineContainer.Dispose();
    }
}