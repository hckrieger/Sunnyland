using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

	public class CollisionSystem
	{
		private List<Collision> collisions = new List<Collision>();
		private RenderSystem renderSystem;

		public event EventHandler<CollisionEventArgs> HandleCollision;



		public CollisionSystem(RenderSystem renderSystem)
		{
			this.renderSystem = renderSystem;

		}



		public void InitializeCollision(string name, int id, Rectangle bounds)
		{
			
			Collision collision = new Collision(id, name, bounds);
			collisions.Add(collision);
		}

		public void ChangeCollisionBounds(string collisionName, Point origin, Point size, BoundsChangeType changeType = BoundsChangeType.Relative)
		{

			if (collisions.FirstOrDefault(c => c.Key == collisionName) is Collision collision)
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



		public Rectangle GetBoundingBox(int id)
		{
			Vector2 position = renderSystem.Data.Position[id];
			Vector2 origin = renderSystem.Data.Origin[id];
			Collision collision = collisions.FirstOrDefault(c => c.Id == id);
			position -= origin;

			Rectangle bbox = new Rectangle(
				(int)position.X + collision.X,
				(int)position.Y + collision.Y,
				collision.Width,
				collision.Height);


			return bbox;
		}




		public void CheckOneToManyCollision(string boxName, string multipleBoxes, Action<CollisionEventArgs> execution)
		{
			var collision = collisions.FirstOrDefault(c => c.Key == boxName);

			if (collision == null)
				return;

			Rectangle bbox1 = GetBoundingBox(collision.Id);

			foreach (var otherCollision in collisions.Where(c => c.Key == multipleBoxes))
			{
				Rectangle bbox2 = GetBoundingBox(otherCollision.Id);

				if (bbox1.Intersects(bbox2) && !otherCollision.CollisionResolved)
				{
					execution?.Invoke(new CollisionEventArgs(collision.Id, otherCollision.Id));
					otherCollision.CollisionResolved = true;
				}
					  

				
			}


		}


	}

	


	public class Collision
	{

		public Collision(int id, string key, Rectangle localBounds)
		{
			Id = id;
			Key = key;
			X = localBounds.X;
			Y = localBounds.Y;
			Width = localBounds.Width;
			Height = localBounds.Height;
		}

		public bool CollisionResolved { get; set; } = false;

		public string Key { get; set; }
		public int Id { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}
}
