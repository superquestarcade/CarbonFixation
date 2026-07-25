using System.Collections.Generic;
using Data;
using UnityEngine;

namespace World
{
	public class PointOfInterestGenerator : MonoBehaviourPlus
	{
		[SerializeField] private float baseDistance = 10f;
		[SerializeField] private float distanceVariance = 2f;
		
		[SerializeField] private PoiGenDataSo poiGenDataSo;

		private List<PointOfInterest> activePois = new();
		private List<PointOfInterest> inactivePois = new();

		public void GeneratePois(Vector3 _originPosition, float _radius, System.Random _rng)
		{
			var ringCount = Mathf.FloorToInt(_radius / baseDistance);
			var hexPoints = HexPointGenerator.GenerateHexPoints(ringCount, baseDistance, distanceVariance, _rng);
			var originHex = WorldToHexPosition(_originPosition);
			foreach (var hexPoint in hexPoints)
			{
				var worldHexPosition = originHex + new Vector3(hexPoint.x * baseDistance, 0, hexPoint.y * baseDistance);
				var heightAtPosition = WorldManager.singleton.GetHeightAtWorldPosition(worldHexPosition);
				worldHexPosition.y = heightAtPosition;
				SpawnRandomPoi(originHex, _rng);
			}
		}

		private Vector3 WorldToHexPosition(Vector3 _position)
		{
			return new Vector3(Mathf.RoundToInt(_position.x/baseDistance)*baseDistance,0f, Mathf.RoundToInt(_position.z/baseDistance)*baseDistance);
		}

		private void SpawnRandomPoi(Vector3 _position, System.Random _rng)
		{
			var randomIndex = _rng.Next(0, poiGenDataSo.pointOfInterestGenData.Length);
			var prefab = poiGenDataSo.pointOfInterestGenData[randomIndex].poiPrefab;
			var poi = Instantiate(prefab, _position, Quaternion.identity);
			activePois.Add(poi);
		}
	}
}