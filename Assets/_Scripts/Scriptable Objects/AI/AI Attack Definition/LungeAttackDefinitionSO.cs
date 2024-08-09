using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Lunge Attack Definition", fileName = "NewLungeAttackDefinitionSO")]
public class LungeAttackDefinitionSO : BaseAIAttackDefinitionSO{
    [Header("Lunge Attack Variables")]
    [Range(1, 5)] public int lungeAmount = 1;
    [Range(0.1f, 30f)] public float lungeForce;

    [Header("Lunge Attack Times"), Tooltip("Scaled with the attack speed.")]    
    public float lungeChargeTime;
    public float lungePerformTime;
    public float lungeRecoveryTime;
    public float lungeWindupTime;

    private const float LUNGE_CHARGE_TIME_RATE = 0.45f;
    private const float LUNGE_PERFORM_TIME_RATE = 0.15f;
    private const float LUNGE_RECOVERY_TIME_RATE = 0.15f;
    private const float LUNGE_WINDUP_RATE = 0.25f;

    protected override void OnBaseAttackSpeedValueChanged(){
        lungeChargeTime = AttackSpeedInSeconds * LUNGE_CHARGE_TIME_RATE / lungeAmount;
        lungePerformTime = AttackSpeedInSeconds * LUNGE_PERFORM_TIME_RATE / lungeAmount;
        lungeRecoveryTime = AttackSpeedInSeconds * LUNGE_RECOVERY_TIME_RATE / lungeAmount;
        lungeWindupTime = AttackSpeedInSeconds * LUNGE_WINDUP_RATE / lungeAmount;
    }

    //Must position the aI in a valid position to be able to damage the player, then trigger the attack, and call the OnAttackStart and OnAttackEnd actions.
    public override void PerformAttack(AIStateMachine aIStateMachine, Transform targetTransform){
        AIMover aIMover = aIStateMachine.GetAIMover();
        AILineOfSight aILineOfSight = aIStateMachine.GetAILineOfSight();

        CoroutineContainer lungeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        lungeCoroutineContainer.SetCoroutine(LungeAttackCoroutine(lungeCoroutineContainer, aIMover, aILineOfSight, targetTransform));
        aIStateMachine.StartNewCoroutineContainer(lungeCoroutineContainer);

    }

    private IEnumerator LungeAttackCoroutine(CoroutineContainer coroutineContainer, AIMover aIMover, AILineOfSight aILineOfSight, Transform targetTransform){
        yield return new WaitForSeconds(lungeWindupTime);
        
        for (int i = 0; i < lungeAmount; i++){
            aIMover.SetNavMeshIsStopped(true);
            OnAttackStart?.Invoke();
            float currentLungeTime = 0f;

            while(currentLungeTime <= lungeChargeTime){
                Vector3 lungeDir = (targetTransform.position - aIMover.transform.position).normalized;
                aIMover.ApplyForce(lungeDir * lungeForce);
            
                //If within 1 unit away from the target then break from the loop and trigger the attack
                if(Vector3.Distance(aIMover.transform.position, targetTransform.position) <= 1.5f){
                    break;
                }

                currentLungeTime += Time.deltaTime;

                yield return null; 
            }

            //Perform attack
            OnAttackPerformed?.Invoke();
            yield return new WaitForSeconds(lungePerformTime);

            //Recovering
            OnAttackRest?.Invoke();
            yield return new WaitForSeconds(lungeRecoveryTime);
        }

        aIMover.SetNavMeshIsStopped(false);
        OnAttackEnd?.Invoke();
        coroutineContainer.Dispose();
    }
}