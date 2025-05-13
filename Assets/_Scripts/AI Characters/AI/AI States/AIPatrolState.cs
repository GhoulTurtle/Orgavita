using System.Collections;
using UnityEngine;

public class AIPatrolState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;
    private AIPatrol aIPatrol;
    private AIMover aIMover;

    private CoroutineContainer patrolIdleCoroutineContainer;

    public AIPatrolState(AIStateType key) : base(key){

    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aIPatrol = aIStateMachine.GetAIPatrol();
        aIMover = aIStateMachine.GetAIMover();
        aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        
        patrolIdleCoroutineContainer = new CoroutineContainer(aIStateMachine);
    }

    public override void EnterState(){

        patrolIdleCoroutineContainer.OnCoroutineDisposed += GetNextPatrolPoint;

        GetNextPatrolPoint(null, null);
    }

    public override void UpdateState(){
        if(aIPatrol.AtGoalPoint(1.5f) && !patrolIdleCoroutineContainer.IsCoroutineRunning()){
            float pointIdleTime = Random.Range(aICharacterDataSO.patrolIdleTimeInSeconds.minValue, aICharacterDataSO.patrolIdleTimeInSeconds.maxValue);
            
            patrolIdleCoroutineContainer.SetCoroutine(PatrolPointIdleCoroutine(patrolIdleCoroutineContainer, pointIdleTime));
            aIStateMachine.StartNewCoroutineContainer(patrolIdleCoroutineContainer);
        }
    }

    private void GetNextPatrolPoint(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        aIPatrol.GenerateGoalPoint();
        aIMover.SetDestination(aIPatrol.GetCurrentGoalPoint());        
    }

    public override void ExitState(){
        patrolIdleCoroutineContainer.OnCoroutineDisposed -= GetNextPatrolPoint;

        aIMover.SetDestination(aIStateMachine.transform.position);
    }

    private IEnumerator PatrolPointIdleCoroutine(CoroutineContainer coroutineContainer, float waitTime){
        yield return new WaitForSeconds(waitTime);
        coroutineContainer.Dispose();
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
