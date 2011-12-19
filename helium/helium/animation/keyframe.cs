using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.animation
{
	public struct AnimationKeyFrame
	{
		//////////////////////////////////////////////////////////////////////////
		// CONSTRUCTORS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public AnimationKeyFrame(float time)
		{ 
			this.time = time; 
			translation = Vector3.Zero; 
			scale = Vector3.Zero; 
			rotation = Quaternion.Identity; 
		}

		//-----------------------------------------------------------------------

		public AnimationKeyFrame(float time, Vector3 trans, Vector3 scale, Quaternion rotation)
		{ 
			this.time = time; 
			this.translation = trans; 
			this.scale = scale; 
			this.rotation = rotation; 
		}

		//-----------------------------------------------------------------------

		public AnimationKeyFrame(float time, Vector3 trans, Quaternion rotation)
		{
			this.time = time;
			this.translation = trans;
			this.scale = Vector3.Zero;
			this.rotation = rotation;
		}

		//-----------------------------------------------------------------------

		public AnimationKeyFrame(float time, Vector3 trans)
		{
			this.time = time;
			this.translation = trans;
			this.scale = Vector3.Zero;
			this.rotation = Quaternion.Identity;
		}

		//-----------------------------------------------------------------------

		public AnimationKeyFrame(float time, ref Matrix transform)
		{
			this.time = time;
			translation = Vector3.Zero;
			scale = Vector3.Zero;
			rotation = Quaternion.Identity; 
			SetTransform(ref transform);
		}

		//-----------------------------------------------------------------------

		public AnimationKeyFrame(float time, Matrix transform)
		{
			this.time = time;
			translation = Vector3.Zero;
			scale = Vector3.Zero;
			rotation = Quaternion.Identity;
			SetTransform(ref transform);
		}

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PUBLIC METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public void SetTransform(ref Matrix transform)
		{
			transform.Decompose(out scale, out rotation, out translation);

			if (scale.X > 0.9999f && scale.X <= 1.0001f)
				scale.X = 1;
			if (scale.Y > 0.9999f && scale.Y <= 1.0001f)
				scale.Y = 1;
			if (scale.Z > 0.9999f && scale.Z <= 1.0001f)
				scale.Z = 1;

#if DEBUG
			Validate();
#endif
		}

		//-----------------------------------------------------------------------

		public void SetTransform(Matrix transform)
		{
			transform.Decompose(out scale, out rotation, out translation);

			if (scale.X > 0.9999f && scale.X <= 1.0001f)
				scale.X = 1;
			if (scale.Y > 0.9999f && scale.Y <= 1.0001f)
				scale.Y = 1;
			if (scale.Z > 0.9999f && scale.Z <= 1.0001f)
				scale.Z = 1;

#if DEBUG
			Validate();
#endif
		}

		//-----------------------------------------------------------------------

		public void GetMatrix(out Matrix matrix)
		{
			Matrix.CreateFromQuaternion(ref rotation, out matrix);

			matrix.M41 = translation.X;
			matrix.M42 = translation.Y;
			matrix.M43 = translation.Z;

			matrix.M11 *= scale.X;
			matrix.M12 *= scale.X;
			matrix.M13 *= scale.X;

			matrix.M21 *= scale.Y;
			matrix.M22 *= scale.Y;
			matrix.M23 *= scale.Y;

			matrix.M31 *= scale.Z;
			matrix.M32 *= scale.Z;
			matrix.M33 *= scale.Z;
		}

        //-----------------------------------------------------------------------

        public void InterpolateToIdentity(float weighting)
        {
            this.translation.X *= weighting;
            this.translation.Y *= weighting;
            this.translation.Z *= weighting;
            this.scale.X = this.scale.X * weighting + (1 - weighting);
            this.scale.Y = this.scale.Y * weighting + (1 - weighting);
            this.scale.Z = this.scale.Z * weighting + (1 - weighting);

            Quaternion.Lerp(rotation, Quaternion.Identity, 1 - weighting);
        }

        //-----------------------------------------------------------------------

        public void InterpolateToKeyframe(AnimationKeyFrame keyframe, float weight)
        {
            weight = 1 - weight;
            float frameTime = MathHelper.Lerp(this.time, keyframe.time, weight);

            this.rotation = Quaternion.Lerp(this.rotation, keyframe.rotation, weight);
            this.scale = this.scale * (1 - weight) + keyframe.scale * weight;
            this.translation = this.translation * (1 - weight) + keyframe.translation * weight;
        }

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PRIVATE METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		private void Validate()
		{
			if (float.IsNaN(rotation.X) ||
				float.IsNaN(rotation.Y) ||
				float.IsNaN(rotation.Z) ||
				float.IsNaN(rotation.W) ||

				float.IsNaN(translation.X) ||
				float.IsNaN(translation.Y) ||
				float.IsNaN(translation.Z) ||

				float.IsNaN(scale.X) ||
				float.IsNaN(scale.Y) ||
				float.IsNaN(scale.Z) ||

				float.IsInfinity(rotation.X) ||
				float.IsInfinity(rotation.Y) ||
				float.IsInfinity(rotation.Z) ||
				float.IsInfinity(rotation.W) ||

				float.IsInfinity(translation.X) ||
				float.IsInfinity(translation.Y) ||
				float.IsInfinity(translation.Z) ||

				float.IsInfinity(scale.X) ||
				float.IsInfinity(scale.Y) ||
				float.IsInfinity(scale.Z))
			{
				throw new ArgumentException();
			}
		}

		//-----------------------------------------------------------------------

		public Vector3 translation;
		public Vector3 scale;
		public Quaternion rotation;
		public float time;
	}
}
