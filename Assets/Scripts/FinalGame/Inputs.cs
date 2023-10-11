using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class Inputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool shoot;
		public bool sprint;
		public bool aim;
		//public bool applicationFocus;
		public bool esc;
		public bool IsFreez = false;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorInputForLook = true;

		public static Inputs Instance;
#if ENABLE_INPUT_SYSTEM
		public PlayerInput playerInput;
#endif

		private void Awake()
        {
			Instance = this;
#if ENABLE_INPUT_SYSTEM
			playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
		}

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(IsFreez?Vector2.zero: value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(IsFreez ? Vector2.zero : value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(IsFreez ? false : value.isPressed);
		}

		public void OnShoot(InputValue value)
		{
			ShootInput(IsFreez ? false : value.isPressed);
		}

		public void OnMenu(InputValue value)
		{
			MenuInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(IsFreez ? false : value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(IsFreez ? false : value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void AimInput(bool newAimState)
		{
			aim = newAimState;
		}
		/*
		private void OnApplicationFocus(bool hasFocus)
		{
			applicationFocus = hasFocus;
			//SetCursorState(cursorLocked);
		}
		*/
		public void LookCursor(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		private void ShootInput(bool newShootState)
        {
			if (aim == false)
				return;
			shoot = newShootState;
        }

		private void MenuInput(bool newEscState)
        {
			esc = newEscState;
        }
	}

}