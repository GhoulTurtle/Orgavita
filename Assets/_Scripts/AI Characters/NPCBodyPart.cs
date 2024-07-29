using System;
using UnityEngine;

public class NPCBodyPart : MonoBehaviour, IDamagable{
    [Header("Required References")]
    [SerializeField] private NPCHealth nPCHealth;
    [SerializeField] private RagdollHandler characterRagdollHandler;
    [SerializeField] private FadeObjectSpawner characterGibletsSpawner;
    
    [Header("NPCBodyPart Variables")]
    [SerializeField] private NPCBodyPartType bodyPartType;

    private float bodyPartHealth = 0;

    private bool isGibbed = false;

    private Rigidbody bodyPartRigidbody;
    private CharacterJoint bodyPartJoint;
    private Action CharacterDeathActionReference;

    private const float RAGDOLL_FORCE_SCALER = 10f; 
    private const float GIBLET_FORCE_SCALER = 15f;

    private const float LIMB_HEALTH_SCALER = 0.3f;
    private const float TORSO_HEALTH_SCALER = 0.7f;
    private const float HEAD_HEALTH_SCALER = 0.5f;

    private const int LIMB_GIBLET_AMOUNT = 5;
    private const int TORSO_GIBLET_AMOUNT = 20;
    private const int HEAD_GIBLET_AMOUNT = 10;

    private void Awake() {
        if(nPCHealth != null){
            CalculateBodyPartHealth();
        }

        TryGetComponent(out bodyPartJoint);
        TryGetComponent(out bodyPartRigidbody);
    }

    public void SetupNPCBodyPart(NPCHealth _nPCHealth, RagdollHandler _characterRagdollHandler, FadeObjectSpawner _characterGibletsSpawner, NPCBodyPartType _bodyPartType){
        nPCHealth = _nPCHealth;
        characterRagdollHandler = _characterRagdollHandler;
        characterGibletsSpawner = _characterGibletsSpawner;
        bodyPartType = _bodyPartType;
        CharacterDeathActionReference = nPCHealth.GetCharacterDeathAction();
    }

    public void TakeDamage(float damageAmount, IDamagable damageDealer, Vector3 damagePoint){
        if(nPCHealth == null) return;

        float adjustedDamageAmount = damageAmount;

        switch (bodyPartType){
            case NPCBodyPartType.Limb: adjustedDamageAmount = damageAmount * 0.5f;
                break;
            case NPCBodyPartType.Head: adjustedDamageAmount = damageAmount * 1.5f;
                break;
        }

        nPCHealth.DamageCharacter(adjustedDamageAmount, damageDealer);

        if(!nPCHealth.IsDead()) return;
        
        AttemptGibBodyPart(damageAmount);

        if(isGibbed) return;

        ApplyRagdollForce(damageAmount, damagePoint);
    }

    public Transform GetDamageableTransform(){
        return nPCHealth.transform;
    }

    public Action GetDeathAction(){
        if(CharacterDeathActionReference == null) return null;
        
        return CharacterDeathActionReference;
    }

    private void AttemptGibBodyPart(float damageAmount){
        bodyPartHealth -= damageAmount;

        if(bodyPartHealth > 0) return;

        isGibbed = true;
        if(characterRagdollHandler != null && bodyPartJoint != null){
            characterRagdollHandler.RemoveCharacterRagdollBodyPart(bodyPartJoint);
        }

        int gibletAmount = 0;
        switch (bodyPartType){
            case NPCBodyPartType.Limb: gibletAmount = LIMB_GIBLET_AMOUNT;
                break;
            case NPCBodyPartType.Body: gibletAmount = TORSO_GIBLET_AMOUNT;
                break;
            case NPCBodyPartType.Head: gibletAmount = HEAD_GIBLET_AMOUNT;
                break;
        }

        float forceToApply = damageAmount * GIBLET_FORCE_SCALER;

        characterGibletsSpawner.SpawnRandomFadeObjects(transform.position, gibletAmount, true, forceToApply);

        gameObject.SetActive(false);
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