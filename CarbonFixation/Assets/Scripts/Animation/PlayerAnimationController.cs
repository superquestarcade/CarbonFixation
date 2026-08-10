using System;
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

		private void Update()
		{
			LocomotionUpdate();
		}

		private void LocomotionUpdate()
		{
			animator.SetFloat(speedParamName,
				playerCharacterController.Motor.GroundingStatus.IsStableOnGround
					? (playerCharacterController.Motor.Velocity.magnitude > 0.1f?
						playerCharacterController.Motor.Velocity.magnitude:
						0)
					: 0f);
		}
	}
}