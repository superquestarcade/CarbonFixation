using System;
using UI;
using UnityEngine;
using UnityEngine.Events;
using World;

namespace Locomotion
{
	public class PlayerHealth : MonoBehaviourPlus
	{
		[SerializeField] private float baseHealth = 100;
		public float BaseHealth => baseHealth;
		private float currentHealth;
		[SerializeField] private float healthRegenRate = 10f;
		private float regenHealth;
		[SerializeField] private PlayerCharacterController playerCharacterController;
		[SerializeField] private float maxMovementHealthLossPerMinute = 100;
		
		public UnityEvent<float> OnHealthChange;
		public UnityEvent OnDeath;
		public UnityEvent<bool> OnRegenActive;
		public UnityEvent<bool> OnLossActive;
		public UnityEvent<bool> OnMeatActive;

		private void Start()
		{
			UiManager.singleton.RegisterPlayerHealth(this);
			currentHealth = baseHealth;
		}

		private void OnDestroy()
		{
			UiManager.singleton.UnregisterPlayerHealth(this);
		}

		private void Update()
		{
			if(currentHealth <= 0) return;
			RegenerateHealthUpdate();
			MovementHealthUpdate();
		}

		public void AddRegenHealth(float _amount)
		{
			regenHealth += _amount;
			OnRegenActive?.Invoke(true);
			OnMeatActive?.Invoke(true);
		}

		public void AddHealth(float _amount)
		{
			currentHealth = Mathf.Clamp(currentHealth + _amount, 0, baseHealth);
			OnHealthChange?.Invoke(currentHealth);
		}

		public void RemoveHealth(float _amount)
		{
			currentHealth = Mathf.Clamp(currentHealth - _amount, 0, baseHealth);
			OnHealthChange?.Invoke(currentHealth);
			if (currentHealth == 0) Die();
		}
		
		private void RegenerateHealthUpdate()
		{
			if (currentHealth <= 0 || regenHealth <= 0)
			{
				OnRegenActive?.Invoke(false);
				OnMeatActive?.Invoke(false);
				return;
			}
			var regenAmount = Mathf.Clamp(healthRegenRate * Time.deltaTime, 0, regenHealth);
			AddHealth(regenAmount);
			regenHealth -= regenAmount;
		}

		private void MovementHealthUpdate()
		{
			if(playerCharacterController.Motor.Velocity.magnitude == 0)
			{
				OnLossActive?.Invoke(false);
				return;
			}
			var maxHealthPerSecond = maxMovementHealthLossPerMinute / 60;
			var normSpeed = playerCharacterController.Motor.Velocity.magnitude /
			                playerCharacterController.MaxStableMoveSpeed;
			var healthReduction = maxHealthPerSecond * normSpeed * Time.deltaTime;
			RemoveHealth(healthReduction);
			OnLossActive?.Invoke(true);
		}

		private void Die()
		{
			OnDeath?.Invoke();
			OnLossActive?.Invoke(false);
			OnRegenActive?.Invoke(false);
			OnMeatActive?.Invoke(false);
			_ = WorldManager.singleton.PlayerDeath();
		}

		public void ResetHealth()
		{
			currentHealth = baseHealth;
			regenHealth = 0;
			OnHealthChange?.Invoke(currentHealth);
		}
	}
}