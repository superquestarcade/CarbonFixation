using System;
using UnityEngine;
using World;

namespace Data
{
	[Serializable]
	public struct PointOfInterestGenData
	{
		public PointOfInterest poiPrefab;
		public float heightClearanceRadius;
		public float heighClearanceFalloff;
	}
}