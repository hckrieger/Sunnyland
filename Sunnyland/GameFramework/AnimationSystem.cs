using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Sunnyland.GameFramework
{
	public class AnimationSystem
	{
		private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
		public Animation CurrentAnimation { get; set; }

		private DataCache<Texture2D> textureCache;
		private RenderSystem renderSystem;


		public AnimationSystem(Game game)
		{
			textureCache = game.Services.GetService<DataCache<Texture2D>>();
			renderSystem = game.Services.GetService<RenderSystem>();
		}

		public void InitializeAnimation(string name, Animation animation)
		{
			animations[name] = animation;
		}

		public void Play(string name)
		{
			if (animations.TryGetValue(name, out Animation value))
			{
				value.IsPlaying = true;
				foreach (var animation in animations)
				{
					if (value.ObjectId == animation.Value.ObjectId && animation.Value.IsPlaying == true && value != animation.Value)
					{
						animation.Value.IsPlaying = false;
						animation.Value.CurrentTime = animation.Value.Duration;
						animation.Value.Index = 0;
						return;
					}
				}

				
			} else
			{
				throw new KeyNotFoundException($"Key {name} not found");
			}
		}

		public bool HasAnimation(string name)
		{
			return animations.ContainsKey(name);
		}

		public void Update(GameTime gameTime)
		{

			foreach (var animation in animations)
			{


				var anim = animation.Value;
				if (!anim.IsPlaying)
					continue;

				anim.CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

				if (anim.CurrentTime <= 0)
				{
					if (anim.Index < anim.FrameIndices.Length - 1)
					{
						anim.Index++;
					}
					else
					{
						anim.Index = 0;


						if (anim.IsLooping == false)
						{
							anim.IsPlaying = false;
						}
					}
					anim.CurrentTime = anim.Duration;

				}

				for (int i = 0; i < renderSystem.Data.SourceRectangle.Length; i++)
				{
					if (anim.TextureName == renderSystem.Data.TextureName[i])
					{
						renderSystem.Data.SourceRectangle[i] = Utils.GetRectangleSource(anim.FrameIndices[anim.Index],
																				  textureCache.GetData(anim.TextureName).Bounds.Width,
																				  anim.FrameSize);
					}

					if (i > anim.ObjectId)
						break;
				}

			}



			//CurrentAnimation.CurrentTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

			//if (CurrentAnimation.CurrentTime <= 0)
			//{
			//	if (CurrentAnimation.Index < CurrentAnimation.FrameIndices.Length - 1)
			//	{
			//		CurrentAnimation.Index++;
			//	} else
			//	{
			//		CurrentAnimation.Index = 0;
			//	}
			//	CurrentAnimation.CurrentTime = CurrentAnimation.Duration;
			//}


		}



	}

	public class Animation
	{


		//public AnimationType Type;
		public int ObjectId;
		public string TextureName;
		public int[] FrameIndices;
		public int Frames;
		public int Index = 0;
		public float Duration;
		public float CurrentTime;
		public Point FrameSize;

		public bool IsPlaying = false;

		public bool IsLooping;
		public Action EndOfAnimationAction;

		public Animation(string textureName, Point frameSize, float duration, int[] frameIndices, int objectId, bool isLooping = true)
		{
			TextureName = textureName;
			FrameIndices = frameIndices;
			FrameSize = frameSize;
			Duration = duration;
			ObjectId = objectId;
			IsLooping = isLooping;

		//	Type = animationType;
		//	EndOfAnimationAction = (endOfAnimationAction != null) ? endOfAnimationAction : () => { };
		}
	}
}
