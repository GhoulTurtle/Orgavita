using UnityEngine;

public class AIPatrolState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AIPatrol aIPatrol;
    private AIMover aIMover;

    public AIPatrolState(AIStateType key) : base(key){

    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aIPatrol = aIStateMachine.GetAIPatrol();
        aIMover = aIStateMachine.GetAIMover();
    }

    public override void EnterState(){
        //Just using 1.5 as a placeholder, can update if needed.
        if(aIPatrol.AtGoalPoint(1.5f)){
            aIPatrol.GenerateGoalPoint();
        }

        aIMover.SetDestination(aIPatrol.GetCurrentGoalPoint());
    }

    public override void UpdateState(){
        if(aIPatrol.AtGoalPoint(1.5f)){
            aIPatrol.GenerateGoalPoint();
            //Need a coroutine here to pause before we move to the next point
            aIMover.SetDestination(aIPatrol.GetCurrentGoalPoint());
        }
    }

    public override void ExitState(){
        aIMover.SetDestination(aIStateMachine.transform.position);
    }

    public override AIStateType GetNextState(){
        return AIStateType.Patrol;
    }

    public override void OnTriggerEnter(Collider other){
        
    }

    public override void OnTriggerExit(Collider other){
        
    }

    public override void OnTriggerStay(Collider other){
        
    }
}
