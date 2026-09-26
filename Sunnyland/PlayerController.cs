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

		public PlayerController(int id, float speed, Game game)
		{
			this.id = id;
			data = game.Services.GetService<RenderSystem>().Data;
			tilemap = game.Services.GetService<TilemapManager>().TileMap;

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
			//data.Position[id] += new Vector2(0, gravity * dt);

			Debug.WriteLine($"{TileFromPosition(new Vector2(BoundingBox.Left, BoundingBox.Top))}");


		}

		
		private Point TileFromPosition(Vector2 position)
		{
			return new Point((int)position.X / tilemap.TileWidth, (int)position.Y / tilemap.TileHeight);
		}
	}
}
