using UnityEngine;

public class AISearchState : BaseState<AIStateType>{
    public AISearchState(AIStateType key) : base(key){
        
    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        
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
