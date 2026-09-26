using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	public class InputManager
	{

		private KeyboardState currentKeyState;
		private KeyboardState previousKeyState;
		private GamePadState currentGamePadState;
		private GamePadState previousGamePadState;
		private MouseState currentMouseState, previousMouseState;
		DisplayManager displayManager;

		public enum BindingType
		{
			None,
			Platformer,
			TopDownAdventure
		}

		public enum InputAction
		{
			Jump,
			Attack,
			Dash,
			Interact,
			MoveLeft,
			MoveRight,
			MoveUp,
			MoveDown,
			LookUp,
			Crouch,
			UseItem,
			Shield,
			Map,
			Inventory,
			Pause

		}

		private BindingType bindingType;

		private Dictionary<BindingType, Dictionary<InputAction, Func<bool>>>? inputBindings { get; set; }

		public Dictionary<InputAction, Func<bool>>? Binding { get; set; }

		public InputManager(DisplayManager displayManager, BindingType bindingType = BindingType.None)
		{
			this.displayManager = displayManager;
			this.bindingType = bindingType;

			inputBindings = new Dictionary<BindingType, Dictionary<InputAction, Func<bool>>>();

			Dictionary<InputAction, Func<bool>> platformerBinding = new Dictionary<InputAction, Func<bool>>
			{
				{ InputAction.MoveLeft,  () => IsKeyDown(Keys.A) || IsKeyDown(Keys.Left) || IsButtonDown(Buttons.DPadLeft) },
				{ InputAction.MoveRight, () => IsKeyDown(Keys.D) || IsKeyDown(Keys.Right) || IsButtonDown(Buttons.DPadRight) },
				{ InputAction.LookUp,    () => IsKeyDown(Keys.W) || IsKeyDown(Keys.Up) || IsButtonDown(Buttons.DPadUp) },
				{ InputAction.Crouch,    () => IsKeyDown(Keys.S) || IsKeyDown(Keys.Down) || IsButtonDown(Buttons.DPadDown) },

				{ InputAction.Jump,      () => IsKeyPressed(Keys.Space) || IsButtonPressed(Buttons.A) },
				{ InputAction.Attack,    () => IsKeyPressed(Keys.U) || IsButtonPressed(Buttons.X) },
				{ InputAction.Dash,      () => IsKeyPressed(Keys.O) || IsButtonPressed(Buttons.B) },
				{ InputAction.Interact,  () => IsKeyPressed(Keys.E) || IsButtonPressed(Buttons.Y) },

				{ InputAction.Pause,     () => IsKeyPressed(Keys.Escape) || IsButtonPressed(Buttons.Start) }
			};

			Dictionary<InputAction, Func<bool>> topDownAdventureBinding = new Dictionary<InputAction, Func<bool>>
			{
				{ InputAction.MoveLeft,  () => IsKeyDown(Keys.A) || IsKeyDown(Keys.Left) || IsButtonDown(Buttons.DPadLeft) },
				{ InputAction.MoveRight, () => IsKeyDown(Keys.D) || IsKeyDown(Keys.Right) || IsButtonDown(Buttons.DPadRight) },
				{ InputAction.MoveUp,    () => IsKeyDown(Keys.W) || IsKeyDown(Keys.Up) || IsButtonDown(Buttons.DPadUp) },
				{ InputAction.MoveDown,  () => IsKeyDown(Keys.S) || IsKeyDown(Keys.Down) || IsButtonDown(Buttons.DPadDown) },

				{ InputAction.Attack,    () => IsKeyPressed(Keys.U) || IsButtonDown(Buttons.X) },
				{ InputAction.Interact,  () => IsKeyPressed(Keys.Space) || IsButtonDown(Buttons.A) },
				{ InputAction.Shield,    () => IsKeyPressed(Keys.O) || IsButtonDown(Buttons.RightShoulder) },
				{ InputAction.UseItem,   () => IsKeyPressed(Keys.I) || IsButtonDown(Buttons.B) },
				{ InputAction.Inventory, () => IsKeyPressed(Keys.R) || IsButtonDown(Buttons.Y) },

				{ InputAction.Map, () => IsKeyPressed(Keys.Enter) || IsButtonPressed(Buttons.Back) },
				{ InputAction.Pause, () => IsKeyPressed(Keys.Escape) || IsButtonPressed(Buttons.Start) },


			};

			inputBindings.Add(BindingType.None, default);
			inputBindings?.Add(BindingType.Platformer, platformerBinding);
			inputBindings?.Add(BindingType.TopDownAdventure, topDownAdventureBinding);

			Binding = inputBindings?[bindingType];
		}

		public void Update()
		{
			previousKeyState = currentKeyState;
			currentKeyState = Keyboard.GetState();
			previousGamePadState = currentGamePadState;
			currentGamePadState = GamePad.GetState(0);
			previousMouseState = currentMouseState;
			currentMouseState = Mouse.GetState();


		}

		public bool IsKeyDown(Keys key)
		{
			return currentKeyState.IsKeyDown(key);
		}

		public bool IsKeyPressed(Keys key)
		{
			return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
		}

		public bool IsKeyReleased(Keys key)
		{
			return !currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyDown(key);
		}



	//	public float MouseX => MousePosition.X;
	//	public float MouseY => MousePosition.Y;

	//	public Vector2 MousePosition => displayManager.ScreenToViewport(new Vector2(currentMouseState.X, currentMouseState.Y));

		public void SetMousePosition(int x, int y)
		{
			Mouse.SetPosition(x, y);
		}

		public bool IsMouseButtonDown()
		{
			return currentMouseState.LeftButton == ButtonState.Pressed;
		}

		public bool IsMouseButtonPressed()
		{
			return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
		}

		public bool IsMouseButtonJustReleased()
		{
			return currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed;
		}

		//public bool IsMouseOver(Rectangle rectangle)
		//{
		//	return rectangle.Contains(MousePosition);
		//}

		//public bool IsMousePressedOver(Rectangle rectangle)
		//{
		//	return rectangle.Contains(MousePosition) & IsMouseButtonPressed();
		//}

		public bool IsButtonDown(Buttons button)
		{
			return currentGamePadState.IsButtonDown(button);
		}

		public bool IsButtonPressed(Buttons button)
		{
			return currentGamePadState.IsButtonDown(button) && !previousGamePadState.IsButtonDown(button);
		}

		public bool IsButtonReleased(Buttons button)
		{
			return !currentGamePadState.IsButtonDown(button) && previousGamePadState.IsButtonDown(button);
		}


		public float RightTriggerValue => currentGamePadState.Triggers.Right;
		public Vector2 LeftThumbStick => currentGamePadState.ThumbSticks.Left;
		public float LeftTriggerValue => currentGamePadState.Triggers.Left;
		public Vector2 RightThumbStick => currentGamePadState.ThumbSticks.Right;

	}
}
