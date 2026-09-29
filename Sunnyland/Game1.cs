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
		private Texture2D texture, square;

		private InputManager inputManager;

		private float speed = 50f;

		private Vector2 position;

		private TilemapManager tilemapManager;

		public Dictionary<string, List<TileObject>> ObjectData = new Dictionary<string, List<TileObject>>();
		private RenderSystem renderSystem;

		private DataCache<Texture2D> textureCache;

		private PlayerController playerController;
		private AnimationSystem animationSystem;

		private Dictionary<string, List<TileObject>> gameObjects = new Dictionary<string, List<TileObject>>();

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
				IsFullScreen = false
			};

			SetWindowSize(384, 240, 3);

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

					textureCache.Add(obj.Name, texturePath);


					RenderableDataInstance instanceData = new RenderableDataInstance
					{
						Position = new Vector2(obj.X, obj.Y),
						TextureName = obj.Name,
						Origin = new Vector2(xOrigin, yOrigin),
						SourceRectangle = Utils.GetRectangleSource(sourceIndex, textureCache.GetData(obj.Name).Bounds.Width, cellSize)
					};

					var id = renderSystem.AddRenderableObjectData(instanceData);

					switch (obj.Name)
					{
						case "Player":
							float speed = Utils.GetValue<int>(obj.Properties, "speed");
							playerController = new PlayerController(id, speed, this);
							Animation idleAnimation = new Animation(obj.Name, cellSize, .21f, [0, 1, 2, 3], id, true);
							animationSystem.InitializeAnimation("playerIdle", idleAnimation);
							Animation runAnimation = new Animation(obj.Name, cellSize, .1f, [6, 7, 8, 9, 10, 11], id, true);
							animationSystem.InitializeAnimation("playerRun", runAnimation);
							animationSystem.Play("playerIdle");
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

			playerController.Update(gameTime, inputManager, animationSystem);

			animationSystem.Update(gameTime);

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
