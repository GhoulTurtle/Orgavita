using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable{
	[Header("Health Variables")]
	[SerializeField] private float maxHealth;
	[SerializeField, Range(0f, 1f)] private float healthyPercentCutoff = 0.8f;
	[SerializeField, Range(0f, 1f)] private float injuredPercentCutoff = 0.5f;
	[SerializeField, Range(0f, 1f)] private float warningPercentCutoff = 0.3f;
	[SerializeField, Range(0f, 1f)] private float criticalPercentCutoff = 0.1f;

	public Action OnStartHealingOverTime;
	public Action OnFinishedHealingOverTime;
	public Action OnDamagedEvent;
	public EventHandler OnHealedEvent;
	public EventHandler OnMaxIncreasedEvent;
	public Action OnDeathEvent;
	public EventHandler<HealthStateChangedEventArgs> OnHealthStateChanged;
	public class HealthStateChangedEventArgs : EventArgs{
		public HealthState healthState;

		public HealthStateChangedEventArgs(HealthState _healthState){
			healthState = _healthState;
		}
	}

	private HealthState currentHealthState;
	private float currentHealth;

	private IEnumerator currentHealOverTimeJob;

	private float healthyStatusCutoff;
	private float injuredStatusCutoff;
	private float warningStatusCutoff;
	private float criticalStatusCutoff;

	private void Start(){
		currentHealth = maxHealth;

		SetStatusCutoff();
		OnHealthStateChanged?.Invoke(this, new HealthStateChangedEventArgs(currentHealthState));
	}

	private void OnDestroy() {
		StopAllCoroutines();
	}

	public void TakeDamage(float damage, IDamagable damageDealer, Vector3 damagePoint){
		currentHealth -= damage;

		if(currentHealth <= 0){
			OnDeathEvent?.Invoke();
			return;
		}

		OnDamagedEvent?.Invoke();
		UpdateHealthState();
	}

	public void HealHealth(float amount){
		currentHealth += amount;
		if(currentHealth > maxHealth) currentHealth = maxHealth;
		OnHealedEvent?.Invoke(this, EventArgs.Empty);
		UpdateHealthState();
	}

	public void HealHealthOverTime(float totalAmount, float totalTime){
		OnStartHealingOverTime?.Invoke();
		currentHealOverTimeJob = HealOverTimeJob(totalAmount, totalTime);
		StartCoroutine(currentHealOverTimeJob);
	}

	public bool IsHealthFull(){
		return currentHealth == maxHealth;
	}

	public bool IsActiveHealOverTimeJob(){
		return currentHealOverTimeJob != null;
	}

    public Transform GetDamageableTransform(){
        return transform;
    }

	public HealthState GetCurrentHealthState(){
		return currentHealthState;
	}

	public void IncreaseMaxHealth(float amount){
		if(IsHealthFull()){
			currentHealth += amount;
		}
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

	private IEnumerator HealOverTimeJob(float totalAmount, float waitTimeBetweenHealPoints){
		while(totalAmount > 0){
			HealHealth(1);
			yield return new WaitForSeconds(waitTimeBetweenHealPoints);
			totalAmount -= 1;
		}

		OnFinishedHealingOverTime();
		currentHealOverTimeJob = null;
	}

    public Action GetDeathAction(){
        return OnDeathEvent;
    }
}