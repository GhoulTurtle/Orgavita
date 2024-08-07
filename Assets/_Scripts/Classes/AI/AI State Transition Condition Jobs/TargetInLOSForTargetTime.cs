using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInLOSForTargetTime : AIStateTransitionConditionJob{
    private AIStateMachine aIStateMachine;
    private AILineOfSight aILineOfSight;
    private AICharacterDataSO aICharacterDataSO;

    private List<Collider> validTargetColliders = new List<Collider>();
    private Dictionary<Collider, CoroutineContainer> targetTimerCoroutineDictionary = new Dictionary<Collider, CoroutineContainer>();

    private IDamagable currentTarget;

    public override void SetupConditionJob(AIStateMachine _aIStateMachine){
        aIStateMachine = _aIStateMachine;
        aILineOfSight = aIStateMachine.GetAILineOfSight();
        aICharacterDataSO = aIStateMachine.GetAICharacterDataSO();
    }

    public override bool EvaluateTransitionCondition(AIStateMachine aIStateMachine){
        if(!aILineOfSight.IsValidTargetInLOS()){
            if(targetTimerCoroutineDictionary.Count > 0){
                foreach (CoroutineContainer coroutineContainer in targetTimerCoroutineDictionary.Values){
                    coroutineContainer.Dispose();
                }

                targetTimerCoroutineDictionary.Clear();
            }
            return false;
        }

        aILineOfSight.GetTargets(out Collider[] targetColliders);
        validTargetColliders.Clear();

        for (int i = 0; i < targetColliders.Length; i++){
            validTargetColliders.Add(targetColliders[i]);
            if(targetTimerCoroutineDictionary.ContainsKey(targetColliders[i])){
                continue;
            } 
            
            CoroutineContainer coroutineContainer = new CoroutineContainer(aIStateMachine);
            coroutineContainer.SetCoroutine(TargetTimeCoroutine(coroutineContainer, aICharacterDataSO.spotTimeInSeconds));
            aIStateMachine.StartNewCoroutineContainer(coroutineContainer);
        }

        foreach (Collider targetCollider in targetTimerCoroutineDictionary.Keys){
            if(validTargetColliders.Contains(targetCollider)) continue;
            targetTimerCoroutineDictionary[targetCollider].Dispose();
            targetTimerCoroutineDictionary.Remove(targetCollider);
        }

        foreach (Collider collider in targetTimerCoroutineDictionary.Keys){
            if(targetTimerCoroutineDictionary[collider].IsCoroutineRunning()) continue;
            
            collider.TryGetComponent(out currentTarget);
            if(currentTarget != null){
                return true;
            }
        }

        return false;
    }

    private IEnumerator TargetTimeCoroutine(CoroutineContainer coroutineContainer, float targetTime){
        yield return new WaitForSeconds(targetTime);
        coroutineContainer.Dispose();
    }

    public override void ResetConditionJob(){
        if(targetTimerCoroutineDictionary.Count > 0){
            foreach (CoroutineContainer coroutineContainer in targetTimerCoroutineDictionary.Values){
                coroutineContainer.Dispose();
            }
        }

        targetTimerCoroutineDictionary.Clear();
    }

    public IDamagable GetTargetThatTriggeredTransition(){
        return currentTarget;
    }
}