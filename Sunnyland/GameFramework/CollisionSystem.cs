using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sunnyland.GameFramework
{
	public enum BoundsChangeType
	{
		Relative,
		Absolute
	}

	public class CollisionSystem(RenderSystem renderSystem)
	{
		private Dictionary<string, Collision> collisions = new Dictionary<string, Collision>();
		private RenderSystem renderSystem = renderSystem;


		public void InitializeCollision(string name, int id, Rectangle bounds)
		{
			
			Collision collision = new Collision(id, bounds);
			collisions[name] = collision;
		}

		public void ChangeCollisionBounds(string collisionName, Point origin, Point size, BoundsChangeType changeType = BoundsChangeType.Relative)
		{

			if (collisions.TryGetValue(collisionName, out Collision collision))
			{
				switch (changeType)
				{
					case BoundsChangeType.Relative:
						collision.X += origin.X;
						collision.Y += origin.Y;
						collision.Width += size.X;
						collision.Height += size.Y;
						break;
					case BoundsChangeType.Absolute:
						collision.X = origin.X;
						collision.Y = origin.Y;
						collision.Width = size.X;
						collision.Height = size.Y;
						break;
				}
			}

		}

		private Collision localCollisions(int id)
		{
			if (collisions.Count(m => m.Value.Id == id) > 1)
			{
				foreach (var collision in collisions)
				{
					if (collision.Value.Id == id)
					{
						return collision.Value;
					}
				}
			} else if (collisions.Count(m => m.Value.Id == id) == 1)
			{
				return collisions.First(m => m.Value.Id == id).Value;
				
			}

			throw new Exception($"No collision found for id {id}");
		}

		public Rectangle GetCollisionBounds(int id)
		{
			Collision collision = localCollisions(id);
			Vector2 position = renderSystem.Data.Position[id];
			Vector2 origin = renderSystem.Data.Origin[id];
			position -= origin;

			Rectangle bbox = new Rectangle(
				(int)position.X + collision.X,
				(int)position.Y + collision.Y,
				collision.Width,
				collision.Height);



			return bbox;
		}


	}

	


	public class Collision
	{

		public Collision(int id, Rectangle localBounds)
		{
			Id = id;
			X = localBounds.X;
			Y = localBounds.Y;
			Width = localBounds.Width;
			Height = localBounds.Height;
		}
		public int Id { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}
}
