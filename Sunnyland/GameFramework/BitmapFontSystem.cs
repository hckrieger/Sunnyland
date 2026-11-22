using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	public class BitmapFontSystem
	{
		public  AssetCache<Texture2D> textureCache;
		private Dictionary<string, Font> fonts = new Dictionary<string, Font>();

		public BitmapFontSystem(ContentManager content, Game game)
		{
			textureCache = game.Services.GetService<AssetCache<Texture2D>>();
		}

		public void AddFont(string key, Font font)
		{
			if (!fonts.ContainsKey(key))
			{
				fonts.Add(key, font);
			} else
			{
				throw new Exception($"{key} already exists");
			}
		}

		public Font GetFont(string key)
		{
			if (fonts.TryGetValue(key, out Font font))
			{
				return font;
			} else
			{
				throw new Exception($"{key} does not exist");
			}
		}

		private Rectangle CharSource(char index, Point size)
		{
			Point point = Utils.IntToPoint(index, size.X);

			return new Rectangle(point.X, point.Y, size.X, size.Y);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			foreach (var font in fonts.Values)
			{
				for (int i = 0; i < font.Text.Length; i++)
				{
					Vector2 position = font.Position + new Vector2(font.Size.X * i, font.Size.Y);

					spriteBatch.Draw(textureCache.GetData(font.TextureName), position, font.Color);

				}
			}
		}

	}

	public class Font
	{
		public string Text { get; set; }
		public string TextureName { get; set; }
		public Vector2 Position { get; set; }
		public Point Size { get; set; } = new Point(8, 8);
		public Color Color { get; set; } = Color.White;

		public Font(string textureName, string text, Vector2 position, Point size, Color color = default)
		{
			Text = text;
			TextureName = textureName;
			Position = position;
			Size = size;
			Color = color;
		}
	}
}
