using System;
using Data;
using Data.Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
	public class InputManager : MonoBehaviourSingleton<InputManager>
	{
		[SerializeField] private PlayerInput playerInput;

		[SerializeField] private PlayerCharacterInput playerCharacterInput;
		private InputState inputState = InputState.Player;

		#region Input Registration
		/*public void RegisterInput(InputReceiver _inputReceiver, bool _setAsActiveState = false)
		{
			switch (_inputReceiver)
			{
				case PlayerCharacterInput pci:
					playerCharacterInput = pci;
					if(_setAsActiveState) SetInputState(InputState.Player);
					break;
				case VehicleInput vi:
					vehicleInput = vi;
					if(_setAsActiveState) SetInputState(InputState.Vehicle);
					break;
				case InputFocus fi:
					inputFocus = fi;
					if(_setAsActiveState) SetInputState(InputState.Focus);
					break;
			}
		}*/

		/*public void UnregisterInput(InputReceiver _inputReceiver)
		{
			switch (_inputReceiver)
			{
				case PlayerCharacterInput pci:
					playerCharacterInput = null;
					if(inputState == InputState.Player) SetInputState(InputState.Ui);
					break;
				case VehicleInput vi:
					vehicleInput = null;
					if(inputState == InputState.Vehicle) SetInputState(InputState.Player);
					break;
				case InputFocus fi:
					inputFocus = null;
					if(inputState == InputState.Focus) SetInputState(InputState.Player);
					break;
			}
		}*/
		
		#endregion
		
		#region Player Inputs

		public void OnMoveP(InputAction.CallbackContext _context)
		{
			if (playerCharacterInput == null) return;
			if (inputState != InputState.Player) return;
			if (_context.performed)
				playerCharacterInput.Move(_context.ReadValue<Vector2>());
			if(_context.canceled)
				playerCharacterInput.Move(Vector2.zero);
		}

		public void OnLookP(InputAction.CallbackContext _context)
		{
			if (playerCharacterInput == null) return;
			if (inputState != InputState.Player) return;
			if (!_context.performed) return;
			playerCharacterInput.Look(_context.ReadValue<Vector2>());
		}

		public void OnJumpP(InputAction.CallbackContext _context)
		{
			if (playerCharacterInput == null) return;
			if (inputState != InputState.Player) return;
			if (_context.performed)
			{
				if(DebugMessages) Debug.Log($"InputManager.OnJump performed");
				playerCharacterInput.Jump(true);
			}

			if (_context.canceled)
			{
				if(DebugMessages) Debug.Log($"InputManager.OnJump canceled");
				playerCharacterInput.Jump(false);
			}
		}
		
		/*public void OnSprintP(InputAction.CallbackContext _context)
		{
			if (playerCharacterInput == null) return;
			if (_context.performed)
			{
				if(DebugMessages) Debug.Log($"InputManager.OnSprint performed");
				playerCharacterInput.Sprint(true);
			}

			if (_context.canceled)
			{
				if(DebugMessages) Debug.Log($"InputManager.OnSprint canceled");
				playerCharacterInput.Sprint(false);
			}
		}*/
		
		public void OnCrouchP(InputAction.CallbackContext _context)
		{
			if (playerCharacterInput == null) return;
			if (inputState != InputState.Player) return;
			if (_context.performed)
			{
				if(DebugMessages) Debug.Log($"InputManager.OnCrouch performed");
				playerCharacterInput.Crouch(true);
			}

			if (_context.canceled)
			{
				if(DebugMessages) Debug.Log($"InputManager.OnCrouch canceled");
				playerCharacterInput.Crouch(false);
			}
		}

		public void OnInteractP(InputAction.CallbackContext _context)
		{
			if (playerCharacterInput == null) return;
			if (inputState != InputState.Player) return;
			if (!_context.performed) return;
			if(DebugMessages) Debug.Log($"InputManager.OnInteract performed");
			playerCharacterInput.Interact();
		}
		#endregion
		
		/*#region Vehicle Inputs
		
		public void OnMoveV(InputAction.CallbackContext _context)
		{
			if (vehicleInput == null) return;
			if (_context.performed) 
				vehicleInput.Move(_context.ReadValue<Vector2>());
			if(_context.canceled)
				vehicleInput.Move(Vector2.zero);
		}
		
		/// <summary>
		/// Currently unused - handled directly by Cinemachine cameras
		/// </summary>
		/// <param name="_context"></param>
		public void OnLookV(InputAction.CallbackContext _context)
		{
			if (vehicleInput == null) return;
			if (_context.performed) 
				vehicleInput.Look(_context.ReadValue<Vector2>());
			if(_context.canceled)
				vehicleInput.Look(Vector2.zero);
		}
		
		public void OnAccelerateV(InputAction.CallbackContext _context)
		{
			if (vehicleInput == null) return;
			if (_context.performed) 
				vehicleInput.Accelerate(_context.ReadValue<float>());
			if (_context.canceled)
				vehicleInput.Accelerate(0f);
		}
		
		public void OnBrakeV(InputAction.CallbackContext _context)
		{
			if (vehicleInput == null) return;
			if (_context.performed) 
				vehicleInput.Brake(_context.ReadValue<float>());
			if (_context.canceled)
				vehicleInput.Brake(0f);
		}
		
		public void OnInteractV(InputAction.CallbackContext _context)
		{
			if (vehicleInput == null) return;
			if (!_context.performed) return;
			if(DebugMessages) Debug.Log($"InputManager.OnInteractV performed");
			vehicleInput.Interact();
		}
		
		#endregion*/

		/*#region Focus Inputs

		public void OnInteractF(InputAction.CallbackContext _context)
		{
			if (inputFocus == null) return;
			if (!_context.performed) return;
			if(DebugMessages) Debug.Log($"InputManager.OnInteractF performed");
			inputFocus.Interact();
		}

		public void OnCancelF(InputAction.CallbackContext _context)
		{
			if (inputFocus == null) return;
			if (!_context.performed) return;
			if(DebugMessages) Debug.Log($"InputManager.OnCancelF performed");
			inputFocus.Cancel();
		}

		#endregion*/
		
		#region Input State Settings

		public void SetInputState(InputState _inputState)
		{
			if (!playerInput.inputIsActive) return; // Prevents an error when trying to switch input when exiting the game
			if(DebugMessages) Debug.Log($"InputManager.SetInputState: {_inputState}");
			switch (_inputState)
			{
				case InputState.Ui:
					playerInput.SwitchCurrentActionMap("UI");
					SetCursorVisible(true);
					break;
				case InputState.Player:
					playerInput.SwitchCurrentActionMap("Player");
					SetCursorVisible(false);
					break;
				case InputState.Vehicle:
					playerInput.SwitchCurrentActionMap("Vehicle");
					SetCursorVisible(false);
					break;
				case InputState.Focus:
					playerInput.SwitchCurrentActionMap("UI");
					SetCursorVisible(true);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(_inputState), _inputState, null);
			}
			inputState = _inputState;
		}

		private void SetCursorVisible(bool _visible)
		{
			Cursor.lockState = _visible? CursorLockMode.None : CursorLockMode.Locked;
			Cursor.visible = _visible;
		}
		
		#endregion
	}
}