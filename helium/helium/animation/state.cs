using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using helium.core;

namespace helium.animation
{
	public class AnimationState
	{
		//////////////////////////////////////////////////////////////////////////
		// CONSTRUCTORS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public AnimationState(Animation animation)
		{
			this.animation = animation;

			length = animation.GetLength();
			curr_position = 0;
			weight = 1;
			speed = 1.0f;
			curr_position = 0;
			prev_position = 0;

			looping = false;
			paused = false;
			active = false;

			fade_step = 0;
		}

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PUBLIC METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public void Update(float elapsed)
		{
			//Update animation
			AddTime(elapsed);

			//Fading
			if (fade_step != 0)
			{
				weight += fade_step * elapsed;

				if (fade_step < 0 && weight <= 0)
				{
					weight = 0;
					active = false;
					fade_step = 0;
				}
				else if (fade_step > 0 && weight >= 1)
				{
					weight = 1;
					fade_step = 0;
				}
			}
		}

		//-----------------------------------------------------------------------

		public void AddTime(float time)
		{
			if (paused) return;

			prev_position = curr_position;
			curr_position += time * speed;

			SetTimePosition(curr_position);
		}

		//-----------------------------------------------------------------------

		public void SetTimePosition(float position)
		{
			if (looping)
			{
				curr_position = Wrap(position, 0, length);
			}
			else
			{
				curr_position = MathHelper.Clamp(position, 0, length);
			}
		}

		public float GetTimePosition()
		{ return curr_position; }

		//-----------------------------------------------------------------------

		public void FadeIn(float time)
		{ fade_step = 1.0f / Math.Abs(time); }

		public void FadeOut(float time)
		{ fade_step = -1.0f / Math.Abs(time); }

		public bool IsFading()
		{ return fade_step != 0; }

		//-----------------------------------------------------------------------

		public void SetActive(bool active)
		{
			if (this.active == active) 
				return;

			this.active = active;

			paused = false;
			fade_step = 0;
		}

		public bool IsActive()
		{ return active; }

		//-----------------------------------------------------------------------

		public void SetLoop(bool abLoop)
		{ this.looping = abLoop; }

		public bool IsLooping()
		{ return looping; }

		//-----------------------------------------------------------------------

		public void SetPaused(bool abPaused)
		{ this.paused = abPaused; }

		public bool IsPaused()
		{ return paused; }

		//-----------------------------------------------------------------------

		public void SetLength(float length)
		{ this.length = length; }

		public float GetLength()
		{ return length; }

		//-----------------------------------------------------------------------

		public bool IsOver()
		{
			if (looping)
				return false;

			return curr_position >= length;
		}

		//-----------------------------------------------------------------------

		public void SetWeight(float weight)
		{ this.weight = weight; }

		public float GetWeight()
		{ return weight; }

		//-----------------------------------------------------------------------

		public void SetSpeed(float speed)
		{ this.speed = speed; }

		public float GetSpeed()
		{ return speed; }

		//-----------------------------------------------------------------------

		public Animation GetAnimation()
		{ return animation; }

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PRIVATE METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

        private static float Wrap(float value, float min, float max)
        {
            //Quick check if value is okay. If so just return.
            if (value >= min && value <= max) return value;

            //Change setup so that min is 0
            max = max - min;
            float fOffSet = min;
            min = 0;
            value = value - fOffSet;

            float fNumOfMax = (float)Math.Floor(Math.Abs(value / max));

            if (value >= max) value = value - fNumOfMax * max;
            else if (value < min) value = ((fNumOfMax + 1.0f) * max) + value;

            return value + fOffSet;
        }

        //-----------------------------------------------------------------------

		private Animation animation;
		
		private float length;
		private float weight;
		private float speed;
		private float curr_position;
		private float prev_position;
		private float fade_step;

		private bool active;
		private bool looping;
		private bool paused;
	}
}
