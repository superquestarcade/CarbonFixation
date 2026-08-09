using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace World
{
	[RequireComponent(typeof(BoxCollider))]
	public class ResourceCorridor : MonoBehaviourPlus
	{
		[SerializeField] private BoxCollider boxCollider;
		[SerializeField] private int minResources = 10;
		[SerializeField] private int maxResources = 100;
		private List<Vector3> debugSpawnPositions = new List<Vector3>();

		[SerializeField] private UnityEvent<Vector3> OnSetCorridorSize;

		public void SetCorridorSize(Vector3 _size)
		{
			boxCollider.size = _size;
			OnSetCorridorSize?.Invoke(_size);
		}
		
		public void Generate(System.Random _rng)
		{
			var spawnCount =  _rng.Next(minResources, maxResources);
			for (var i = 0; i < spawnCount; i++)
			{
				var spawnPoint = RandomPointOnBoxColliderXZ(boxCollider, _rng);
				var spawnPos = new Vector3(spawnPoint.x, 0, spawnPoint.y);
				spawnPos.y = WorldManager.singleton.GetHeightAtWorldPosition(spawnPos);
				var spawnObject = Instantiate
				(
					WorldManager.singleton.GetRandomResourceItem(_rng),  
					spawnPos, 
					Quaternion.Euler(new Vector3(0,(float)_rng.NextDouble() * 359,0))
				);
				spawnObject.transform.SetParent(transform);
				// debugSpawnPositions.Add(spawnPos);
			}
		}
		
		private static Vector2 RandomPointOnBoxColliderXZ(BoxCollider _box, System.Random _rng)
		{
			if (_box == null)
			{
				Debug.LogError($"{_box.gameObject.name} has no BoxCollider attached.");
				return Vector2.zero;
			}

			// Random offsets in local space, within [-half, +half] on X and Z
			var halfX = _box.size.x * 0.5f;
			var halfZ = _box.size.z * 0.5f;

			var localX = _box.center.x + (float)(_rng.NextDouble() * 2.0 - 1.0) * halfX;
			var localZ = _box.center.z + (float)(_rng.NextDouble() * 2.0 - 1.0) * halfZ;

			// Keep local Y at the collider's center height — irrelevant for the XZ result,
			// but needed so TransformPoint gives a correct world position
			var localPoint = new Vector3(localX, _box.center.y, localZ);

			var worldPoint = _box.transform.TransformPoint(localPoint);

			return new Vector2(worldPoint.x, worldPoint.z);
		}

		private void OnDrawGizmos()
		{
			/*Gizmos.color = Color.blueViolet;
			Gizmos.DrawWireSphere(boxCollider.bounds.min, 2f);
			Gizmos.color = Color.deepPink;
			Gizmos.DrawWireSphere(boxCollider.bounds.max, 2f);*/
			/*Gizmos.color = Color.yellow;
			foreach(var point in debugSpawnPositions)
				Gizmos.DrawSphere(point, 1f);*/
		}
	}
}