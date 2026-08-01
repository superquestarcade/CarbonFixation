using UnityEngine;
using World.Resources;

namespace ScriptableObjects
{
	[CreateAssetMenu(menuName = "ScriptableObjects/ResourceSO", fileName = "ResourceSO",  order = 0)]
	public class ResourceItemsSO : ScriptableObject
	{
		[SerializeField] private ResourceItem[] resources;

		public ResourceItem GetRandomResourceItem(System.Random _rng)
		{
			return resources[_rng.Next(0, resources.Length)];
		}
	}
}