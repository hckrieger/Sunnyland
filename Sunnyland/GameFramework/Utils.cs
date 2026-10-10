using Microsoft.Xna.Framework;
using SharpDX.MediaFoundation;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	public static class Utils
	{
		public static Point IntToPoint(int index, int width)
		{
			return new Point(index % width, index / width);
		}

		public static int PointToInt(Point point, int width)
		{
			return point.Y * width + point.X;
		}

		public static Rectangle GetRectangleSource(int index, int width, Point cellSize)
		{
			int columns = width / cellSize.X;
			Point coordinate = IntToPoint(index, columns);

			return new Rectangle(
				coordinate.X * cellSize.X,
				coordinate.Y * cellSize.Y,
				cellSize.X,
				cellSize.Y
			);
		}

		public static bool ShapesIntersect(Rectangle rectangle1, Rectangle rectangle2)
		{
			return rectangle1.Intersects(rectangle2);
		}

		public static Rectangle CalculateIntersection(Rectangle rect1, Rectangle rect2)
		{
			if (!ShapesIntersect(rect1, rect2))
				return new Rectangle(0, 0, 0, 0);

			int xmin = Math.Max(rect1.Left, rect2.Left);
			int xmax = Math.Min(rect1.Right, rect2.Right);
			int ymin = Math.Max(rect1.Top, rect2.Top);
			int ymax = Math.Min(rect1.Bottom, rect2.Bottom);
			return new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
		}

		public static T GetValue<T>(this List<TileProperty> propList, string name)
		{
			var prop = propList.FirstOrDefault(m => m.Name == name);

			if (prop == null)
				return default;

			if (prop?.Value is JsonElement element)
			{
				switch (prop.Type)
				{
					case "int":
						return (T)(object)element.GetInt32();

					case "float":
						return (T)(object)element.GetSingle();

					case "string":
						return (T)(object)element.GetString();

					case "bool":
						return (T)(object)element.GetBoolean();
				}
			}

			return (T)prop.Value;
		}

	}
}
