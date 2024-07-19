using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : StateMachine<AIStateType>{
    [Header("Required References")]
    [SerializeField] private AIStateDataList aIStateDataList;
    [SerializeField] private AICharacterDataSO aICharacterDataSO;
    [SerializeField] private AILineOfSight aILineOfSight;
    [SerializeField] private AIMover aIMover;
    [SerializeField] private NPCHealth nPCHealth;

    [Header("AI Debugging")]
    [SerializeField] private bool showGizmos;
    [SerializeField] private Color attackRangeGizmosColor = Color.red;

    private Dictionary<AIStateTransitionType, AIStateTransitionConditionJob> transitionJobs = new Dictionary<AIStateTransitionType, AIStateTransitionConditionJob>();

    //TODO: Add a coroutine container list with functions so that states and transition jobs can use coroutines.

    private void Awake() {
        if(aIStateDataList != null){
            aIStateDataList.GetAIStateDictionary(States, out AIStateType firstState);
            CurrentState = States[firstState];
        }
    }

    private void OnDestroy() {
        transitionJobs.Clear();
    }

    private void Update() {
        if (CurrentState == null || aIStateDataList == null) return;
        Debug.Log(CurrentState.StateKey);
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
        if(transitionJobs.ContainsKey(transitionType)) return;
        transitionJobs.Add(transitionType, transitionJob);
    }

    public AIStateTransitionConditionJob AttemptGetTransitionConditionJob(AIStateTransitionType transitionType){
        if(!transitionJobs.ContainsKey(transitionType)) return null;
        return transitionJobs[transitionType];
    }

    public AILineOfSight GetAILineOfSight(){
        if(aIStateDataList == null) return null;
        return aILineOfSight;
    }

    public AIMover GetAIMover(){
        if(aIMover == null) return null;
        return aIMover;
    }

    public NPCHealth GetNPCHealth(){
        if(nPCHealth == null) return null;
        return nPCHealth;
    }

    public AICharacterDataSO GetAICharacterDataSO(){
        if(aICharacterDataSO == null) return null;
        return aICharacterDataSO;
    }

    private void OnDrawGizmosSelected() {
        if(!showGizmos || aICharacterDataSO == null) return;

        Gizmos.color = attackRangeGizmosColor;

        Gizmos.DrawWireSphere(transform.position, aICharacterDataSO.attackRange);
    }
}