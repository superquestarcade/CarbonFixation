using UnityEditor;
using UnityEngine;

namespace World
{
	[CustomEditor(typeof(WorldManager))]
	public class WorldManagerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// Draw the default inspector fields (loadRadius, terrainGeneratorPrefab, etc.)
			DrawDefaultInspector();

			var worldManager = (WorldManager) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Editor Controls", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Set Player At Start"))
			{
				worldManager.SetPlayerOnTerrain();
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}