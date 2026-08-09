using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World
{
	public class TerrainManager : MonoBehaviourPlus
	{
		[SerializeField] private TRN_Generator terrainGeneratorPrefab;
		[SerializeField] private float loadRadius = 1000;

		private const string terrainParentName = "Terrain";
		private Transform terrainParent;
		private List<TRN_Generator> activeTerrains = new();
		
		// private List<DebugTerrainInfo> debugTerrainInfos = new();

		public void LoadTerrains()
		{
			EnsureTerrainParent();
			var loadCount = (Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width)*2) + 1;
			var loadPositionOffset = new Vector3(-(loadCount*terrainGeneratorPrefab.width)/2f, 0, -(loadCount*terrainGeneratorPrefab.width)/2f);
			for(var x = 0;x<loadCount;x++)
				for (var z = 0; z < loadCount; z++)
				{
					var worldPosition = new Vector3(x*terrainGeneratorPrefab.width, 0, z*terrainGeneratorPrefab.width) + loadPositionOffset;
					var newTerrainGen = Instantiate(terrainGeneratorPrefab, worldPosition,  Quaternion.identity, terrainParent);
					newTerrainGen.Generate();
					// Todo: An error in the terrain indexing is causing lookup bugs later on
					var terrainIndex = WorldToTerrainIndex(worldPosition);
					newTerrainGen.SetWorldIndex(terrainIndex);
					newTerrainGen.gameObject.name = $"TerrainGen ({terrainIndex.x},{terrainIndex.y})";
					activeTerrains.Add(newTerrainGen);
				}
		}

		public void ClearTerrains()
		{
			EnsureTerrainParent();
			if (Application.isEditor && !Application.isPlaying)
			{
				foreach(var childTerrain in terrainParent.GetComponentsInChildren<TRN_Generator>())
					DestroyImmediate(childTerrain.gameObject);
			}
			else
			{
				foreach (var terrain in activeTerrains)
					Destroy(terrain.gameObject);
				activeTerrains.Clear();
			}
		}

		public float GetHeightAtPosition(Vector3 _position)
		{
			EnsureTerrainParent();
			var terrainIndex = WorldToTerrainIndex(_position);
			Debug.Log($"TerrainManager.GetHeightAtPosition {_position}, terrainIndex: {terrainIndex}");
			var terrainAtPosition = activeTerrains.First(_t => _t.WorldIndex == terrainIndex);
			
			if (terrainAtPosition == null)
			{
				Debug.LogError($"TerrainManager.GetHeightAtPosition: Can't find terrain at {_position}");
				return float.MaxValue;
			}

			// Todo: height sampling is incorrect
			// Checked: terrain index, terrainAtPosition
			// Check: uvPos, SampleHeight
			var uvPos = terrainAtPosition.GetNormalizedPosition(_position);
			Debug.Log($"TerrainManager.GetHeightAtPosition {_position}, uvPos: {uvPos}");
			terrainAtPosition.SampleHeight(uvPos, out var height, out var worldHeight, out var normalizedHeight);
			/*debugTerrainInfos.Add(new DebugTerrainInfo()
			{
				terrainGenAtPosition = terrainAtPosition,
				inputPosition = new Vector3(_position.x, 0, _position.z),
				outputPosition = new Vector3(_position.x, worldHeight, _position.z),
				uvWorldPosition = new Vector3(
					terrainAtPosition.transform.position.x + (uvPos.x * terrainAtPosition.width), 
					0, 
					terrainAtPosition.transform.position.z + (uvPos.y * terrainAtPosition.width)
					),
			});*/
			return worldHeight;
		}
		
		public float EditorGetHeightAtPosition(Vector2 _position)
		{
			EnsureTerrainParent();
			// Todo: I don't think the terrain index will be correct when the player moves off the first terrain. Double check this
			var terrainIndex = WorldToTerrainIndex(_position);
			Debug.Log($"TerrainManager.EditorGetHeightAtPosition {_position}, terrainIndex: {terrainIndex}");
			var terrains = terrainParent.GetComponentsInChildren<TRN_Generator>();
			var terrainAtPosition = terrains.First(_t => _t.WorldIndex == terrainIndex);
			
			if (terrainAtPosition == null)
			{
				Debug.LogError($"TerrainManager.EditorGetHeightAtPosition: Can't find terrain at {_position}");
				return float.MaxValue;
			}
			terrainAtPosition.SampleHeight(terrainAtPosition.GetNormalizedPosition(_position), out var height, out var worldHeight, out var normalizedHeight);
			return worldHeight;
		}

		private void EnsureTerrainParent()
		{
			if (terrainParent != null) return;
			var searchTerrainParent = transform.Find(terrainParentName);
			if (searchTerrainParent == null)
			{
				terrainParent = new GameObject(terrainParentName).transform;
				terrainParent.SetParent(transform);
			}
			else
			{
				terrainParent = searchTerrainParent;
			}
		}

		private Vector2Int WorldToTerrainIndex(Vector3 _position)
		{
			Debug.Log($"TerrainManager.WorldToTerrainIndex position: {_position}");
			var posX = _position.x + ((float) terrainGeneratorPrefab.width / 2);
			var posZ = _position.z + ((float) terrainGeneratorPrefab.width / 2);
			Debug.Log($"TerrainManager.WorldToTerrainIndex posX: {posX}, posZ: {posZ}");
			var indexX = Mathf.FloorToInt(posX / terrainGeneratorPrefab.width);
			var indexZ = Mathf.FloorToInt(posZ / terrainGeneratorPrefab.width);
			Debug.Log($"TerrainManager.WorldToTerrainIndex indexX: {indexX}, indexZ: {indexZ}");
			var terrainIndex = new Vector2Int(indexX, indexZ);
			Debug.Log($"TerrainManager.WorldToTerrainIndex terrainIndex: {terrainIndex}");
			return terrainIndex;
		}

		private struct DebugTerrainInfo
		{
			public TRN_Generator terrainGenAtPosition;
			public Vector3 inputPosition;
			public Vector3 outputPosition;
			public Vector3 uvWorldPosition;
		}

		private void OnDrawGizmos()
		{
			/*if (debugTerrainInfos.Count == 0) return;
			foreach (var debugInfo in debugTerrainInfos)
			{
				/*Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(debugInfo.inputPosition, 1f);#1#
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(debugInfo.outputPosition, 1f);
				Gizmos.color = Color.orange;
				Gizmos.DrawLine(debugInfo.inputPosition, debugInfo.outputPosition);
				/*Gizmos.color = Color.blue;
				Gizmos.DrawLine(debugInfo.outputPosition, debugInfo.terrainGenAtPosition.transform.position);#1#
				Gizmos.color = Color.hotPink;
				Gizmos.DrawSphere(debugInfo.uvWorldPosition, 10f);
				Gizmos.DrawLine(debugInfo.outputPosition, debugInfo.uvWorldPosition);
			}*/
			
		}
	}
}