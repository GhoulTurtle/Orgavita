using UnityEngine;

public class AIChaseState : BaseState<AIStateType>{
    public AIChaseState(AIStateType key) : base(key){
    
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
        return AIStateType.Chase;
    }

    public override void OnTriggerEnter(Collider other){
        
    }

    public override void OnTriggerExit(Collider other){
        
    }

    public override void OnTriggerStay(Collider other){
        
    }
}
