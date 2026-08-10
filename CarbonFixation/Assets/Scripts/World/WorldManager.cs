using System;
using Cameras;
using Locomotion;
using ScriptableObjects;
using UnityEngine;
using World.Resources;

namespace World
{
	public class WorldManager : MonoBehaviourSingleton<WorldManager>
	{
		[SerializeField] private TerrainManager terrainManager;
		[SerializeField] private PointOfInterestGenerator poiGenerator;
		[SerializeField] private PlayerCharacterController playerCharacterController;
		[SerializeField] private Transform startingCameraRig;
		[SerializeField] private ResourceItemsSO resourceItems;
		[SerializeField] private float updateClosestCameraRig = 0.5f;
		private float cameraRigUpdateTimer = 0f;

		private System.Random worldGenRng;
		private bool setNearestCameraRigDirty = true;

		private void Start()
		{
			CameraManager.singleton.SetPlayerLookAtTarget(playerCharacterController.transform);
			worldGenRng = new System.Random(DateTime.Now.GetHashCode());
			terrainManager.ClearTerrains();
			terrainManager.LoadTerrains();
			var playerStartPosition = new Vector2(playerCharacterController.transform.position.x, playerCharacterController.transform.position.z);
			poiGenerator.GeneratePois(playerStartPosition, worldGenRng);
			SetPlayerOnTerrain();
			startingCameraRig.position = playerCharacterController.transform.position;
		}

		private void Update()
		{
			CameraRigUpdate();
		}

		private void CameraRigUpdate()
		{
			cameraRigUpdateTimer -= Time.deltaTime;
			if (cameraRigUpdateTimer > 0) return;
			CameraManager.singleton.SetNearestCameraRigFocus(playerCharacterController.transform.position);
			cameraRigUpdateTimer += updateClosestCameraRig;
		}

		public void SetPlayerOnTerrain()
		{
			var playerWorldPosition = playerCharacterController.transform.position;
			var worldHeight = GetHeightAtWorldPosition(playerWorldPosition);
			var positionOnTerrain = new Vector3(0, worldHeight, 0);
			if (Application.isEditor && !Application.isPlaying)
			{
				// Code runs strictly when working in the scene editor view
				playerCharacterController.transform.position = positionOnTerrain;
				if(startingCameraRig!=null) startingCameraRig.position = playerCharacterController.transform.position;
			}
			else
			{
				playerCharacterController.Motor.SetPosition(positionOnTerrain);
			}
			
			Debug.Log($"TerrainManager.SetPlayerOnTerrain: {positionOnTerrain}");
		}

		public float GetHeightAtWorldPosition(Vector3 _positionOnTerrain)
		{
			var worldHeight = 0f;
			if (Application.isEditor && !Application.isPlaying)
			{
				// Code runs strictly when working in the scene editor view
				worldHeight = terrainManager.EditorGetHeightAtPosition(_positionOnTerrain);
			}
			else
			{
				worldHeight = terrainManager.GetHeightAtPosition(_positionOnTerrain);
			}
			
			return worldHeight;
		}

		public ResourceItem GetRandomResourceItem(System.Random _rng)
		{
			return resourceItems.GetRandomResourceItem(_rng);
		}
	}
}