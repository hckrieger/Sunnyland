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



		private int id;
		private float speed;
		

		public PlayerController(int id, float speed, Game game)
		{
			this.id = id;
			data = game.Services.GetService<RenderSystem>().Data;
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

			data.Position[id] += (float)gameTime.ElapsedGameTime.TotalSeconds * speed * direction;

		}

 
	}
}
