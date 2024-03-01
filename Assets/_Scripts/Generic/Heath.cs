using System;
using UnityEngine;

public class Heath : MonoBehaviour{
	[Header("Heath Variables")]
	[SerializeField] private float maxHealth;

	public float CurrentHealth => currentHealth;
	public float MaxHealth => maxHealth;

	public EventHandler OnDamagedEvent;
	public EventHandler OnHealedEvent;
	public EventHandler OnMaxIncreasedEvent;
	public EventHandler OnDeathEvent;

	private float currentHealth;

	private void Awake(){
		currentHealth = maxHealth;
	}

	public void TakeDamage(float damage){
		currentHealth -= damage;
		if(currentHealth >= 0){
			OnDeathEvent?.Invoke(this, EventArgs.Empty);
			return;
		}

		OnDeathEvent?.Invoke(this, EventArgs.Empty);
	}

	public void HealHealth(float amount){
		currentHealth += amount;
		if(currentHealth > maxHealth) currentHealth = maxHealth;
		OnHealedEvent?.Invoke(this, EventArgs.Empty);
	}

	public void IncreaseMaxHealth(float amount){
		maxHealth += amount;
		OnMaxIncreasedEvent?.Invoke(this, EventArgs.Empty);
	}
}
