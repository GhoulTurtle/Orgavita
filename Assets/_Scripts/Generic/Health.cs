using System;
using UnityEngine;

public class Health : MonoBehaviour{
	[Header("Health Variables")]
	[SerializeField] private float maxHealth;
	[SerializeField, Range(0f, 1f)] private float healthyPercentCutoff = 0.8f;
	[SerializeField, Range(0f, 1f)] private float injuredPercentCutoff = 0.5f;
	[SerializeField, Range(0f, 1f)] private float warningPercentCutoff = 0.3f;
	[SerializeField, Range(0f, 1f)] private float criticalPercentCutoff = 0.1f;

	public EventHandler OnDamagedEvent;
	public EventHandler OnHealedEvent;
	public EventHandler OnMaxIncreasedEvent;
	public EventHandler OnDeathEvent;
	public EventHandler<HealthStateChangedEventArgs> OnHealthStateChanged;
	public class HealthStateChangedEventArgs : EventArgs{
		public HealthState healthState;

		public HealthStateChangedEventArgs(HealthState _healthState){
			healthState = _healthState;
		}
	}

	private HealthState currentHealthState;
	private float currentHealth;

	private float healthyStatusCutoff;
	private float injuredStatusCutoff;
	private float warningStatusCutoff;
	private float criticalStatusCutoff;

	private void Awake(){
		currentHealth = maxHealth;
		SetStatusCutoff();
		OnHealthStateChanged?.Invoke(this, new HealthStateChangedEventArgs(currentHealthState));
	}

	public void TakeDamage(float damage){
		currentHealth -= damage;
		if(currentHealth <= 0){
			OnDeathEvent?.Invoke(this, EventArgs.Empty);
			return;
		}

		OnDamagedEvent?.Invoke(this, EventArgs.Empty);
		UpdateHealthState();
	}

	public void HealHealth(float amount){
		currentHealth += amount;
		if(currentHealth > maxHealth) currentHealth = maxHealth;
		OnHealedEvent?.Invoke(this, EventArgs.Empty);
		UpdateHealthState();
	}

	public void IncreaseMaxHealth(float amount){
		maxHealth += amount;
		OnMaxIncreasedEvent?.Invoke(this, EventArgs.Empty);
		SetStatusCutoff();
		UpdateHealthState();
	}

	private void SetStatusCutoff(){
		healthyStatusCutoff = maxHealth * healthyPercentCutoff;
		injuredStatusCutoff = maxHealth * injuredPercentCutoff;
		warningStatusCutoff = maxHealth * warningPercentCutoff;
		criticalStatusCutoff = maxHealth * criticalPercentCutoff;
	}

	private void UpdateHealthState(){
		HealthState incomingHealthState = HealthState.Healthy;
		switch(currentHealth){
			case float health when health >= healthyStatusCutoff:
				incomingHealthState = HealthState.Healthy;
			break;
			case float health when health >= injuredStatusCutoff: 
				incomingHealthState = HealthState.Injured;
			break;
			case float health when health >= warningStatusCutoff:
				incomingHealthState = HealthState.Warning;
			break;
			case float health when health >= criticalStatusCutoff: 
				incomingHealthState = HealthState.Critical;
			break;
		}

		if(currentHealthState == incomingHealthState) return;

		currentHealthState = incomingHealthState;

		OnHealthStateChanged?.Invoke(this, new HealthStateChangedEventArgs(currentHealthState));
	}
}