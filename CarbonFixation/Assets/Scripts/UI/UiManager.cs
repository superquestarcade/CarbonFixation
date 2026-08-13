using Locomotion;
using UnityEngine;

namespace UI
{
	public class UiManager : MonoBehaviourSingleton<UiManager>
	{
		private UiPlayerHud playerHud;
		private PlayerHealth playerHealth;

		public void RegisterPlayerHud(UiPlayerHud _playerHud)
		{
			if (playerHud != null)
			{
				Debug.LogError("UiManager.RegisterPlayerHud already exists");
				return;
			}
			playerHud = _playerHud;
			LinkHealthHud();
		}

		public void UnregisterPlayerHud(UiPlayerHud _playerHud)
		{
			if (playerHud == null)
			{
				Debug.LogError("UiManager.UnregisterPlayerHud no hud found");
				return;
			}

			UnlinkHealthHud();
			playerHud = null;
		}

		public void RegisterPlayerHealth(PlayerHealth _playerHealth)
		{
			if (playerHealth != null)
			{
				Debug.LogError("UiManager.RegisterPlayerHealth already exists");
				return;
			}
			playerHealth = _playerHealth;
			LinkHealthHud();
		}

		public void UnregisterPlayerHealth(PlayerHealth _playerHealth)
		{
			if (playerHealth == null)
			{
				Debug.LogError("UiManager.UnregisterPlayerHealth no player health found");
				return;
			}
			UnlinkHealthHud();
			playerHealth = null;
		}

		private void LinkHealthHud()
		{
			if (playerHud == null || playerHealth == null) return;
			playerHealth.OnHealthChange.AddListener(playerHud.OnHealthChange);
			playerHud.OnSetBaseHealth(playerHealth.BaseHealth);
			playerHealth.OnMeatActive.AddListener(playerHud.OnSetMeat);
			playerHealth.OnRegenActive.AddListener(playerHud.OnSetHealthGain);
			playerHealth.OnLossActive.AddListener(playerHud.OnSetHealthLoss);
		}

		private void UnlinkHealthHud()
		{
			if(playerHealth != null && playerHud != null) playerHealth.OnHealthChange.RemoveListener(playerHud.OnHealthChange);
		}
	}
}