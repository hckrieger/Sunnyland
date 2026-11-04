using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	public class CollisionSystem
	{
		public 
	}

	public class CollisionData
	{

		public CollisionData(int id, Rectangle localBounds)
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
