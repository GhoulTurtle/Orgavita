using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Lunge Attack Definition", fileName = "NewLungeAttackDefinitionSO")]
public class LungeAttackDefinitionSO : BaseAIAttackDefinitionSO{
    [Header("Lunge Attack Variables")]
    [Range(1, 5)] public int lungeAmount = 1;
    [Range(0.1f, 30f)] public float lungeForce;
    [Range(0.1f, 30f)] public float lungeSpeed;
    public float lungleDamageRangeRadius = 1f;
    public int maxLungeTargetsHit = 5;

    [Header("Lunge Attack Times"), Tooltip("Scaled with the attack speed.")]    
    public float lungeChargeTime;
    public float lungePerformTime;
    public float lungeRecoveryTime;
    public float lungeWindupTime;

    private const float LUNGE_CHARGE_TIME_RATE = 0.6f;
    private const float LUNGE_PERFORM_TIME_RATE = 0.15f;
    private const float LUNGE_RECOVERY_TIME_RATE = 0.15f;
    private const float LUNGE_WINDUP_RATE = 0.1f;

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
        NPCHealth nPCHealth = aIStateMachine.GetNPCHealth();

        CoroutineContainer lungeCoroutineContainer = new CoroutineContainer(aIStateMachine);
        lungeCoroutineContainer.SetCoroutine(LungeAttackCoroutine(lungeCoroutineContainer, aIMover, aILineOfSight, nPCHealth, targetTransform));
        aIStateMachine.StartNewCoroutineContainer(lungeCoroutineContainer);
    }

    private IEnumerator LungeAttackCoroutine(CoroutineContainer coroutineContainer, AIMover aIMover, AILineOfSight aILineOfSight, NPCHealth nPCHealth, Transform targetTransform){
        AITargetDefinition aITargetDefinition = aILineOfSight.GetAITargetDefinition();
        
        yield return new WaitForSeconds(lungeWindupTime);

        Vector3 lungeDir = new();

        Collider[] hitColliders = new Collider[maxLungeTargetsHit];

        for (int i = 0; i < lungeAmount; i++){
            OnAttackStart?.Invoke();
            float currentLungeTime = 0f;

            while(currentLungeTime <= lungeChargeTime){
                lungeDir = (targetTransform.position - aIMover.transform.position).normalized;
                aIMover.ApplyForce(lungeDir, lungeForce, lungeSpeed);
            
                //If within 1 unit away from the target then break from the loop and trigger the attack
                if(Vector3.Distance(aIMover.transform.position, targetTransform.position) <= 1.5f){
                    break;
                }

                currentLungeTime += Time.deltaTime;

                yield return null; 
            }

            //Perform attack
            aIMover.SetNavMeshIsStopped(true);
            OnAttackPerformed?.Invoke();
            if(Physics.OverlapSphereNonAlloc(aIMover.transform.position + lungeDir, lungleDamageRangeRadius, hitColliders, aITargetDefinition.targetLayerMask, QueryTriggerInteraction.Ignore) != 0){
                for (int j = 0; j < hitColliders.Length; j++){
                    if(hitColliders[j] == null) continue;
                    if(!hitColliders[j].TryGetComponent(out IDamagable damagable)) continue;
                    damagable.TakeDamage(attackDamage, nPCHealth.GetRandomDamagableBodyPart(), aIMover.transform.position);
                }
            }
            yield return new WaitForSeconds(lungePerformTime);
            

            //Recovering
            OnAttackRest?.Invoke();
            yield return new WaitForSeconds(lungeRecoveryTime);
            aIMover.SetNavMeshIsStopped(false);
        }

        aIMover.ResetNavMeshAgentSpeed();
        OnAttackEnd?.Invoke();
        coroutineContainer.Dispose();
    }
}