using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sunnyland.GameFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sunnyland.GameFramework.InputManager;

namespace Sunnyland
{
	public class PlayerController
	{
		private RenderableData data;
		private int feetMargin = 3;

		private float gravity = 75;
		public Rectangle BoundingBox
		{
			get
			{
				Rectangle localRectangle = new Rectangle(6, 10, 18, 22);

				Vector2 position = data.Position[id] - data.Origin[id];
				return new Rectangle(6 + (int)position.X, 10 + (int)position.Y, 18, 22 + feetMargin);
			}
		}

		private int id;
		private float speed;
		private TileMap tilemap;
		private TilemapManager tilemapManager;

		public PlayerController(int id, float speed, Game game)
		{
			this.id = id;
			data = game.Services.GetService<RenderSystem>().Data;
			tilemapManager = game.Services.GetService<TilemapManager>();
			tilemap = tilemapManager.TileMap;
			this.speed = speed;
		}

		public void Update(GameTime gameTime, InputManager input, AnimationSystem anim)
		{
			//playerData.Position[Id] += new Vector2(1, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds * 25f;
			Vector2 direction = Vector2.Zero;

			if (input.Binding[InputAction.MoveLeft].Invoke())
			{
				direction = new Vector2(-1, 0);
				anim.Play("playerRun");
				data.SpriteEffects[id] = SpriteEffects.FlipHorizontally;
			} else if (input.Binding[InputAction.MoveRight].Invoke())
			{
				direction = new Vector2(1, 0);
				anim.Play("playerRun");
				data.SpriteEffects[id] = SpriteEffects.None;
			}

			if (direction == new Vector2(0, 0))
				anim.Play("playerIdle");

			var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			data.Position[id] += dt * speed * direction;
			data.Position[id] += new Vector2(0, gravity * dt);

			int leftTile = (int)MathF.Floor((float)BoundingBox.Left / (float)tilemap.TileWidth);
			int rightTile = (int)MathF.Floor((float)BoundingBox.Right / (float)tilemap.TileWidth);
			int topTile = (int)MathF.Floor((float)BoundingBox.Top / (float)tilemap.TileHeight);
			int bottomTile = (int)MathF.Floor((float)BoundingBox.Bottom / (float)tilemap.TileHeight);

		//	Debug.WriteLine($"Left: {leftTile} - Right: {rightTile}\nTop: {topTile} - Bottom {bottomTile}\n");

			for (int y = topTile; y < bottomTile + 1; y++)
			{
				for (int x = leftTile; x < rightTile + 1; x++)
				{
					bool groundCollision = tilemapManager.TileCoordinateHasLayer(new Point(x, y), "Ground");

					if (groundCollision == false)
						continue;

					

					Rectangle tileBounds = new Rectangle(x * tilemap.TileWidth, y * tilemap.TileHeight, tilemap.TileWidth, tilemap.TileHeight);

					if (!BoundingBox.Contains(tileBounds))
						continue;

					Vector2 intersectionDepth = Utils.GetIntersectionDepth(BoundingBox, tileBounds);


					if (intersectionDepth.X > intersectionDepth.Y)
					{
						data.Position[id] += new Vector2(0, intersectionDepth.Y);
					} else
					{

					}
					
					
				}
			}

		}



	}
}
