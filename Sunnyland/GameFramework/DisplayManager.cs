using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	
	public class DisplayManager
	{
		private GraphicsDeviceManager graphics;
		public Point InternalResolution { get; set; }

		private Point windowSize;
		public Point WindowSize
		{
			get => windowSize;
			set
			{
				windowSize = value;
				SetScreenSize();
			}
		}


		public Rectangle Viewport { get; set; }

		public bool IsFullScreen
		{
			set
			{
				graphics.IsFullScreen = value;
				graphics.ApplyChanges();
				SetScreenSize();
			}

			get
			{
				return graphics.IsFullScreen;
			}
		}

		private void SetScreenSize()
		{
			if (graphics.IsFullScreen)
			{
				graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			} else
			{
				graphics.PreferredBackBufferWidth = WindowSize.X;
				graphics.PreferredBackBufferHeight = WindowSize.Y;
			}
			SetBackBufferSize();
			graphics.ApplyChanges();
		}

		public void SetBackBufferSize()
		{
			int x, y, width, height;
			float windowAspectRatio = (float)graphics.PreferredBackBufferWidth / graphics.PreferredBackBufferHeight;
			float internalAspectRatio = (float)InternalResolution.X / InternalResolution.Y;
			if (internalAspectRatio < windowAspectRatio)
			{
				height = graphics.PreferredBackBufferHeight;
				width = (int)(height * internalAspectRatio);
				y = 0;
				x = (graphics.PreferredBackBufferWidth - width) / 2;
			}
			else
			{
				width = graphics.PreferredBackBufferWidth;
				height = (int)(width / internalAspectRatio);
				x = 0;
				y = (graphics.PreferredBackBufferHeight - height) / 2;
			}

			Viewport = new Rectangle(x, y, width, height);

		}

		public Vector2 ScreenToViewport(Vector2 position)
		{
			return Vector2.Zero;
		}


		public DisplayManager(GraphicsDeviceManager graphicsDevice)
		{
			this.graphics = graphicsDevice;
			graphicsDevice.HardwareModeSwitch = false;
			graphicsDevice.ApplyChanges();
		}
	}
}
