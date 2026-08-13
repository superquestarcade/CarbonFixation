using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UiPlayerHud : MonoBehaviourPlus
	{
		[SerializeField] private Slider healthSlider;
		[SerializeField] private TMP_Text healthText;
		[SerializeField] private Image plusIcon;
		[SerializeField] private Image minusIcon;
		[SerializeField] private Image meatIcon;

		private void Start()
		{
			UiManager.singleton.RegisterPlayerHud(this);
			OnSetMeat(false);
			OnSetHealthGain(false);
			OnSetHealthLoss(false);
		}

		private void OnDestroy()
		{
			UiManager.singleton.UnregisterPlayerHud(this);
		}

		public void OnSetBaseHealth(float _value)
		{
			healthSlider.maxValue = _value;
			healthSlider.value = _value;
			if(healthText!=null) healthText.text = _value.ToString("F0");
		}
		
		public void OnHealthChange(float _value)
		{
			healthSlider.value = _value;
			if(healthText!=null) healthText.text = _value.ToString("F0");
		}

		public void OnSetMeat(bool _meat)
		{
			meatIcon.enabled = _meat;
		}

		public void OnSetHealthGain(bool _value)
		{
			plusIcon.enabled = _value;
		}

		public void OnSetHealthLoss(bool _value)
		{
			minusIcon.enabled = _value;
		}
	}
}