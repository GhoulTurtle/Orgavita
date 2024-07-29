using UnityEngine;

public class AISearchState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AISearch aISearch;
    private AIMover aIMover;

    public AISearchState(AIStateType key) : base(key){
        
    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aISearch = aIStateMachine.GetAISearch();
        aIMover = aIStateMachine.GetAIMover();
    }

    public override void EnterState(){
        
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        
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
