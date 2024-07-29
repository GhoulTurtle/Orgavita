using System;
using System.Collections.Generic;
using UnityEngine;

public class AIAggression : MonoBehaviour{
    [Header("Required References")]
    [SerializeField] private NPCHealth nPCHealth;
    
    private Dictionary<IDamagable, float> aggressorsDictionary = new Dictionary<IDamagable, float>();

    public Action OnTopAggressorChanged;
    public Action<IDamagable> OnAggressorRemoved;

    private IDamagable currentTopAggressor;
    private Action topAggressorDeathAction;

    private void Awake() {
        if(nPCHealth == null) return;

        nPCHealth.OnCharacterDamaged += CharacterDamaged;    
        nPCHealth.OnCharacterHealed += CharacterHealed;    
        nPCHealth.OnCharacterDeath += CharacterDeath;
    }

    private void OnDestroy() {
        if(nPCHealth == null) return;

        nPCHealth.OnCharacterDamaged -= CharacterDamaged;    
        nPCHealth.OnCharacterHealed -= CharacterHealed;    
        nPCHealth.OnCharacterDeath -= CharacterDeath;

        if(topAggressorDeathAction != null){
            topAggressorDeathAction -= TopAggressorDeath;
        }
    }

    public IDamagable GetTopAggressor(){
        return currentTopAggressor;
    }

    private void CharacterDeath(){
        aggressorsDictionary.Clear();
        if(currentTopAggressor != null){
            RemoveTopAggressor();
        }
    }

    private void CharacterDamaged(float damageAmount, IDamagable damageDealer){
        if(damageDealer == null) return;

        if(IsTrackedAggressor(damageDealer)){
            aggressorsDictionary[damageDealer] += damageAmount;
        }
        else{
            aggressorsDictionary.Add(damageDealer, damageAmount);
        }

        if(damageDealer == currentTopAggressor) return;
        CheckTopAggressor();
    }

    private void CharacterHealed(float healAmount, IDamagable healerEntity){
        if(healerEntity == null) return;

        if(IsTrackedAggressor(healerEntity)){
            aggressorsDictionary.Remove(healerEntity);
        }

        if(currentTopAggressor == healerEntity){
            OnAggressorRemoved?.Invoke(healerEntity);
            CheckTopAggressor();
        }
    }

    private void TopAggressorDeath(){
        RemoveTopAggressor();

        CheckTopAggressor();
    }

    private void CheckTopAggressor(){
        if(aggressorsDictionary.Count == 0) return;

        List<IDamagable> keys = new(aggressorsDictionary.Keys);

        IDamagable highestAggressor = null;
        float highestAggression = 0f;

        if(currentTopAggressor != null){
            highestAggressor = currentTopAggressor;
            highestAggression = aggressorsDictionary[currentTopAggressor];
        }

        for (int i = 0; i < aggressorsDictionary.Count; i++){
            IDamagable currentKey = keys[i];
            float aggressionValue = aggressorsDictionary[currentKey];

            if(aggressionValue > highestAggression){
                highestAggression = aggressionValue;
                highestAggressor = currentKey;
            }
        }

        if(highestAggressor != currentTopAggressor){
            AssignCurrentTopAggressor(highestAggressor);
        }
    }

    private void AssignCurrentTopAggressor(IDamagable aggressor){
        currentTopAggressor = aggressor;
        topAggressorDeathAction = currentTopAggressor.GetDeathAction();
        topAggressorDeathAction += TopAggressorDeath;
        OnTopAggressorChanged?.Invoke();
    }

    private void RemoveTopAggressor(){
        OnAggressorRemoved?.Invoke(currentTopAggressor);
        topAggressorDeathAction -= TopAggressorDeath;
        currentTopAggressor = null;
        topAggressorDeathAction = null;
    }

    private bool IsTrackedAggressor(IDamagable aggressor){
        return aggressorsDictionary.ContainsKey(aggressor);
    }
}