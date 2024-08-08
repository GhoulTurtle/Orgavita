using System.Collections;
using UnityEngine;

public class SearchTimeExpired : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;

    private CoroutineContainer searchTimeCoroutineContainer;

    private float currentSearchProgress;
    private float currentTotalSearchTime;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        if(aIStateMachine == null){
            aIStateMachine = _aIStateMachine;
            aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        }

        if(searchTimeCoroutineContainer == null){
            searchTimeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        }
    }

    public override void StartTransitionTimer(){
        if(searchTimeCoroutineContainer.IsCoroutineRunning()){
            searchTimeCoroutineContainer.Dispose();
        }

        currentTotalSearchTime = Random.Range(aICharacterDataSO.searchTimeInSeconds.minValue, aICharacterDataSO.searchTimeInSeconds.maxValue);
        searchTimeCoroutineContainer.SetCoroutine(SearchTimeCoroutine(searchTimeCoroutineContainer));
        aIStateMachine.StartNewCoroutineContainer(searchTimeCoroutineContainer);
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        if(searchTimeCoroutineContainer.IsCoroutineRunning()){
            return false;
        }

        return true;
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