using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	
	public class TilemapManager
	{
		public TileMap TileMap { get; set; }
		private ContentManager Content;

		public TilemapManager(string tileMapPath, ContentManager content)
		{
			Content = content;
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			TileMap = JsonSerializer.Deserialize<TileMap>(File.ReadAllText(tileMapPath), options);


		}


		private Texture2D GetTexture(int gid)
		{
			var tileset = GetTileSet(gid);

			string path = Path.GetDirectoryName(tileset.Image);
			string filename = Path.GetFileNameWithoutExtension(tileset.Image);
			var splitContentPath = path.Split("Content\\");
			var contentPath = splitContentPath[1];

			return Content.Load<Texture2D>($"{contentPath}/{filename}");
		}

		private TileSet GetTileSet(int gid)
		{
			for (int i = TileMap.TileSets.Count - 1; i >= 0; i--)
			{
				if (TileMap.TileSets[i].FirstGid <= gid)
				{
					return TileMap.TileSets[i];
				}
			}

			return null;
		}

	

		public bool TileCoordinateHasLayer(Point coordinate, string layerName)
		{
			foreach (var layer in TileMap.Layers)
			{
				if (layer.Name != layerName)
					continue;

				for (int i = 0; i < layer.Data.Count; i++)
				{
					if (layer.Data[i] == 0)
						continue;

					var tileCoordinate = Utils.IntToPoint(i, TileMap.Width);



					if (coordinate == tileCoordinate)
						return true;


				}
			}

			return false;
		}

		private Rectangle GetSourceRectangle(int gid)
		{
			var tileset = GetTileSet(gid);

			int localId = gid - tileset.FirstGid;

			int x = localId % tileset.Columns * tileset.TileWidth;
			int y = localId / tileset.Columns * tileset.TileHeight;

			int width = tileset.TileWidth;
			int height = tileset.TileHeight;

			return new Rectangle(x, y, width, height);

		}

		private Vector2 TilePosition(int index, int gid)
		{
			int x = index % TileMap.Width * TileMap.TileWidth;
			int y = index / TileMap.Width * TileMap.TileHeight;

			return new Vector2(x, y);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
		
			foreach (var layer in TileMap.Layers)
			{
				if (layer == null || layer.Type != "tilelayer" || !layer.Visible) 
					continue;


				for (var i = 0; i < layer.Data.Count; i++)
				{
					if (layer.Data[i] == 0)
						continue;
					spriteBatch.Draw(GetTexture(i), TilePosition(i, layer.Data[i]), GetSourceRectangle(layer.Data[i]), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .5f);	
				}
				
			}
			
			
		}
	}



	public class TileMap
	{
		public int Width { get; set; }
		public int Height { get; set; }

		public int TileWidth { get; set; }
		public int TileHeight { get; set; }

		public List<TileLayer> Layers { get; set; } = [];
		public List<TileSet> TileSets { get; set; } = [];
	}

	public class TileLayer
	{
		public string Name { get; set; } = "";

		public int Width { get; set; }
		public int Height { get; set; }
		public bool Visible { get; set; }
		public string Type { get; set; } = "";

		public List<int> Data { get; set; } = [];
		public List<TileObject> Objects { get; set; } = [];

		
	}

	public class TileObject
	{
		public int Id { get; set; }
		public string Name { get; set; } = "";

		public float X { get; set; }
		public float Y { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }

		public bool Point { get; set; }

		public List<TileProperty> Properties { get; set; } = new();
	}

	public class TileSet
	{
		public int FirstGid { get; set; }

		public string Image { get; set; } = "";

		public int ImageWidth { get; set; }
		public int ImageHeight { get; set; }

		public int TileWidth { get; set; }
		public int TileHeight { get; set; }

		public int Columns { get; set; }
		public int TileCount { get; set; }
	}

	public class TileProperty
	{
		public string Name { get; set; } = "";
		public string Type { get; set; } = "";
		public object Value { get; set; }
	}
}
