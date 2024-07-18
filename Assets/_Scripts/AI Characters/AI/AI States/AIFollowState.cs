using UnityEngine;

public class AIFollowState : BaseState<AIStateType>{
    public AIFollowState(AIStateType key) : base(key){

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
        return AIStateType.Follow;
    }

    public override void OnTriggerEnter(Collider other){
        
    }

    public override void OnTriggerExit(Collider other){
        
    }

    public override void OnTriggerStay(Collider other){
        
    }
}
