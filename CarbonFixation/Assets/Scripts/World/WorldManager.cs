using System;
using Locomotion;
using UnityEngine;

namespace World
{
	public class WorldManager : MonoBehaviourPlus
	{
		[SerializeField] private TerrainManager terrainManager;
		[SerializeField] private PlayerCharacterController playerCharacterController;
		[SerializeField] private Transform startingCameraRig;

		private void Start()
		{
			terrainManager.ClearTerrains();
			terrainManager.LoadTerrains();
			SetPlayerAtStart();
			startingCameraRig.position = playerCharacterController.transform.position;
		}

		public void SetPlayerAtStart()
		{
			var startPosition = new Vector2(playerCharacterController.transform.position.x, playerCharacterController.transform.position.z);
			SetPlayerOnTerrain(startPosition);
		}

		private void SetPlayerOnTerrain(Vector2 _positionOnTerrain)
		{
			var worldHeight = 0f;
#if UNITY_EDITOR
			worldHeight = terrainManager.EditorGetHeightAtPosition(_positionOnTerrain);
#else
			worldHeight = terrainManager.GetHeightAtPosition(_positionOnTerrain);
#endif
			var positionOnTerrain = new Vector3(0, worldHeight, 0);
#if UNITY_EDITOR
			playerCharacterController.transform.position = positionOnTerrain;
			if(startingCameraRig!=null) startingCameraRig.position = playerCharacterController.transform.position;
#else
			playerCharacterController.Motor.SetPosition(positionOnTerrain);
#endif
			Debug.Log($"TerrainManager.SetPlayerOnTerrain: {positionOnTerrain}");
		}
	}
}