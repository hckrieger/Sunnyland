using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sunnyland.GameFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static Sunnyland.GameFramework.InputManager;

namespace Sunnyland
{
	public class PlayerController
	{
		private RenderableData data;
		private int feetMargin = 2;

		private float gravity = 100;

		private Vector2 velocity, startPosition;

		private bool isGrounded;

		private bool IsMoving => velocity != Vector2.Zero;

		private bool IsFalling => velocity.Y > 0 && !isGrounded;

		private void SetOriginToBottomCenter()
		{
			data.Origin[id] = new Vector2(data.SourceRectangle[id].Width / 2, data.SourceRectangle[id].Height);
		}


		public Rectangle BoundingBoxForCollision
		{
			get
			{
				//Rectangle localRectangle = new Rectangle(6, 10, 18, 22);

				//Vector2 position = data.Position[id] - data.Origin[id];
				//return new Rectangle(6 + (int)position.X, 10 + (int)position.Y, 18, 22 + feetMargin);
				Vector2 position = data.Position[id] - data.Origin[id];

				Rectangle bbox = data.SourceRectangle[id];
				bbox.X += 6 + (int)position.X;
				bbox.Y += 10 + (int)position.Y;
				bbox.Width -= 15;
				bbox.Height += 22;
				
				return bbox;
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
			Vector2 previousPosition = data.Position[id];
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



		}

		private void HandleTileCollision(Vector2 previousPosition)
		{
			isGrounded = false;

			Rectangle bbox = BoundingBoxForCollision;

			int leftTile = (int)MathF.Floor((float)BoundingBoxForCollision.Left / (float)tilemap.TileWidth);
			int rightTile = (int)MathF.Floor((float)BoundingBoxForCollision.Right / (float)tilemap.TileWidth);
			int topTile = (int)MathF.Floor((float)BoundingBoxForCollision.Top / (float)tilemap.TileHeight);
			int bottomTile = (int)MathF.Floor((float)BoundingBoxForCollision.Bottom / (float)tilemap.TileHeight);



			//	Debug.WriteLine($"Left: {leftTile} - Right: {rightTile}\nTop: {topTile} - Bottom {bottomTile}\n");

			for (int y = topTile; y < bottomTile + 1; y++)
			{
				for (int x = leftTile; x < rightTile + 1; x++)
				{
					bool groundCollision = tilemapManager.TileCoordinateHasLayer(new Point(x, y), "Ground");

					if (groundCollision == false)
						continue;



					Rectangle tileBounds = new Rectangle(x * tilemap.TileWidth, y * tilemap.TileHeight, tilemap.TileWidth, tilemap.TileHeight);

					if (!BoundingBoxForCollision.Intersects(tileBounds))
						continue;

					Vector2 intersectionDepth = Utils.GetIntersectionDepth(BoundingBoxForCollision, tileBounds);




					if (intersectionDepth.Y < intersectionDepth.X)
					{
						if ((velocity.X >= 0 && bbox.Center.X < tileBounds.Left) ||
							(velocity.X <= 0 && bbox.Center.X > tileBounds.Right))
						{
							data.Position[id].X = previousPosition.X;
						}
					} else
					{
						if (velocity.Y >= 0 && bbox.Center.Y < tileBounds.Top && intersectionDepth.X > 6)
						{
							isGrounded = true;
							velocity.Y = 0;
							data.Position[id].Y = tileBounds.Top;
						} else if (velocity.Y <= 0 && bbox.Center.Y > tileBounds.Bottom && intersectionDepth.Y > 2)
						{
							data.Position[id].Y = previousPosition.Y;
							velocity.Y = 0;
						}
					}


				}
			}
		}


	}
}
