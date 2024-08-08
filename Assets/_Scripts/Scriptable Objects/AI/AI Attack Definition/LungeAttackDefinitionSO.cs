using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Lunge Attack Definition", fileName = "NewLungeAttackDefinitionSO")]
public class LungeAttackDefinitionSO : BaseAIAttackDefinitionSO{
    [Header("Lunge Attack Variables")]
    [Range(1, 5)] public int lungeAmount = 1;
    [Range(0.1f, 30f)] public float lungeForce;
    
    private float lungeChargeTime;
    private float lungePerformTime;
    private float lungeRecoveryTime;

    private const float LUNGE_CHARGE_TIME_RATE = 0.6f;
    private const float LUNGE_PERFORM_TIME_RATE = 0.15f;
    private const float LUNGE_RECOVERY_TIME_RATE = 0.25f;

    //Must position the aI in a valid position to be able to damage the player, then trigger the attack, and call the OnAttackStart and OnAttackEnd actions.
    public override void PerformAttack(AIStateMachine aIStateMachine, Transform targetTransform){
        lungeChargeTime = attackSpeedInSeconds * LUNGE_CHARGE_TIME_RATE / lungeAmount;
        lungePerformTime = attackSpeedInSeconds * LUNGE_PERFORM_TIME_RATE / lungeAmount;
        lungeRecoveryTime = attackSpeedInSeconds * LUNGE_RECOVERY_TIME_RATE / lungeAmount;

        AIMover aIMover = aIStateMachine.GetAIMover();

        CoroutineContainer lungeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        lungeCoroutineContainer.SetCoroutine(LungeAttackCoroutine(lungeCoroutineContainer, aIMover, targetTransform, lungeChargeTime, lungeRecoveryTime));
        aIStateMachine.StartNewCoroutineContainer(lungeCoroutineContainer);

    }

    private IEnumerator LungeAttackCoroutine(CoroutineContainer coroutineContainer, AIMover aIMover, Transform targetTransform, float chargeTime, float recoveryTime){
        aIMover.SetNavMeshIsStopped(true);
        for (int i = 0; i < lungeAmount; i++){
            OnAttackStart?.Invoke();
            float currentLungeTime = 0f;
            while(currentLungeTime <= lungeChargeTime){
                Vector3 lungeDir = (targetTransform.position - aIMover.transform.position).normalized;
                aIMover.ApplyForce(lungeDir * lungeForce);
                
                currentLungeTime += Time.deltaTime;
                yield return null;   
            }

            //Perform attack
            OnAttackPerformed?.Invoke();
            yield return new WaitForSeconds(lungePerformTime);

            //Recovering
            OnAttackRest?.Invoke();
            yield return new WaitForSeconds(recoveryTime);
        }


        aIMover.SetNavMeshIsStopped(false);
        OnAttackEnd?.Invoke();
        coroutineContainer.Dispose();
    }
}