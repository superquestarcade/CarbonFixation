using System.Collections.Generic;
using Data;
using UnityEngine;

namespace World
{
	public class PointOfInterestGenerator : MonoBehaviourPlus
	{
		[SerializeField] private Transform parentTransform;
		[SerializeField] private float loadRadius = 500f;
		[SerializeField] private float baseDistance = 10f;
		[SerializeField, Range(0,1)] private float minRandomOffset = 0.1f;
		[SerializeField, Range(0,1)] private float maxRandomOffset = 0.5f;
		
		[SerializeField] private PoiGenDataSo poiGenDataSo;

		private List<PointOfInterest> activePois = new();
		private List<PointOfInterest> inactivePois = new();
		private Dictionary<Vector3, Vector3> poiPositions = new();

		public void GeneratePois(Vector3 _originPosition, System.Random _rng)
		{
			var ringCount = Mathf.FloorToInt(loadRadius / baseDistance);
			var minOffset = (baseDistance / 2) * minRandomOffset;
			var maxOffset = (baseDistance / 2) * maxRandomOffset;
			var hexPoints = HexPointGenerator.GenerateHexPoints(ringCount, baseDistance);
			var originHex = HexPointGenerator.WorldToNearestHexPoint(_originPosition, baseDistance);
			if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GeneratePoi originHex: {originHex}, hexPoints: {hexPoints.Count}, offset: ({minOffset} - {maxOffset})");
			foreach (var hexPoint in hexPoints)
			{
				var worldHexPosition = new Vector3(originHex.x + hexPoint.x, 0, originHex.y + hexPoint.y);
				if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GeneratePoi generate POI at hexPoint: {hexPoint}, worldHexPosition: {worldHexPosition}");
				
				if (poiPositions.TryGetValue(worldHexPosition, out var poiPosition))
				{
					if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GeneratePoi found existing POI at {worldHexPosition}");
					// Point has already been generated
					var poi = SpawnRandomPoi(poiPosition, _rng);
					poi.gameObject.name = $"PointOfInterest {worldHexPosition}";
				}
				else
				{
					// Point has not been generated
					if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GeneratePoi spawning new POI at {worldHexPosition}");
					var offsetPosition = GetRandomPlanarOffset(_rng, maxOffset, minOffset);
					var worldOffsetPosition = worldHexPosition + offsetPosition;
					if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GeneratePoi hexPoint: {hexPoint}, worldHexPosition: {worldHexPosition}, offsetPosition: {offsetPosition}");
					var heightAtPosition = WorldManager.singleton.GetHeightAtWorldPosition(worldOffsetPosition);
					if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GeneratePoi worldOffsetPosition: {worldOffsetPosition}, heightAtPosition {heightAtPosition}");
					worldOffsetPosition.y = heightAtPosition;
					var poi = SpawnRandomPoi(worldOffsetPosition, _rng);
					poi.gameObject.name = $"PointOfInterest ({worldHexPosition})";
					poiPositions.Add(worldHexPosition, worldOffsetPosition);
					Debug.Log($"PointOfInterestGenerator.GeneratePoi worldHexPosition: {worldHexPosition} distance to origin: {Vector3.Distance(worldOffsetPosition,_originPosition)}");
				}
				
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

		private Vector3 GetRandomPlanarOffset(System.Random _rng, float _maxDistance, float _minDistance = 0)
		{
			float magnitudeX = _minDistance + ((float)_rng.NextDouble() * (_maxDistance - _minDistance));
			float signX = _rng.Next(2) == 0 ? -1f : 1f;
			var offsetX = magnitudeX * signX;
			
			float magnitudeZ = _minDistance + ((float)_rng.NextDouble() * (_maxDistance - _minDistance));
			float signZ = _rng.Next(2) == 0 ? -1f : 1f;
			var offsetZ = magnitudeZ * signZ;
			
			var offset = new Vector3(offsetX, 0,  offsetZ);
			offset.Normalize();
			var distance = _minDistance + ((float) _rng.NextDouble() * (_maxDistance - _minDistance));
			offset *= distance;
			// if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GetRandomPlanar offset: {offset}, distance: {distance}, minDistance: {_minDistance}, maxDistance: {_maxDistance}");
			return offset;
		}
	}
}