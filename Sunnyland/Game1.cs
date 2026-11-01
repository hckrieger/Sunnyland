using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sunnyland.GameFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Sunnyland.GameFramework.InputManager;

namespace Sunnyland
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private DisplayManager displayManager;
		private RenderTarget2D renderTarget;
		private Texture2D texture;

		private InputManager inputManager;

		private TilemapManager tilemapManager;

		public Dictionary<string, List<TileObject>> ObjectData = new Dictionary<string, List<TileObject>>();
		private RenderSystem renderSystem;

		private DataCache<Texture2D> textureCache;

		private PlayerController playerController;
		private Rectangle winRectangleTrigger;
		private AnimationSystem animationSystem;

		private Dictionary<string, List<TileObject>> gameObjects = new Dictionary<string, List<TileObject>>();

		public enum GameState
		{
			IsPlaying,
			Win,

		}

		public static GameState CurrentGameState { get; set; }


		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;


			
		}

		private void SetRenderTarget()
		{
			if (renderTarget != null)
				renderTarget.Dispose();
	
			renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, displayManager.InternalResolution.X, displayManager.InternalResolution.Y);
			
		}

		private void SetWindowSize(int width, int height, int scale = 3)
		{
			

			if (displayManager != null)
			{
				displayManager.InternalResolution = new Point(width, height);
				displayManager.WindowSize = new Point(width * scale, height * scale);
			}
			SetRenderTarget();
		}



		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			displayManager = new DisplayManager(_graphics)
			{
				IsFullScreen = true
			};

			SetWindowSize(384, 240, 3);
			CurrentGameState = GameState.IsPlaying;

			tilemapManager = new TilemapManager("Data/tilemaps/level.json", Content);
			textureCache = new DataCache<Texture2D>(Content.Load<Texture2D>);
			renderSystem = new RenderSystem(24, textureCache);
			inputManager = new InputManager(displayManager, InputManager.BindingType.Platformer);

			Services.AddService(typeof(TilemapManager), tilemapManager);	
			Services.AddService(typeof(RenderSystem), renderSystem);
			Services.AddService(typeof(DataCache<Texture2D>), textureCache);
			Services.AddService(typeof(InputManager), inputManager);

			animationSystem = new AnimationSystem(this);

			foreach (var layer in tilemapManager.TileMap.Layers)
			{
				if (layer.Type != "objectgroup" || !layer.Visible)
					continue;

				if (!gameObjects.ContainsKey(layer.Name))
				{
					gameObjects.Add(layer.Name, new List<TileObject>());

					foreach (var obj in layer.Objects)
						gameObjects[layer.Name].Add(obj);
					
						
				}
			}

			foreach (KeyValuePair<string, List<TileObject>> kvp in gameObjects)
			{
				foreach (var obj in kvp.Value)
				{
					
					string texturePath = Utils.GetValue<string>(obj.Properties, "texture path");
					int xOrigin = Utils.GetValue<int>(obj.Properties, "x origin");
					int yOrigin = Utils.GetValue<int>(obj.Properties, "y origin");
					int sourceIndex = Utils.GetValue<int>(obj.Properties, "source index");
					int cellWidth = Utils.GetValue<int>(obj.Properties, "cell width");
					int cellHeight = Utils.GetValue<int>(obj.Properties, "cell height");

					Point cellSize = new Point(cellWidth, cellHeight);


					
					RenderableDataInstance instanceData = default;

					if (texturePath != null)
					{

						textureCache.Add(obj.Name, texturePath);
						instanceData = new RenderableDataInstance
						{
							Position = new Vector2(obj.X, obj.Y),
							TextureName = obj.Name,
							Origin = new Vector2(xOrigin, yOrigin),
							SourceRectangle = Utils.GetRectangleSource(sourceIndex, textureCache.GetData(obj.Name).Bounds.Width, cellSize)
						};
					}


					var id = (instanceData.TextureName != null) ? renderSystem.AddRenderableObjectData(instanceData) : -1;

					switch (obj.Name)
					{
						case "Player":
							playerController = new PlayerController(id, this);
							Animation idleAnimation = new Animation(obj.Name, cellSize, .21f, [0, 1, 2, 3], id, true);
							animationSystem.InitializeAnimation("playerIdle", idleAnimation);
							Animation runAnimation = new Animation(obj.Name, cellSize, .08f, [6, 7, 8, 9, 10, 11], id, true);
							animationSystem.InitializeAnimation("playerRun", runAnimation);
							animationSystem.Play("playerIdle");
							Animation jumpAnimation = new Animation(obj.Name, cellSize, 1f, [30], id, true);
							animationSystem.InitializeAnimation("playerJump", jumpAnimation);
							Animation fallAnimation = new Animation(obj.Name, cellSize, 1f, [31], id, true);
							animationSystem.InitializeAnimation("playerFall", fallAnimation);

							Animation doubleJumpAnimation = new Animation(obj.Name, cellSize, .08f, [54, 55, 56, 57], id, false)
							{
								EndOfAnimationAction = () => playerController.EndOfDoubleJump()
							};
							
							animationSystem.InitializeAnimation("playerDoubleJump", doubleJumpAnimation);

							Animation winAnimation = new Animation(obj.Name, cellSize, 1f, [66], id);
							animationSystem.InitializeAnimation("playerWin", winAnimation);
							break;
						case "Gem":
							Animation gemAnimation = new Animation(obj.Name, cellSize, .18f, [0, 1, 2, 3], id, true);
							animationSystem.InitializeAnimation("gem", gemAnimation);
							animationSystem.Play("gem");
							break;
						case "Cherry":
							Animation cherryAnimation = new Animation(obj.Name, cellSize, .25f, [0, 1, 2, 3, 4, 3, 2, 1], id, true);
							animationSystem.InitializeAnimation("cherry", cherryAnimation);


							animationSystem.Play("cherry");

							break;
						case "End Area":
							winRectangleTrigger = new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
							break;
					}

				}
			}
		
			






			base.Initialize();
		}
		

	
		

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			texture = Content.Load<Texture2D>("environment/background/back");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			inputManager.Update();

			playerController.Update(gameTime, inputManager, animationSystem, winRectangleTrigger);



			animationSystem.Update(gameTime);

			if (CurrentGameState != GameState.IsPlaying && inputManager.IsKeyPressed(Keys.Enter))
			{
				
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			_graphics.GraphicsDevice.SetRenderTarget(renderTarget);

			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			
			_spriteBatch.Draw(texture, Vector2.Zero, Color.White);
			tilemapManager.Draw(_spriteBatch);
			renderSystem.Draw(_spriteBatch, Content);
			_spriteBatch.End();

			_graphics.GraphicsDevice.SetRenderTarget(null);


			_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			_spriteBatch.Draw(renderTarget, displayManager.Viewport, Color.White);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
