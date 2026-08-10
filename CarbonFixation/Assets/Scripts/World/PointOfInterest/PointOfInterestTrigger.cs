using System;
using Locomotion;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace World
{
	public class PointOfInterestTrigger : MonoBehaviourPlus
	{
		[SerializeField] private LayerMask playerLayerMask;
		[SerializeField] private UnityEvent<PlayerCharacterController> OnPlayerEnter;
		private void OnTriggerEnter(Collider _other)
		{
			if (!playerLayerMask.IsInLayerMask(_other.gameObject.layer)) return;
			var pcc = _other.GetComponent<PlayerCharacterController>();
			if(pcc==null) return;
			OnPlayerEnter?.Invoke(pcc);
		}
	}
}