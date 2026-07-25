using System.Collections.Generic;
using Data;
using UnityEngine;

namespace World
{
	public class PointOfInterestGenerator : MonoBehaviourPlus
	{
		[SerializeField] private Transform parentTransform;
		[SerializeField] private float baseDistance = 10f;
		[SerializeField] private float distanceVariance = 2f;
		
		[SerializeField] private PoiGenDataSo poiGenDataSo;

		private List<PointOfInterest> activePois = new();
		private List<PointOfInterest> inactivePois = new();

		public void GeneratePois(Vector3 _originPosition, float _radius, System.Random _rng)
		{
			var ringCount = Mathf.FloorToInt(_radius / baseDistance);
			var hexPoints = HexPointGenerator.GenerateHexPoints(ringCount, baseDistance, distanceVariance, _rng);
			var originHex = HexPointGenerator.WorldToNearestHexPoint(_originPosition, baseDistance);
			Debug.Log($"PointOfInterestGenerator.GeneratePoi originHex {originHex}, ringCount {ringCount}, hexPoints {hexPoints.Count}");
			foreach (var hexPoint in hexPoints)
			{
				var worldHexPosition = new Vector3(originHex.x + hexPoint.x, 0, originHex.y + hexPoint.y);
				Debug.Log($"PointOfInterestGenerator.GeneratePoi hexPoint {hexPoint}, worldHexPosition {worldHexPosition}");
				var heightAtPosition = WorldManager.singleton.GetHeightAtWorldPosition(worldHexPosition);
				Debug.Log($"PointOfInterestGenerator.GeneratePoi hexPoint {hexPoint}, worldHexPosition {worldHexPosition}, heightAtPosition {heightAtPosition}");
				worldHexPosition.y = heightAtPosition;
				var poi = SpawnRandomPoi(worldHexPosition, _rng);
				poi.gameObject.name = $"PointOfInterest ({hexPoint})";
			}
		}

		private PointOfInterest SpawnRandomPoi(Vector3 _position, System.Random _rng)
		{
			var randomIndex = _rng.Next(0, poiGenDataSo.pointOfInterestGenData.Length);
			var prefab = poiGenDataSo.pointOfInterestGenData[randomIndex].poiPrefab;
			var poi = Instantiate(prefab, _position, Quaternion.identity, parentTransform);
			activePois.Add(poi);
			return poi;
		}
	}
}