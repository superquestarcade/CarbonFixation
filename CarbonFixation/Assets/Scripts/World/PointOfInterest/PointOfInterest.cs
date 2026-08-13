using System.Collections;
using Animation;
using Cysharp.Threading.Tasks;
using Locomotion;
using UnityEngine;

namespace World
{
	public class PointOfInterest : MonoBehaviourPlus
	{
		[SerializeField] private Animator animator;
		[SerializeField] private string triggerBloomParam = "TriggerBloom";
		[SerializeField] private Transform playerBloomTransform;
		[SerializeField] private float releasePlayerDuration = 12f;
		private bool bloom = false;

		public void OnPlayerEnter(PlayerCharacterController _characterController)
		{
			if(bloom) return;
			_characterController.Motor.SetPositionAndRotation(playerBloomTransform.position, playerBloomTransform.rotation);
			var animController = _characterController.GetComponent<PlayerAnimationController>();
			animController.Kneel(true);
			animator.SetTrigger(triggerBloomParam);
			StartCoroutine(ReleasePlayerDelay(animController));
		}

		private IEnumerator ReleasePlayerDelay(PlayerAnimationController _playerAnimationController)
		{
			yield return new WaitForSeconds(releasePlayerDuration);
			_playerAnimationController.GetComponent<PlayerHealth>().RemoveHealth(50);
			_playerAnimationController.Kneel(false);
			bloom = true;
		}
	}
}