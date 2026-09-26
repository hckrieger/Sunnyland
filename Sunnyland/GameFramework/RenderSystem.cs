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
	public class RenderSystem
	{
		private int capacity;
		private RenderableData renderableData;
		public RenderableData Data => renderableData;
		private int index = 0;

		DataCache<Texture2D> assetCache;


		public RenderSystem(int capacity, DataCache<Texture2D> assetCache)
		{
			this.capacity = capacity;
			renderableData = new RenderableData(capacity);
			this.assetCache = assetCache;

		}



		public int AddRenderableObjectData(RenderableDataInstance renderableObject)
		{
			if (index < 0 || index >= capacity)
				throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {capacity - 1}.");
			
			renderableData.Position[index] = renderableObject.Position;
			renderableData.TextureName[index] = renderableObject.TextureName;
			renderableData.SourceRectangle[index] = renderableObject.SourceRectangle;
			renderableData.Color[index] = renderableObject.Color;
			renderableData.Rotation[index] = renderableObject.Rotation;
			renderableData.Origin[index] = renderableObject.Origin;
			renderableData.Scale[index] = renderableObject.Scale;
			renderableData.SpriteEffects[index] = renderableObject.SpriteEffects;
			renderableData.LayerDepth[index] = renderableObject.LayerDepth;
			return index++;
		}


		public void Draw(SpriteBatch spriteBatch, ContentManager Content)
		{
			for (int i = 0; i < capacity; i++)
			{
				if (renderableData.TextureName[i] != null)
				{
					var texture = assetCache.GetData(renderableData.TextureName[i]);
					spriteBatch.Draw(texture, renderableData.Position[i], renderableData.SourceRectangle[i], renderableData.Color[i], renderableData.Rotation[i], renderableData.Origin[i], renderableData.Scale[i], renderableData.SpriteEffects[i], renderableData.LayerDepth[i]);
				}
			}
		}
	}

	public struct RenderableData(int capacity)
	{
		public Vector2[] Position = new Vector2[capacity];
		public string[] TextureName = new string[capacity];
		public Rectangle[] SourceRectangle = new Rectangle[capacity];
		public Color[] Color = new Color[capacity];
		public float[] Rotation = new float[capacity];
		public Vector2[] Origin = new Vector2[capacity];
		public Vector2[] Scale = new Vector2[capacity];
		public SpriteEffects[] SpriteEffects = new SpriteEffects[capacity];
		public float[] LayerDepth = new float[capacity];
	}

	public struct RenderableDataInstance()
	{
		public Vector2 Position = Vector2.Zero;
		public string TextureName = "";
		public Rectangle SourceRectangle = Rectangle.Empty;
		public Color Color = Color.White;
		public float Rotation = 0f;
		public Vector2 Origin = Vector2.Zero;
		public Vector2 Scale = Vector2.One;
		public SpriteEffects SpriteEffects = SpriteEffects.None;
		public float LayerDepth = .5f;

	}
}
