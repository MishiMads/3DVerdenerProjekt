using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/*
 This script was imported from the Unity Asset Store.
- In conjunction with the FirstPersonController.cs script, this script allows the player to move around the scene using the keyboard and mouse. 
- This script also allows the player to interact with objects in the scene, such as the inventory system and the pause menu, which were modifications I added to the script.
- In short, this script employs the use of the Unity Input System, which allows the player to move around the scene using the keyboard and mouse, and interact with objects in the scene.
- The methods are called on specified button presses, such as the spacebar for jumping, the left shift key for sprinting, the E key for interacting with objects, and the ESC or P key for pausing the game.
*/
namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool use;
		public bool pause;
		public Inventory inventory;
		public Pause pauseMenu;


		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
		
		public void OnUse(InputValue value)
		{
			if (inventory != null)
			{
				UseInput(value.isPressed);
				//Debug.Log("Yesss");
				inventory.Interact();
			}
		}
		
		public void OnPause(InputValue value)
		{
			pauseInput(value.isPressed);
			pauseMenu.PauseGame();
			pauseMenu.gameIsPaused = !pauseMenu.gameIsPaused;
			Debug.Log("Yesss");
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
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
		
		public void UseInput(bool newUseState)
		{
			use = newUseState;
		}
		
		public void pauseInput(bool newPauseState)
		{
			pause = newPauseState;
		}
	}
	
}