using UnityEngine;

public class AIAttackState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AIMover aIMover;
    private AILineOfSight aILineOfSight;
    private AIAttack aIAttack;

    public AIAttackState(AIStateType key) : base(key){

    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aIMover = aIStateMachine.GetAIMover();
        aILineOfSight = aIStateMachine.GetAILineOfSight();
        aIAttack = aIStateMachine.GetAIAttack();
    }

    public override void EnterState(){
        
    }

    public override void UpdateState(){
        
    }

    public override void ExitState(){
        
    }

    public override AIStateType GetNextState(){
        return AIStateType.Attack;
    }

    public override void OnTriggerEnter(Collider other){
        
    }

    public override void OnTriggerExit(Collider other){
        
    }

    public override void OnTriggerStay(Collider other){
        
    }
}
