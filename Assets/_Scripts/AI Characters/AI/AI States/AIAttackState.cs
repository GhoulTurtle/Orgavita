using UnityEngine;

public class AIAttackState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private NPCHealth nPCHealth;

    public AIAttackState(AIStateType key) : base(key){

    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        nPCHealth = aIStateMachine.GetNPCHealth();
    }

    public override void EnterState(){
        PerformBestAttack();
    }

    public override void UpdateState(){

    }

    public override void ExitState(){
        ClearCurrentAttack();
    }

    private void PerformBestAttack(){
        SelectBestAttack();

        
    }

    private void ClearCurrentAttack(){
        
    }

    private void SelectBestAttack(){

    }

    private void CurrentAttackFinished(){
        ClearCurrentAttack();
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
