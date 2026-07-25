using UnityEditor;
using UnityEngine;

namespace World
{
	[CustomEditor(typeof(TerrainManager))]
	public class TerrainManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Draw the default inspector fields (loadRadius, terrainGeneratorPrefab, etc.)
			DrawDefaultInspector();

			TerrainManager terrainManager = (TerrainManager)target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Terrain Controls", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Load Terrains"))
			{
				terrainManager.LoadTerrains();
			}

			if (GUILayout.Button("Clear Terrains"))
			{
				terrainManager.ClearTerrains();
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}