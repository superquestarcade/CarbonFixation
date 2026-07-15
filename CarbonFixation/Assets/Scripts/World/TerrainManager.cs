using System;
using Locomotion;
using UnityEngine;

namespace World
{
	public class TerrainManager : MonoBehaviourPlus
	{
		[SerializeField] private PlayerCharacterController playerCharacterController;
		[SerializeField] private Transform cameraTransform;
		[SerializeField] private TRN_Generator terrainGeneratorPrefab;
		[SerializeField] private float loadRadius = 1000;

		private TRN_Generator[,] terrainGenArray;

		private Vector3 debugPlayerSpawnPoint = Vector3.zero;

		private void Start()
		{
			LoadTerrains();
			SetPlayerOnTerrain(Vector2.zero);
		}

		private void LoadTerrains()
		{
			var loadCount = (Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width)*2) + 1;
			var loadPositionOffset = new Vector3(-(loadCount*terrainGeneratorPrefab.width)/2f, 0, -(loadCount*terrainGeneratorPrefab.width)/2f);
			terrainGenArray = new TRN_Generator[loadCount, loadCount];
			for(var x = 0;x<loadCount;x++)
				for (var z = 0; z < loadCount; z++)
				{
					var worldPosition = new Vector3(x*terrainGeneratorPrefab.width, 0, z*terrainGeneratorPrefab.width) + loadPositionOffset;
					var newTerrainGen = Instantiate(terrainGeneratorPrefab, worldPosition,  Quaternion.identity, transform);
					newTerrainGen.Generate();
					terrainGenArray[x,z] = newTerrainGen;
				}
		}

		private void SetPlayerOnTerrain(Vector2 _position)
		{
			// Todo: I don't think the terrain index will be correct when the player moves off the first terrain. Double check this
			var terrainIndex = new Vector2Int(Mathf.RoundToInt(_position.x / terrainGeneratorPrefab.width)+Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width),
				Mathf.RoundToInt(_position.y / terrainGeneratorPrefab.width)+Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width));
			Debug.Log($"TerrainManager.SetPlayerOnTerrain terrainIndex: {terrainIndex}");
			var terrainAtPosition = terrainGenArray[terrainIndex.x, terrainIndex.y];
			
			if (terrainAtPosition == null)
			{
				Debug.LogError($"TerrainManager.SetPlayerOnTerrain: Can't find terrain at {_position}");
				return;
			}
			terrainAtPosition.SampleHeight(terrainAtPosition.GetNormalizedPosition(_position), out var height, out var worldHeight, out var normalizedHeight);
			var positionOnTerrain = new Vector3(_position.x, worldHeight, _position.y);
			playerCharacterController.Motor.SetPosition(positionOnTerrain);
			cameraTransform.position = positionOnTerrain;
			debugPlayerSpawnPoint = positionOnTerrain;
			Debug.Log($"TerrainManager.SetPlayerOnTerrain: {positionOnTerrain}");
			// playerCharacterController.gameObject.SetActive(true);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.greenYellow;
			Gizmos.DrawWireSphere(debugPlayerSpawnPoint, 0.5f);
		}
	}
}