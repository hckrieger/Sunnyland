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
		public int OneId { get; set; }
		public int ManyId { get; set; }
		public CollisionEventArgs(int id1, int id2)
		{
			OneId = id1;
			ManyId = id2;
		}
	}
	
}
