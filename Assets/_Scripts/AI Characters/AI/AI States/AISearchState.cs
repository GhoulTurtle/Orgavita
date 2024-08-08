using System.Collections;
using UnityEngine;

public class AISearchState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;
    private AISearch aISearch;
    private AIMover aIMover;

    private SearchTimeExpired targetOutLOSForSearchTime;

    private CoroutineContainer searchCoroutineContainer;

    private Vector3 currentSearchPoint;

    public AISearchState(AIStateType key) : base(key){
        
    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aISearch = aIStateMachine.GetAISearch();
        aIMover = aIStateMachine.GetAIMover();
        aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        searchCoroutineContainer = new(aIStateMachine);
    }

    public override void EnterState(){
        if(targetOutLOSForSearchTime == null){
            targetOutLOSForSearchTime = (SearchTimeExpired)aIStateMachine.AttemptGetTransitionConditionJob(AIStateTransitionType.SearchTimeExpired);
        }

        searchCoroutineContainer.OnCoroutineDisposed += CalculateNextSearchPoint;
    
        CalculateNextSearchPoint(null, null);
    }

    private void CalculateNextSearchPoint(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        if(aIMover.IsAgentAtMovementTarget()){
            currentSearchPoint = aISearch.GetNextSearchVector(aIStateMachine.transform.position, targetOutLOSForSearchTime.GetCurrentSearchTimeProgress());
            aIMover.SetDestination(currentSearchPoint);
        }

        searchCoroutineContainer.SetCoroutine(SearchPointIdleCoroutine(searchCoroutineContainer));
        aIStateMachine.StartNewCoroutineContainer(searchCoroutineContainer);
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        searchCoroutineContainer.OnCoroutineDisposed -= CalculateNextSearchPoint;
        
        if(searchCoroutineContainer.IsCoroutineRunning()){
            searchCoroutineContainer.Dispose();
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
