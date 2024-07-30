using System.Collections;
using UnityEngine;

public class AISearchState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;
    private AISearch aISearch;
    private AIMover aIMover;

    private TargetOutLOSForSearchTime targetOutLOSForSearchTime;

    private CoroutineContainer coroutineContainer;

    private Vector3 currentSearchPoint;

    public AISearchState(AIStateType key) : base(key){
        
    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aISearch = aIStateMachine.GetAISearch();
        aIMover = aIStateMachine.GetAIMover();
        aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        targetOutLOSForSearchTime = (TargetOutLOSForSearchTime)aIStateMachine.AttemptGetTransitionConditionJob(AIStateTransitionType.TargetOutLOSForSearchTime);
        coroutineContainer = new(aIStateMachine);
    }

    public override void EnterState(){
        coroutineContainer.OnCoroutineDisposed += CalculateNextSearchPoint;
    
        CalculateNextSearchPoint(null, null);
    }

    private void CalculateNextSearchPoint(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        //Just using 1.5 as a placeholder, can update if needed.
        if(Vector3.Distance(aIStateMachine.transform.position, currentSearchPoint) <= 1.5f){
            currentSearchPoint = aISearch.GetNextSearchVector(aIStateMachine.transform.position, targetOutLOSForSearchTime.GetCurrentSearchTimeProgress());
            aIMover.SetDestination(currentSearchPoint);
        }

        coroutineContainer.SetCoroutine(SearchPointIdleCoroutine(coroutineContainer));
        aIStateMachine.StartNewCoroutineContainer(coroutineContainer);
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        coroutineContainer.OnCoroutineDisposed -= CalculateNextSearchPoint;
        
        if(coroutineContainer.IsCoroutineRunning()){
            coroutineContainer.Dispose();
        }
    }

    private IEnumerator SearchPointIdleCoroutine(CoroutineContainer coroutineContainer){
        yield return new WaitForSeconds(Random.Range(aICharacterDataSO.searchIdleTimeInSeconds.minValue, aICharacterDataSO.searchIdleTimeInSeconds.maxValue));
        coroutineContainer.Dispose();
    }

    public override AIStateType GetNextState(){
        return AIStateType.Search;
    }

    public override void OnTriggerEnter(Collider other){
        
    }

    public override void OnTriggerExit(Collider other){
        
    }

    public override void OnTriggerStay(Collider other){
        
    }
}
