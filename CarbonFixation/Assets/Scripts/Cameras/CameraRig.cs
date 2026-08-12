using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using World;

namespace Cameras
{
	public class CameraRig : MonoBehaviour
	{
		[SerializeField] private CinemachineCamera cCamera;
		[SerializeField] private CinemachineSplineDolly cSplineDolly;
		[SerializeField] private SplineContainer cSpline;
		[SerializeField] private float splineKnotHeight = 5f;
		
		[SerializeField] private float behindDistance = 3f;
		[SerializeField] private float smoothSpeed = 5f;
		private float currentZOffset;

		private List<Vector3> debugSplineAdjustedPoints = new();
		private List<Vector3> debugSplineWorldPoints = new();
		private Vector3 closestSplinePoint;
		private void Start()
		{
			CameraManager.singleton.RegisterCameraRig(this);
			closestSplinePoint = transform.position;
		}

		private void Update()
		{
			UpdateOffsetDirection();
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
			// cSpline.transform.localScale = new Vector3(_size.x, 1, _size.z);
			// Scale the spline knots instead of the spline transform to prevent follow anomalies later
			
			for (var splineIndex = 0; splineIndex < cSpline.Splines.Count; splineIndex++)
			{
				var spline = cSpline.Splines[splineIndex];

				for (var knotIndex = 0; knotIndex < spline.Count; knotIndex++)
				{
					var knot = spline[knotIndex];

					var scaledPosition = new float3(_size.x * knot.Position.x, _size.y/2 * knot.Position.y, _size.z * knot.Position.z);
					var worldPosition = cSpline.transform.TransformPoint(scaledPosition);
					debugSplineWorldPoints.Add(worldPosition);
					var terrainHeight = WorldManager.singleton.GetHeightAtWorldPosition(worldPosition);
					worldPosition.y = terrainHeight + splineKnotHeight;
					debugSplineAdjustedPoints.Add(worldPosition);
					knot.Position = cSpline.transform.InverseTransformPoint(worldPosition);

					// Write the modified knot back into the spline
					spline.SetKnot(knotIndex, knot);
				}
			}
		}

		public Vector3 GetClosestSplinePoint(Vector3 _position)
		{
			
			var closestPoint = cSpline.transform.TransformPoint(cSpline.Splines[0][0].Position);
			for (var splineIndex = 0; splineIndex < cSpline.Splines.Count; splineIndex++)
			{
				var spline = cSpline.Splines[splineIndex];
				
				// Convert world space to spline's local space
				Vector3 localPoint = cSpline.transform.InverseTransformPoint(_position);

				float3 nearest;
				float t; // normalized time (0-1) along the spline
				float distance = SplineUtility.GetNearestPoint(
					spline,
					localPoint,
					out nearest,
					out t
				);

				// Convert result back to world space
				Vector3 nearestWorld = cSpline.transform.TransformPoint(nearest);
				
				if(Vector3.Distance(nearestWorld, _position) < Vector3.Distance(closestPoint, _position))
					closestPoint = nearestWorld;
			}
			// Debug.DrawLine(_position, closestPoint, Color.chocolate, 10f);
			closestSplinePoint = closestPoint;
			return closestPoint;
		}
		
		private void UpdateOffsetDirection()
		{
			// Only moving cameras should do this check
			if (!cSplineDolly.AutomaticDolly.Enabled) return;
			// Convert CameraPosition to a normalized 0-1 t regardless of PositionUnits setting
	        float normalizedT = GetNormalizedT(cSpline.Spline);

	        cSpline.Spline.Evaluate(normalizedT, out float3 pos, out float3 tangent, out float3 up);
	        Vector3 splineTangent = ((Vector3)tangent).normalized;

	        // Use velocity if you have a rigidbody/characterController, otherwise transform.forward
	        Vector3 travelDir = CameraManager.singleton.PlayerLookAtTarget.forward;

	        float dot = Vector3.Dot(travelDir.normalized, splineTangent);

	        // Moving with spline tangent -> pull camera back (negative Z)
	        // Moving against spline tangent -> push camera forward (positive Z)
	        float targetZOffset = dot >= 0f ? -behindDistance : behindDistance;

	        currentZOffset = Mathf.Lerp(currentZOffset, targetZOffset, Time.deltaTime * smoothSpeed);

	        Vector3 offset = cSplineDolly.SplineOffset;
	        offset.x = currentZOffset;
	        cSplineDolly.SplineOffset = offset;
		}
		
		private float GetNormalizedT(Spline spline)
		{
			if (cSplineDolly.PositionUnits == PathIndexUnit.Normalized)
				return cSplineDolly.CameraPosition;

			return SplineUtility.GetNormalizedInterpolation(spline, cSplineDolly.CameraPosition, cSplineDolly.PositionUnits);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.chocolate;
			Gizmos.DrawWireSphere(closestSplinePoint, 20f);
			Gizmos.color = Color.chocolate;
			Gizmos.DrawSphere(closestSplinePoint, 1f);
			
			/*if (debugSplineAdjustedPoints.Count > 0)
			{
				foreach (var point in debugSplineAdjustedPoints)
				{
					Gizmos.color = Color.darkOrchid;
					Gizmos.DrawWireSphere(point, 20f);
					Gizmos.color = Color.cornflowerBlue;
					Gizmos.DrawSphere(point, 1f);
				}
			}
			if (debugSplineWorldPoints.Count > 0)
			{
				foreach (var point in debugSplineWorldPoints)
				{
					Gizmos.color = Color.magenta;
					Gizmos.DrawWireSphere(point, 20f);
					Gizmos.color = Color.deepPink;
					Gizmos.DrawSphere(point, 1f);
				}
			}*/
		}
	}
}