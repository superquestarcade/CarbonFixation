using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World
{
	public class PointOfInterestGenerator : MonoBehaviourPlus
	{
		[SerializeField] private Transform parentTransform;
		[SerializeField] private float loadRadius = 500f;
		[SerializeField] private float baseDistance = 10f;
		[SerializeField] private bool applyRandomOffset = true;
		[SerializeField, Range(0,1)] private float minRandomOffset = 0.1f;
		[SerializeField, Range(0,1)] private float maxRandomOffset = 0.5f;
		[SerializeField] private string poiGenSeed = "potato";
		[SerializeField, Range(0,1)] float noiseScale = 0.5f;
		[SerializeField, Range(0,1)] float filterThreshold = 0.3f;

		[SerializeField] private PoiGenDataSo poiGenDataSo;
		[SerializeField] private float poiResourceClearanceRadius = 15f;
		[SerializeField] private float poiCameraClearanceRadius = 30f;
		
		[Header("Resource Corridors")]
		[SerializeField] private ResourceCorridor resourceCorridorPrefab;

		[SerializeField] private Vector2 corridorCrossSection = new(50,100);
		private List<ResourceCorridor> poiCorridors = new List<ResourceCorridor>();

		private List<PointOfInterest> activePois = new();
		private List<PointOfInterest> inactivePois = new();
		private Dictionary<Vector2Int, Vector3> poiPositions = new();
		
		private Dictionary<Vector2Int, Vector2Int[]> neighbours = new();

		public void GeneratePois(Vector3 _originPosition, System.Random _rng)
		{
			var ringCount = Mathf.FloorToInt(loadRadius / baseDistance);
			var loadArea = new Vector2(loadRadius * 2, loadRadius * 2);
			var minOffset = (baseDistance / 2) * minRandomOffset;
			var maxOffset = (baseDistance / 2) * maxRandomOffset;
			var originHex = HexPointGenerator.WorldToNearestHexVertex(baseDistance, _originPosition);
			var hexPoints = HexPointGenerator.GenerateHexOuterPoints(originHex, baseDistance,loadArea, out var worldPositions);
			Debug.Assert(hexPoints.Length==worldPositions.Length);
			var filteredHexPoints = HexPointGenerator.FilterHexPointsByNoise(hexPoints, worldPositions, noiseScale, filterThreshold, out var filteredPositions);
			
			if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GeneratePoi originHex: {originHex}, hexPoints: {hexPoints.Length}, offset: ({minOffset} - {maxOffset})");

			/*var firstRingPoints = MathNL.RandomUniqueFew(6, 3, _rng);
			Debug.LogWarning($"PointOfInterestGenerator.GeneratePoi first ring points: {string.Join(", ", firstRingPoints)}");*/
			
			for (var i = 0; i < filteredHexPoints.Length; i++)
			{
				var hexPoint = filteredHexPoints[i];
				var pointNeighbours = hexPoint.GetNeighbours(filteredHexPoints);
				if (DebugMessages)
					Debug.Log(
						$"PointOfInterestGenerator.GeneratePoi hexPoint: {hexPoint}, pointNeighbours: {pointNeighbours.Length}");
				neighbours.Add(hexPoint, pointNeighbours);
				var worldHexPosition = new Vector3(filteredPositions[i].x, 0, filteredPositions[i].y);
				if (DebugMessages)
					Debug.Log(
						$"PointOfInterestGenerator.GeneratePoi generate POI at hexPoint: {hexPoint}, worldHexPosition: {worldHexPosition}");
				if (poiPositions.TryGetValue(hexPoint, out var poiPosition))
				{
					if (DebugMessages)
						Debug.Log($"PointOfInterestGenerator.GeneratePoi found existing POI at {worldHexPosition}");
					// Point has already been generated
					var randomIndex = _rng.Next(0, poiGenDataSo.pointOfInterestGenData.Length);
					var poiData = poiGenDataSo.pointOfInterestGenData[randomIndex];
					var poi = Instantiate(poiData.poiPrefab, poiPosition, Quaternion.identity, parentTransform);
					activePois.Add(poi);
					poi.gameObject.name = $"PointOfInterest {hexPoint}";
				}
				else
				{
					// Point has not been generated
					if (DebugMessages)
						Debug.Log($"PointOfInterestGenerator.GeneratePoi spawning new POI at {worldHexPosition}");
					var offsetPosition = applyRandomOffset?GetRandomPlanarOffset(_rng, maxOffset, minOffset):Vector3.zero;
					var worldOffsetPosition = worldHexPosition + offsetPosition;
					if (DebugMessages)
						Debug.Log(
							$"PointOfInterestGenerator.GeneratePoi hexPoint: {hexPoint}, worldHexPosition: {worldHexPosition}, offsetPosition: {offsetPosition}");
					var heightAtPosition = WorldManager.singleton.GetHeightAtWorldPosition(worldOffsetPosition);
					if (DebugMessages)
						Debug.Log(
							$"PointOfInterestGenerator.GeneratePoi worldOffsetPosition: {worldOffsetPosition}, heightAtPosition {heightAtPosition}");
					worldOffsetPosition.y = heightAtPosition;
					var randomIndex = _rng.Next(0, poiGenDataSo.pointOfInterestGenData.Length);
					var poiData = poiGenDataSo.pointOfInterestGenData[randomIndex];
					var poi = Instantiate(poiData.poiPrefab, worldOffsetPosition, Quaternion.identity, parentTransform);
					activePois.Add(poi);
					poi.gameObject.name = $"PointOfInterest {hexPoint}";
					poiPositions.Add(hexPoint, worldOffsetPosition);
					Debug.Log(
						$"PointOfInterestGenerator.GeneratePoi worldHexPosition: {worldHexPosition} distance to origin: {Vector3.Distance(worldOffsetPosition, _originPosition)}");
				}
			}
			
			SpawnResourceCorridors(_originPosition, neighbours,_rng);
			Physics.SyncTransforms();
			foreach(var corridor in poiCorridors)
				corridor.Generate(_rng);
		}
		
		private void SpawnResourceCorridors(Vector3 _spawnOrigin, Dictionary<Vector2Int, Vector2Int[]> _poiSpokes, System.Random _rng)
		{
			var existingOrigins = new List<Vector2Int>();
			// Spawn a resource corridor to the first poi
			var originHex = HexPointGenerator.WorldToNearestHexVertex(baseDistance, _spawnOrigin);
			var hexPoints = originHex.GetNeighbours(_poiSpokes.Keys.ToArray());
			_spawnOrigin.y = WorldManager.singleton.GetHeightAtWorldPosition(_spawnOrigin);
			
			foreach (var hexPoint in hexPoints)
			{
				var spokePosition  = poiPositions[hexPoint];
				var spawnPosition = Vector3.Lerp(_spawnOrigin, spokePosition, 0.5f);
				var scale = Vector3.Distance(_spawnOrigin, spokePosition) - (poiResourceClearanceRadius*2);
				var direction = spokePosition - _spawnOrigin;
				var targetRotation = Quaternion.LookRotation(direction);
				var corridor = Instantiate(resourceCorridorPrefab, spawnPosition, targetRotation, parentTransform);
				corridor.SetCorridorSize(new Vector3(corridorCrossSection.x, corridorCrossSection.y, scale));
				var cameraScale = Vector3.Distance(_spawnOrigin, spokePosition) - (poiCameraClearanceRadius*2);
				corridor.SetCameraSize(new Vector3(corridorCrossSection.x, corridorCrossSection.y, cameraScale));
				poiCorridors.Add(corridor);
			}
			
			foreach (var poiWheel in _poiSpokes)
			{
				existingOrigins.Add(poiWheel.Key);
				var originPosition = poiPositions[poiWheel.Key];
				foreach (var spoke in poiWheel.Value)
				{
					if (existingOrigins.Contains(spoke)) continue;
					var spokePosition  = poiPositions[spoke];
					var spawnPosition = Vector3.Lerp(originPosition, spokePosition, 0.5f);
					var scale = Vector3.Distance(originPosition, spokePosition) - (poiResourceClearanceRadius*2);
					var direction = spokePosition - originPosition;
					var targetRotation = Quaternion.LookRotation(direction);
					var corridor = Instantiate(resourceCorridorPrefab, spawnPosition, targetRotation, parentTransform);
					corridor.SetCorridorSize(new Vector3(corridorCrossSection.x, corridorCrossSection.y, scale));
					var cameraScale = Vector3.Distance(originPosition, spokePosition) - (poiCameraClearanceRadius*2);
					corridor.SetCameraSize(new Vector3(corridorCrossSection.x, corridorCrossSection.y, cameraScale));
					poiCorridors.Add(corridor);
				}
			}
		}

		private Vector3 GetRandomPlanarOffset(System.Random _rng, float _maxDistance, float _minDistance = 0)
		{
			var magnitudeX = _minDistance + ((float)_rng.NextDouble() * (_maxDistance - _minDistance));
			var signX = _rng.Next(2) == 0 ? -1f : 1f;
			var offsetX = magnitudeX * signX;
			
			var magnitudeZ = _minDistance + ((float)_rng.NextDouble() * (_maxDistance - _minDistance));
			var signZ = _rng.Next(2) == 0 ? -1f : 1f;
			var offsetZ = magnitudeZ * signZ;
			
			var offset = new Vector3(offsetX, 0,  offsetZ);
			offset.Normalize();
			var distance = _minDistance + ((float) _rng.NextDouble() * (_maxDistance - _minDistance));
			offset *= distance;
			// if(DebugMessages) Debug.Log($"PointOfInterestGenerator.GetRandomPlanar offset: {offset}, distance: {distance}, minDistance: {_minDistance}, maxDistance: {_maxDistance}");
			return offset;
		}

		/*private void OnDrawGizmos()
		{
			if (neighbours.Count <= 0) return;
			foreach (var poi in neighbours)
			{
				if (poi.Value.Length < 1) continue;
				var originPosition = poiPositions[poi.Key];
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(originPosition, 1f);
				Gizmos.color = Color.yellow;
				foreach (var hexPoint in poi.Value)
				{
					var endPosition = poiPositions[hexPoint];
					Gizmos.DrawLine(originPosition, endPosition);
				}
			}
		}*/
	}
}