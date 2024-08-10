using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIAttackState : BaseState<AIStateType>{
    private AIStateMachine aIStateMachine;
    private AICharacterDataSO aICharacterDataSO;
    private AIMover aIMover;
    private AILineOfSight aILineOfSight;
    private AIAttack aIAttack;
    private NPCHealth nPCHealth;

    private BaseAIAttackDefinitionSO currentAttack;

    private CoroutineContainer attackDodgeCoroutineContainer;

    private bool isDodging = false;

    public AIAttackState(AIStateType key) : base(key){

    }

    public override void SetupState(StateMachine<AIStateType> stateMachine){
        aIStateMachine = (AIStateMachine)stateMachine;
        aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
        aIMover = aIStateMachine.GetAIMover();
        aILineOfSight = aIStateMachine.GetAILineOfSight();
        aIAttack = aIStateMachine.GetAIAttack();
        nPCHealth = aIStateMachine.GetNPCHealth();

        attackDodgeCoroutineContainer = new CoroutineContainer(aIStateMachine);
    }

    public override void EnterState(){
        PerformBestAttack();
    }

    public override void UpdateState(){
        if(currentAttack == null && !isDodging){
            PerformBestAttack();
        }
    }

    public override void ExitState(){
        ClearCurrentAttack();
        isDodging = false;
    }

    private void PerformBestAttack(){
        SelectBestAttack();

        if(currentAttack != null){
            currentAttack.PerformAttack(aIStateMachine, aIAttack.GetCurrentTargetTransform());
        }
    }

    private void ClearCurrentAttack(){
        if(currentAttack != null){
            currentAttack.OnAttackEnd -= CurrentAttackFinished;
            currentAttack = null;
        }
    }

    private void SelectBestAttack(){
        Transform currentTargetTransform = aIAttack.GetCurrentTargetTransform();

        if (currentTargetTransform == null) return;
        Vector3 currentTargetPos = currentTargetTransform.position;

        float distanceFromTarget = Vector3.Distance(aIStateMachine.transform.position, currentTargetPos);
        float healthFraction = nPCHealth.GetHealthFraction();

        currentAttack = aIAttack.GetBestAttack(distanceFromTarget, healthFraction);
        if(currentAttack == null) return;
        currentAttack.OnAttackEnd += CurrentAttackFinished;
        aIAttack.SetIsAttacking(true);
    }

    private void CurrentAttackFinished(){
        aIAttack.SetIsAttacking(false);

        //Start the attack dodge coroutine
        float amountToDodgeInSeconds = Random.Range(currentAttack.attackDodgePeriodInSeconds.minValue, currentAttack.attackDodgePeriodInSeconds.maxValue);

        attackDodgeCoroutineContainer.SetCoroutine(AttackDodgeCoroutine(amountToDodgeInSeconds));
        aIStateMachine.StartNewCoroutineContainer(attackDodgeCoroutineContainer);
        isDodging = true;

        ClearCurrentAttack();
    }

    private IEnumerator AttackDodgeCoroutine(float amountToWaitInSeconds){
        float currentTime = 0f;

        Vector3 dodgePos = Vector3.zero;

        while(currentTime < amountToWaitInSeconds){
            currentTime += Time.deltaTime;

            if(dodgePos == Vector3.zero || Vector3.Distance(aIStateMachine.transform.position, dodgePos) < 1.5f){
                float dodgeRange = Random.Range(aICharacterDataSO.dodgeRange.minValue, aICharacterDataSO.dodgeRange.maxValue);
                dodgePos = aIAttack.GetNextAttackDodgeVector(aIStateMachine.transform.position, dodgeRange);
                aIMover.SetDestination(dodgePos);
            }

            yield return null;
        }

        isDodging = false;
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
