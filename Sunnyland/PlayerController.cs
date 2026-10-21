using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sunnyland.GameFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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

		private float gravity = 700f;
		private float maxFallSpeed = 300f;

		private Vector2 velocity, startPosition;
		private float walkingSpeed = 65f;
		private float jumpSpeed = 225f;

		private bool isGrounded;
		private bool IsFalling => velocity.Y > 0;
		private bool IsAscending=> velocity.Y < 0;

		private bool isDoubleJumping = false;
		private bool hasDoubleJumped = true;

		private void SetOriginToBottomCenter()
		{
			data.Origin[id] = new Vector2(data.SourceRectangle[id].Width / 2, data.SourceRectangle[id].Height);
		}




		public void EndOfDoubleJump()
		{
			isDoubleJumping = false;
			hasDoubleJumped = true;
		}

		public Rectangle BoundingBoxForCollision
		{
			get
			{
				Rectangle localRectangle = new Rectangle(11, 12, 12, 20);
				Vector2 position = data.Position[id];
				Vector2 origin = data.Origin[id];
				position -= origin;

				Rectangle bbox = new Rectangle(
					(int)position.X + localRectangle.X,
					(int)position.Y + localRectangle.Y,
					localRectangle.Width,
					localRectangle.Height + 1);

				
				
				return bbox;
			}
		}

		private int id;
		private TileMap tilemap;
		private TilemapManager tilemapManager;

		public PlayerController(int id, Game game)
		{
			this.id = id;
			data = game.Services.GetService<RenderSystem>().Data;
			tilemapManager = game.Services.GetService<TilemapManager>();
			tilemap = tilemapManager.TileMap;
			SetOriginToBottomCenter();
			
		}



		public void Update(GameTime gameTime, InputManager input, AnimationSystem anim, Rectangle winRectangleTrigger)
		{
			//playerData.Position[Id] += new Vector2(1, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds * 25f;
			Vector2 previousPosition = data.Position[id];
			float desiredHorizontalSpeed;


			if (input.Binding[InputAction.MoveLeft].Invoke() && BoundingBoxForCollision.Left > 0)
			{
				
	
				data.SpriteEffects[id] = SpriteEffects.FlipHorizontally;
				desiredHorizontalSpeed = -walkingSpeed;
			} else if (input.Binding[InputAction.MoveRight].Invoke() && BoundingBoxForCollision.Right < tilemap.Width * tilemap.TileWidth)
			{
				data.SpriteEffects[id] = SpriteEffects.None;
				desiredHorizontalSpeed = walkingSpeed;
			}
			else
			{
				desiredHorizontalSpeed = 0;
				if (isGrounded)
				{
					anim.Play("playerIdle");
					
				}
					
			}

			if (input.Binding[InputAction.Jump].Invoke() && !isGrounded && !isDoubleJumping && !hasDoubleJumped)
			{
				velocity.Y = -jumpSpeed;
				//anim.Play("playerDoubleJump");
				isDoubleJumping = true;
			}

			if (input.Binding[InputAction.Jump].Invoke() && isGrounded)
			{
				 velocity.Y = -jumpSpeed;
				isGrounded = false;
				hasDoubleJumped = false;
			}

			if (input.Binding[InputAction.JumpRelease].Invoke() && velocity.Y < 0)
			{
				velocity.Y = -jumpSpeed/2;
			}



			if (isGrounded)
			{
				if (!hasDoubleJumped)
					hasDoubleJumped = true;

				if (velocity.X == 0)
					anim.Play("playerIdle");
				else
					anim.Play("playerRun");
			} else {
				if (!isDoubleJumping)
				{
					if (IsFalling)
					{
						anim.Play("playerFall");
					}
					else if (IsAscending)
					{
						anim.Play("playerJump");
					}
				}
				else
				{
					anim.Play("playerDoubleJump");
				}

			}


			if (BoundingBoxForCollision.Intersects(winRectangleTrigger) && isGrounded)
			{
				anim.Play("playerWin");
			}


			SetOriginToBottomCenter();

			velocity.X += (desiredHorizontalSpeed - velocity.X);

			var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			ApplyGravity(dt);


			data.Position[id] += dt * velocity;

			HandleTileCollision(previousPosition);
		}

		private void ApplyGravity(float dt)
		{
			velocity.Y += gravity * dt;
			if (velocity.Y > maxFallSpeed)
				velocity.Y = maxFallSpeed;
		}

		private void HandleTileCollision(Vector2 previousPosition)
		{
			isGrounded = false;

			Rectangle bbox = BoundingBoxForCollision;

			Point topLeftTile = tilemapManager.GetTileCoordinates(new Vector2(bbox.Left, bbox.Top)) - new Point(1, 1);
			Point bottomRightTile = tilemapManager.GetTileCoordinates(new Vector2(bbox.Right, bbox.Bottom)) + new Point(1, 1);




			for (int y = topLeftTile.Y; y < bottomRightTile.Y + 1; y++)
			{
				for (int x = topLeftTile.X; x < bottomRightTile.X + 1; x++)
				{
					bool groundCollision = tilemapManager.TileCoordinateHasLayer(new Point(x, y), "Ground");

					if (groundCollision == false)
						continue;



					Rectangle tileBounds =  new Rectangle(x * tilemap.TileWidth, y * tilemap.TileHeight, tilemap.TileWidth, tilemap.TileHeight);

					if (!BoundingBoxForCollision.Intersects(tileBounds))
						continue;

					Rectangle intersection = Utils.CalculateIntersection(BoundingBoxForCollision, tileBounds);
					
					if (intersection.Width == 0 || intersection.Height == 0)
						continue;


					if (intersection.Width < intersection.Height)
					{
						if ((velocity.X >= 0 && (bbox.Center.X < tileBounds.Left)) ||
							(velocity.X <= 0 && bbox.Center.X > tileBounds.Right))
						{
							data.Position[id].X = previousPosition.X;
						}
					} else
					{
						if (velocity.Y >= 0 && bbox.Center.Y < tileBounds.Top)
						{
							isGrounded = true;
							velocity.Y = 0;
							data.Position[id].Y = tileBounds.Top;
						} else if (velocity.Y <= 0 && bbox.Center.Y > tileBounds.Bottom)
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
