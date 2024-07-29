using System;
using UnityEngine;

public class AIChaseState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AIAggression aIAggression;
    private AILineOfSight aILineOfSight;
    private AIMover aIMover;
    private AISearch aISearch;

    private IDamagable currentChaseTarget;
    private Action currentChaseTargetDeathAction;
    private Transform currentChaseTargetTransform;

    public AIChaseState(AIStateType key) : base(key){
    
    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aIAggression = aIStateMachine.GetAIAggression();
        aILineOfSight = aIStateMachine.GetAILineOfSight();
        aIMover = aIStateMachine.GetAIMover();
        aISearch = aIStateMachine.GetAISearch();
    }

    public override void EnterState(){
        ChooseTarget();
    }

    public override void UpdateState(){
        if(currentChaseTarget == null){
            ChooseTarget();
            return;
        }

        aISearch.SetLastKnownTargetPosition(currentChaseTargetTransform.position);

        //Try to chase the target within 3 units.
        if(aIMover.CheckValidPosition(currentChaseTargetTransform.position, out Vector3 validPosition, 3f)){
            aIMover.SetDestination(validPosition);
        }
        else{
            //If can't chase the target then clear that target
            ClearCurrentTargetReferences();
        }
    }

    public override void ExitState(){
        ClearCurrentTargetReferences();
    }

    private void ClearCurrentTargetReferences(){
        currentChaseTargetTransform = null;
        currentChaseTargetDeathAction -= ClearCurrentTargetReferences;
        currentChaseTargetDeathAction = null;
        currentChaseTarget = null;
    }
    
    private void ChooseTarget(){
        //Set our current target to the top aggressor
        currentChaseTarget = aIAggression.GetTopAggressor();
        
        //If we don't have a top aggressor check if we have a target in our LOS
        if(currentChaseTarget == null){
            currentChaseTarget = aILineOfSight.ChooseRandomTargetInLOS();
        }

        if(currentChaseTarget == null) return;

        currentChaseTargetTransform = currentChaseTarget.GetDamageableTransform();
        currentChaseTargetDeathAction = currentChaseTarget.GetDeathAction();
        currentChaseTargetDeathAction += ClearCurrentTargetReferences;
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