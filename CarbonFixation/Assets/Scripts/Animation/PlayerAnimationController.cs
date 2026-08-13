using System;
using Data.Enums;
using Input;
using Locomotion;
using UnityEngine;

namespace Animation
{
	public class PlayerAnimationController : MonoBehaviourPlus
	{
		[SerializeField] private PlayerCharacterController playerCharacterController;
		[SerializeField] private Animator animator;
		[SerializeField] private string speedParamName = "Speed";
		[SerializeField] private string kneelParamName = "Kneel";
		private bool isKneeling = false;

		private void Update()
		{
			LocomotionUpdate();
		}

		private void LocomotionUpdate()
		{
			if(isKneeling) return;
			animator.SetFloat(speedParamName,
				playerCharacterController.Motor.GroundingStatus.IsStableOnGround
					? (playerCharacterController.Motor.Velocity.magnitude > 0.1f?
						playerCharacterController.Motor.Velocity.magnitude:
						0)
					: 0f);
		}

		public void Kneel(bool _isKneeling)
		{
			isKneeling = _isKneeling;
			InputManager.singleton.SetInputState(isKneeling?InputState.Inactive:InputState.Player);
			animator.SetBool(kneelParamName, isKneeling);
		}
	}
}