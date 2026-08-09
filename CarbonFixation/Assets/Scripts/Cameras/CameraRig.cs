using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

namespace Cameras
{
	public class CameraRig : MonoBehaviour
	{
		[SerializeField] private CinemachineCamera cCamera;
		[SerializeField] private CinemachineSplineDolly cSplineDolly;
		[SerializeField] private SplineContainer cSpline;
		
		private void Start()
		{
			CameraManager.singleton.RegisterCameraRig(this);
		}

		private void OnDestroy()
		{
			CameraManager.singleton.UnregisterCameraRig(this);
		}

		public void SetAsFocus(bool _isFocus)
		{
			if(_isFocus)
			{
				Debug.Log($"CameraRig.SetAsFocus {gameObject.name}");
			}
			cCamera.Priority = (_isFocus ? 10 : 5);
			cSplineDolly.AutomaticDolly.Enabled = _isFocus;
			// Todo: snap dolly to the closest point to the player when focusing. Maybe disable dampening for a frame?
		}
		
		public Vector3 GetCameraPosition()
		{
			return cCamera.transform.position;
		}

		public void SetTarget(Transform _target)
		{
			// cCamera.LookAt = _target;
			cCamera.Target.LookAtTarget = _target;
			cCamera.Target.TrackingTarget = _target;
		}

		public void SetSize(Vector3 _size)
		{
			// Don't scale the height
			cSpline.transform.localScale = new Vector3(_size.x, 1, _size.z);
		}
	}
}