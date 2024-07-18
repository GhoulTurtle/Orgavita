using System;
using System.Collections.Generic;
using UnityEngine;

 public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum{
	protected Dictionary<EState, BaseState<EState>> States = new();

	protected BaseState<EState> CurrentState;

	protected bool IsTransitioningState = false;

	private void Start() {
		CurrentState?.EnterState();
	}

	private void Update() {
		if (CurrentState == null) return;
		DetectStateChange();
	}

	protected virtual void DetectStateChange(){
		EState nextStateKey = CurrentState.GetNextState();

		if(IsTransitioningState) return;

		if(nextStateKey.Equals(CurrentState.StateKey)){
			CurrentState.UpdateState();
		}
		else{
			TransitionToState(nextStateKey);
		}
	}

	private void OnTriggerEnter(Collider other) {
		CurrentState.OnTriggerEnter(other);
	}

	private void OnTriggerStay(Collider other) {
		CurrentState.OnTriggerStay(other);
	}

	private void OnTriggerExit(Collider other) {
		CurrentState.OnTriggerExit(other);
	}

	protected void TransitionToState(EState nextStateKey){
		IsTransitioningState = true;
		CurrentState.ExitState();
		CurrentState = States[nextStateKey];
		CurrentState.EnterState();
		IsTransitioningState = false;
	}
}