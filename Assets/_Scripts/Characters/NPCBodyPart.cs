using UnityEngine;

public class NPCBodyPart : MonoBehaviour, IDamagable{
    [Header("Required References")]
    [SerializeField] private NPCHealth nPCHealth;
    [SerializeField] private RagdollHandler characterRagdollHandler;
    
    [Header("NPCBodyPart Variables")]
    [SerializeField] private NPCBodyPartType bodyPartType;

    private float bodyPartHealth = 0;

    private bool isGibbed = false;

    private Rigidbody bodyPartRigidbody;
    private CharacterJoint bodyPartJoint;

    private const float RAGDOLL_FORCE_SCALER = 10f; 
    private const float LIMB_HEALTH_SCALER = 0.3f;
    private const float TORSO_HEALTH_SCALER = 0.7f;
    private const float HEAD_HEALTH_SCALER = 0.5f;

    private void Awake() {
        if(nPCHealth != null){
            CalculateBodyPartHealth();
        }

        TryGetComponent(out bodyPartJoint);
        TryGetComponent(out bodyPartRigidbody);
    }

    public void SetupNPCBodyPart(NPCHealth _nPCHealth, RagdollHandler _characterRagdollHandler, NPCBodyPartType _bodyPartType){
        nPCHealth = _nPCHealth;
        characterRagdollHandler = _characterRagdollHandler;
        bodyPartType = _bodyPartType;
    }

    public void TakeDamage(float damageAmount, Vector3 damagePoint){
        if(nPCHealth == null) return;

        float adjustedDamageAmount = damageAmount;

        switch (bodyPartType){
            case NPCBodyPartType.Limb: adjustedDamageAmount = damageAmount * 0.5f;
                break;
            case NPCBodyPartType.Head: adjustedDamageAmount = damageAmount * 1.5f;
                break;
        }

        nPCHealth.DamageCharacter(adjustedDamageAmount);

        if(!nPCHealth.IsDead()) return;
        
        // //Check if this body part should gib
        // AttemptGibBodyPart(damageAmount);

        // if(isGibbed) return;

        ApplyRagdollForce(damageAmount, damagePoint);
    }

    private void AttemptGibBodyPart(float damageAmount){
        bodyPartHealth -= damageAmount;

        if(bodyPartHealth < 0){
            isGibbed = true;
            if(characterRagdollHandler != null){
                characterRagdollHandler.RemoveCharacterRagdollBodyPart(bodyPartJoint);
            }

            //Spawn Gib Function Here
            Debug.Log("Gibbing body part: " + gameObject.name);
        }
    }

    private void ApplyRagdollForce(float damageAmount, Vector3 damagePoint){
        float forceToApply = damageAmount * RAGDOLL_FORCE_SCALER;

        if(bodyPartRigidbody != null){
            Vector3 direction = bodyPartRigidbody.position - damagePoint;

            direction.Normalize();

            bodyPartRigidbody.AddForce(direction * forceToApply, ForceMode.Impulse);
        }
    }

    private void CalculateBodyPartHealth(){
        if(nPCHealth == null) return;

        switch (bodyPartType){
            case NPCBodyPartType.Limb: bodyPartHealth = nPCHealth.GetMaxHealth() * LIMB_HEALTH_SCALER; 
                break;
            case NPCBodyPartType.Body: bodyPartHealth = nPCHealth.GetMaxHealth() * TORSO_HEALTH_SCALER;
                break;
            case NPCBodyPartType.Head: bodyPartHealth = nPCHealth.GetMaxHealth() * HEAD_HEALTH_SCALER;
                break;
        }
    }
}
