using System;
// using Interaction;
using KinematicCharacterController.Examples;
using Locomotion;
using UnityEngine;

namespace Input
{
	public class PlayerCharacterInput : MonoBehaviourPlus
	{
		[SerializeField] private Camera playerCamera;
		[SerializeField] private PlayerCharacterController kcc;
		// [SerializeField] private CharacterCameraMovement cameraMovement;
		// [SerializeField] private PlayerCharacterInteract playerCharacterInteract;
		
		[Header("Settings")]
		[SerializeField] private float verticalLookSensitivity = 1f;
		[SerializeField] private float horizontalLookSensitivity = 1f;
		[SerializeField] private bool invertMouseInput = false;
		
		private PlayerCharacterInputs characterInput = new();
		private float verticalLookRotationDelta;

		public Action OnInteract;

		#region Unity Functions
		private void Start()
		{
			// RegisterCharacterInputs();
		}

		private void FixedUpdate()
		{
			HandleCharacterControllerInput();
			// HandleCameraInput();
		}

		private void OnDestroy()
		{
			// UnregisterCharacterInputs();
		}
		
		#endregion

		#region Register Character Input

		/*private void RegisterCharacterInputs()
		{
			if(DebugMessages) Debug.Log($"Registering Character Input ({gameObject.name})");
			InputManager.singleton.RegisterInput(this, true);
		}*/

		/*private void UnregisterCharacterInputs()
		{
			if(DebugMessages) Debug.Log($"Unregistering Character Input ({gameObject.name})");
			InputManager.singleton.UnregisterInput(this);
		}*/
		
		#endregion

		#region Input Conversions
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_lookDelta"></param>
		public void Look(Vector2 _lookDelta)
		{
			// Horizontal
			var cameraInputRotation = new Vector3(0, _lookDelta.x * horizontalLookSensitivity, 0);
			characterInput.CameraRotation = Quaternion.Euler(transform.rotation.eulerAngles + cameraInputRotation);
			
			// Vertical
			verticalLookRotationDelta = _lookDelta.y * verticalLookSensitivity * (invertMouseInput?-1:1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="_moveDirection"></param>
		public void Move(Vector2 _moveDirection)
		{
			characterInput.MoveAxisForward = _moveDirection.y;
			characterInput.MoveAxisRight = _moveDirection.x;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="_buttonDown"></param>
		public void Jump(bool _buttonDown)
		{
			if(DebugMessages) Debug.Log($"PlayerCharacterInput.Jump {_buttonDown}");
			characterInput.JumpDown = _buttonDown;
		}

		public void Crouch(bool _buttonDown)
		{
			if(DebugMessages) Debug.Log($"PlayerCharacterInput.Crouch {_buttonDown}");
			characterInput.CrouchDown = _buttonDown;
			characterInput.CrouchUp = !_buttonDown;
		}

		/*public void Sprint(bool _buttonDown)
		{
			if(DebugMessages) Debug.Log($"PlayerCharacterInput.Sprint {_buttonDown}");
			characterInput.SprintDown = _buttonDown;
		}*/

		public void Interact()
		{
			if(DebugMessages) Debug.Log($"PlayerCharacterInput.Interact");
			OnInteract?.Invoke();
			// playerCharacterInteract.Interact();
		}
		#endregion
		
		#region Handle Inputs

		/*private void HandleCameraInput()
		{
			// vertical
			cameraMovement.RotateCameraVertically(verticalLookRotationDelta);
			verticalLookRotationDelta = 0;
		}*/

		private void HandleCharacterControllerInput()
		{
			// Move & horizontal rotation
			characterInput.CameraRotation = playerCamera.transform.rotation;
			kcc.SetInputs(ref characterInput);
			// characterInput.MoveAxisForward = 0;
			// characterInput.MoveAxisRight = 0;
			// characterInput.CameraRotation = new Quaternion();
		}
		#endregion

		#region Player Visibility

		public void SetPlayerActive(bool _active)
		{
			kcc.gameObject.SetActive(_active);
		}

		#endregion
	}
}