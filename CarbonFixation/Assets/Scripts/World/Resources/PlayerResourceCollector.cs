using System;
using UnityEngine;

namespace World.Resources
{
	public class PlayerResourceCollector : MonoBehaviourPlus
	{
		[SerializeField] private Vector3 offset;
		[SerializeField] private float radius = 0.25f;
		[SerializeField] private LayerMask layerMask;

		private void FixedUpdate()
		{
			ResourceCheck();
		}

		private void ResourceCheck()
		{
			var hits = Physics.OverlapSphere(transform.position + offset, radius, layerMask);
			if (hits.Length > 0)
			{
				foreach (var hit in hits)
				{
					var resourceItem = hit.GetComponent<ResourceItem>();
					if (resourceItem != null) Destroy(resourceItem.gameObject);
				}
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.coral;
			Gizmos.DrawWireSphere(transform.position + offset, radius);
		}
	}
}