using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
	public class CameraManager : MonoBehaviourSingleton<CameraManager>
	{
		private Transform playerLookAtTarget;
		private List<CameraRig> cameraRigs = new();

		public void RegisterCameraRig(CameraRig _cameraRig)
		{
			cameraRigs.Add(_cameraRig);
			_cameraRig.SetTarget(playerLookAtTarget);
		}

		public void UnregisterCameraRig(CameraRig _cameraRig)
		{
			cameraRigs.Remove(_cameraRig);
		}

		public void SetNearestCameraRigFocus(Vector3 _position)
		{
			Debug.Log($"CameraManager.SetNearestCameraRigFocus position: {_position}, rigs: {cameraRigs.Count}");
			var nearestRig = cameraRigs[0];
			foreach (var cameraRig in cameraRigs)
			{
				var nRigDistance = Vector3.Distance(nearestRig.transform.position, _position);
				var cRigDistance = Vector3.Distance(cameraRig.transform.position, _position);
				if(cRigDistance < nRigDistance)
				{
					// Debug.Log($"CameraManager.SetNearestCameraRigFocus rig at {cameraRig.transform.position} " +
					//           $"closer than nearest rig {nearestRig.transform.position}");
					nearestRig = cameraRig;
				}
			}
			
			foreach (var cameraRig in cameraRigs)
				cameraRig.SetAsFocus(cameraRig == nearestRig);
		}
		
		public void SetPlayerLookAtTarget(Transform _target)
		{
			playerLookAtTarget = _target;
		}
	}
}