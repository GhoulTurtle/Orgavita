using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

/// <summary>
/// Handles heath relating to non playable characters. Handles character death, ragdolling, and gibbing.  
/// </summary>
public class NPCHealth : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private RagdollHandler characterRagdollHandler;
    [SerializeField] private FadeObjectSpawner characterGibletsSpawner;
    [SerializeField] private AICharacterDataSO aICharacterDataSO;

    [Header("Editor Variables")]
    [SerializeField] private List<NPCBodyPart> nPCBodyParts = new List<NPCBodyPart>();
    [SerializeField] private NPCBodyPartType defaultGeneratedBodyPartType;

    [SerializeField] private float currentHealth;

    private bool isDead = false;

    public Action<float, IDamagable> OnCharacterDamaged;
    public Action<float, IDamagable> OnCharacterHealed;

    public Action OnCharacterDeath;

    private void Awake() {
        if(aICharacterDataSO != null){
            currentHealth = aICharacterDataSO.maxHealth;
        }
    }

    [ContextMenu("Generate Body Parts List")]
    private void GenerateNPCBodyParts(){
        if(characterRagdollHandler == null && !TryGetComponent(out characterRagdollHandler)){
            Debug.LogWarning("No dragged reference or found reference of a RagdollHandler. Can't generate body parts.");
            return;
        }

        List<GameObject> ragdollGameObjects = characterRagdollHandler.GetRagdollGameObjects(); 

        if(ragdollGameObjects == null){
            Debug.LogWarning("No ragdoll transforms found, make sure that there are rigidbodies components in the RagdollHandler rigidbodies list. Can't generate body parts.");
            return;
        }

        nPCBodyParts.Clear();

        for (int i = 0; i < ragdollGameObjects.Count; i++){
            if(!ragdollGameObjects[i].TryGetComponent(out NPCBodyPart nPCBodyPart)){
                nPCBodyPart= ragdollGameObjects[i].AddComponent<NPCBodyPart>();
            }

            nPCBodyPart.SetupNPCBodyPart(this, characterRagdollHandler, characterGibletsSpawner, defaultGeneratedBodyPartType);
            nPCBodyParts.Add(nPCBodyPart);
        }
    }

    public void DamageCharacter(float amount, IDamagable damagerDealer){
        if(isDead || amount == 0) return;

        currentHealth -= amount;
        if(currentHealth <= 0){
            TriggerCharacterDealth();
            return;
        }

        OnCharacterDamaged?.Invoke(currentHealth, damagerDealer);
    }

    public void HealCharacter(float amount, IDamagable healSource){
        if(isDead || amount == 0) return;

        currentHealth += amount;
        if(aICharacterDataSO != null && currentHealth > aICharacterDataSO.maxHealth){
            currentHealth = aICharacterDataSO.maxHealth;
        }

        OnCharacterHealed?.Invoke(currentHealth, healSource);
    }

    public IDamagable GetRandomDamagableBodyPart(){
        int randomPart = Random.Range(0, nPCBodyParts.Count);
        return nPCBodyParts[randomPart];
    }

    private void TriggerCharacterDealth(){
        isDead = true;
        OnCharacterDeath?.Invoke();
        characterRagdollHandler.EnableCharacterRagdoll();
    }

    public bool IsDead(){
        return isDead;
    }

    public float GetMaxHealth(){
        return aICharacterDataSO != null ? aICharacterDataSO.maxHealth : 0; 
    }

    public float GetCurrentHealth(){
        return currentHealth;
    }

    public float GetHealthFraction(){
        return aICharacterDataSO != null ? currentHealth / aICharacterDataSO.maxHealth : 0; 
    }
    
    public Action GetCharacterDeathAction(){
        return OnCharacterDeath;
    }
}   