using System;
using UnityEngine;

public class NPCBodyPart : MonoBehaviour, IDamagable{
    [Header("Required References")]
    [SerializeField] private NPCHealth nPCHealth;
    
    [Header("NPCBodyPart Variables")]
    [SerializeField] private NPCBodyPartType bodyPartType;

    [Header("Body Part Settings")]
    [SerializeField] private bool isPunchable;

    private Action CharacterDeathActionReference;

    private const float ROTATE_FORCE_SCALER = 10f; 

    public void SetupNPCBodyPart(NPCHealth _nPCHealth, NPCBodyPartType _bodyPartType){
        nPCHealth = _nPCHealth;
        bodyPartType = _bodyPartType;
        CharacterDeathActionReference = nPCHealth.GetCharacterDeathAction();
    }

    public void TakeDamage(float damageAmount, IDamagable damageDealer, Vector3 damagePoint){
        if(nPCHealth == null || nPCHealth.IsDead()) return;

        float adjustedDamageAmount = damageAmount;

        switch (bodyPartType){
            case NPCBodyPartType.Limb: adjustedDamageAmount = damageAmount * 0.5f;
                break;
            case NPCBodyPartType.Head: adjustedDamageAmount = damageAmount * 1.5f;
                break;
        }

        nPCHealth.DamageCharacter(adjustedDamageAmount, damageDealer);
    }

    public Transform GetDamageableTransform(){
        return nPCHealth.transform;
    }

    public Action GetDeathAction(){
        if(CharacterDeathActionReference == null) return null;
        
        return CharacterDeathActionReference;
    }
}