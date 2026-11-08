using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	public class CollisionEventArgs : EventArgs
	{
		public Rectangle BoundingBox1 { get; set; }
		public Rectangle BoundingBox2 { get; set; }

		public CollisionEventArgs(Rectangle boundingBox1, Rectangle boundingBox2)
		{
			BoundingBox1 = boundingBox1;
			BoundingBox2 = boundingBox2;
		}
	}
	
}
