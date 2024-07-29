using UnityEngine;

public class AIIdleState : BaseState<AIStateType>{
    private AIMover aIMover;
    private AIStateMachine aIStateMachine;

    public AIIdleState(AIStateType key) : base(key){

    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aIMover = aIStateMachine.GetAIMover();
    }

    public override void EnterState(){
        if(aIMover != null){
            aIMover.SetNavMeshIsStopped(true);
        }
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        if(aIMover != null){
            aIMover.SetNavMeshIsStopped(false);
        }
    }

    public override AIStateType GetNextState(){
        return AIStateType.Idle;
    }

    public override void OnTriggerEnter(Collider other){
        
    }

    public override void OnTriggerExit(Collider other){
        
    }

    public override void OnTriggerStay(Collider other){
        
    }
}