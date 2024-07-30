using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : StateMachine<AIStateType>{
    [Header("Required References")]
    [SerializeField] private AIStateDataList aIStateDataList;
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private AILineOfSight aILineOfSight;
    [SerializeField] private AIMover aIMover;
    [SerializeField] private NPCHealth nPCHealth;

    [Header("AI State References")]
    [SerializeField] private AIPatrol aIPatrol;
    [SerializeField] private AIAggression aIAggression;
    [SerializeField] private AIAttack aIAttack;
    [SerializeField] private AISearch aISearch;

    private Dictionary<AIStateTransitionType, AIStateTransitionConditionJob> aIStateTransitionJobs = new Dictionary<AIStateTransitionType, AIStateTransitionConditionJob>();

    private AICommandType currentAICommand = AICommandType.None;
    private ICommandIssuer currentCommandIssuer;

    public Action<AICommandType> OnStartCommand;
    public Action<AICommandType> OnStopCommand;

    private List<CoroutineContainer> coroutineContainerList = new();

    private void Awake() {
        if(aIStateDataList != null){
            aIStateDataList.GetAIStateDictionary(States, this, out AIStateType firstState); 
            CurrentState = States[firstState];

            aIStateDataList.GenerateAIStateTransitionJobDictionary(this);
        }
    }
    
    private void OnDestroy() {
        aIStateTransitionJobs.Clear();
        for (int i = 0; i < coroutineContainerList.Count; i++){
            coroutineContainerList[i].Dispose();
        }

        StopAllCoroutines();
    }

    private void Update() {
        if (CurrentState == null || aIStateDataList == null) return;
		aIStateDataList.NextStateTransition(this);
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

    public void AddStateTransitionConditionJob(AIStateTransitionType transitionType, AIStateTransitionConditionJob transitionJob){
        if(aIStateTransitionJobs.ContainsKey(transitionType)) return;
        Debug.Log("Added: " + transitionType + " to the transition job dictionary");
        aIStateTransitionJobs.Add(transitionType, transitionJob);
        transitionJob.SetupConditionJob(this);
    }

    //Reset the transition jobs for the from state
    public void ResetTransitionConditionJobs(List<AIStateTransitionType> transitionsToReset){
        for (int i = 0; i < transitionsToReset.Count; i++){
            if(!aIStateTransitionJobs.ContainsKey(transitionsToReset[i])) continue;
            aIStateTransitionJobs[transitionsToReset[i]].ResetConditionJob();
        }
    }

    public void StartAICommand(AICommandType aICommandType, ICommandIssuer commandIssuer){
        if(currentAICommand != AICommandType.None){
            currentCommandIssuer.CommandInterrupted(this, currentAICommand);
            StopAICommand();
        }
        
        if(currentCommandIssuer != null && currentCommandIssuer != commandIssuer){
            currentCommandIssuer = null;
            currentCommandIssuer = commandIssuer;
        }

        currentAICommand = aICommandType;
        if(currentAICommand != AICommandType.None){
            OnStartCommand?.Invoke(currentAICommand);
        }
    }

    public void StopAICommand(){
        if(currentAICommand == AICommandType.None) return;

        OnStopCommand?.Invoke(currentAICommand);
        currentAICommand = AICommandType.None;
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

    public AICommandType GetCurrentCommand(){
        return currentAICommand;
    }

    public ICommandIssuer GetCurrentCommandIssuer(){
        return currentCommandIssuer;
    }

    public AIStateTransitionConditionJob AttemptGetTransitionConditionJob(AIStateTransitionType transitionType){
        if(!aIStateTransitionJobs.ContainsKey(transitionType)) return null;
        return aIStateTransitionJobs[transitionType];
    }

    public AILineOfSight GetAILineOfSight(){
        if(aIStateDataList == null) return null;
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

    public AIAttack GetAIAttack(){
        if(aIAttack == null) return null;
        return aIAttack;
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