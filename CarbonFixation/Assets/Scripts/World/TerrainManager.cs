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
		private TRN_Generator[,] terrainGenArray;

		public void LoadTerrains()
		{
			EnsureTerrainParent();
			var loadCount = (Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width)*2) + 1;
			var loadPositionOffset = new Vector3(-(loadCount*terrainGeneratorPrefab.width)/2f, 0, -(loadCount*terrainGeneratorPrefab.width)/2f);
			terrainGenArray = new TRN_Generator[loadCount, loadCount];
			for(var x = 0;x<loadCount;x++)
				for (var z = 0; z < loadCount; z++)
				{
					var worldPosition = new Vector3(x*terrainGeneratorPrefab.width, 0, z*terrainGeneratorPrefab.width) + loadPositionOffset;
					var newTerrainGen = Instantiate(terrainGeneratorPrefab, worldPosition,  Quaternion.identity, terrainParent);
					newTerrainGen.Generate();
					newTerrainGen.SetWorldIndex(x, z);
					newTerrainGen.gameObject.name = $"TerrainGen ({x},{z})";
					terrainGenArray[x,z] = newTerrainGen;
				}
		}

		public void ClearTerrains()
		{
			EnsureTerrainParent();
#if UNITY_EDITOR
			foreach(var childTerrain in terrainParent.GetComponentsInChildren<TRN_Generator>())
				DestroyImmediate(childTerrain.gameObject);
#else
			if (terrainGenArray != null)
			{
				foreach (var terrain in terrainGenArray)
					Destroy(terrain.gameObject);
				terrainGenArray = null;
			}
#endif
		}

		public float GetHeightAtPosition(Vector2 _position)
		{
			EnsureTerrainParent();
			// Todo: I don't think the terrain index will be correct when the player moves off the first terrain. Double check this
			var terrainIndex = new Vector2Int(Mathf.RoundToInt(_position.x / terrainGeneratorPrefab.width)+Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width),
				Mathf.RoundToInt(_position.y / terrainGeneratorPrefab.width)+Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width));
			Debug.Log($"TerrainManager.SetPlayerOnTerrain terrainIndex: {terrainIndex}");
			var terrainAtPosition = terrainGenArray[terrainIndex.x, terrainIndex.y];
			
			if (terrainAtPosition == null)
			{
				Debug.LogError($"TerrainManager.SetPlayerOnTerrain: Can't find terrain at {_position}");
				return float.MaxValue;
			}
			terrainAtPosition.SampleHeight(terrainAtPosition.GetNormalizedPosition(_position), out var height, out var worldHeight, out var normalizedHeight);
			return worldHeight;
		}
		
		public float EditorGetHeightAtPosition(Vector2 _position)
		{
			EnsureTerrainParent();
			// Todo: I don't think the terrain index will be correct when the player moves off the first terrain. Double check this
			var terrainIndex = new Vector2Int(Mathf.RoundToInt(_position.x / terrainGeneratorPrefab.width)+Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width),
				Mathf.RoundToInt(_position.y / terrainGeneratorPrefab.width)+Mathf.CeilToInt(loadRadius / terrainGeneratorPrefab.width));
			Debug.Log($"TerrainManager.EditorGetHeightAtPosition terrainIndex: {terrainIndex}");
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
	}
}