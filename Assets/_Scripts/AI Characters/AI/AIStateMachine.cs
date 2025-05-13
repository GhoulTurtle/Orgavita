using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : StateMachine<AIStateType>{
    [Header("Required References")]
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private AILineOfSight aILineOfSight;
    [SerializeField] private AIMover aIMover;
    [SerializeField] private NPCHealth nPCHealth;

    [Header("AI State References")]
    [SerializeField] private AIPatrol aIPatrol;
    [SerializeField] private AIAggression aIAggression;
    [SerializeField] private AISearch aISearch;

    private List<CoroutineContainer> coroutineContainerList = new();

    private void Awake(){
        Activate();
    }

    public virtual void Activate(){
        if (nPCHealth != null){
            nPCHealth.OnCharacterDeath += StopAICoroutines;
        }
    }

    private void OnDestroy(){
        if(nPCHealth != null){
            nPCHealth.OnCharacterDeath -= StopAICoroutines;
        }

        StopAllCoroutineContainers();

        StopAllCoroutines();
    }

    private void StopAllCoroutineContainers(){
        for (int i = 0; i < coroutineContainerList.Count; i++){
            coroutineContainerList[i].Dispose();
        }
    }

    private void Update() {

    }

    public void UpdateState(AIStateType nextStateKey){
        if(IsTransitioningState) return;
        if(nextStateKey.Equals(CurrentState.StateKey)){
			CurrentState.UpdateState();
		}
		else{
			TransitionToState(nextStateKey);
		}
    }



    public void StartNewCoroutineContainer(CoroutineContainer coroutineContainer){
        coroutineContainer.OnCoroutineDisposed += DisposeCoroutineContainer;
        coroutineContainer.StartCoroutine();
        coroutineContainerList.Add(coroutineContainer);
    }

    public void StartNewCoroutineContainer(IEnumerator coroutine){
        CoroutineContainer coroutineContainer = new(this, coroutine);
        coroutineContainer.OnCoroutineDisposed += DisposeCoroutineContainer;
        coroutineContainer.StartCoroutine();
        coroutineContainerList.Add(coroutineContainer);
    }

    private void DisposeCoroutineContainer(object sender, CoroutineContainer.CoroutineDisposedEventArgs e){
        CoroutineContainer coroutineContainer = e.coroutineContainer;
        if(coroutineContainerList.Contains(coroutineContainer)){
            coroutineContainerList.Remove(coroutineContainer);
        }

        coroutineContainer.OnCoroutineDisposed -= DisposeCoroutineContainer;
    }

    private void StopAICoroutines(){
        StopAllCoroutineContainers();
        StopAllCoroutines();
    }


    public AILineOfSight GetAILineOfSight(){
        return aILineOfSight;
    }

    public AIMover GetAIMover(){
        if(aIMover == null) return null;
        return aIMover;
    }

    public AIPatrol GetAIPatrol(){
        if(aIPatrol == null) return null;
        return aIPatrol;
    }

    public AIAggression GetAIAggression(){
        if(aIAggression == null) return null;
        return aIAggression;
    }

    public AISearch GetAISearch(){
        if(aISearch == null) return null;
        return aISearch;
    }

    public NPCHealth GetNPCHealth(){
        if(nPCHealth == null) return null;
        return nPCHealth;
    }

    public AICharacterDataSO GetAICharacterDataSO(){
        if(aICharacterDataSO == null) return null;
        return aICharacterDataSO;
    }
}