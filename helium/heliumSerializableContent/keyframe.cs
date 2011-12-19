using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.serializable
{
    public struct SerializableKeyFrame
    {
        public SerializableKeyFrame(float time, Matrix transform)
        {
            this.time = time;
            translation = Vector3.Zero;
            scale = Vector3.Zero;
            rotation = Quaternion.Identity;
            SetTransform(ref transform);
        }

        public void SetTransform(ref Matrix transform)
        {
            transform.Decompose(out scale, out rotation, out translation);

            if (scale.X > 0.9999f && scale.X <= 1.0001f)
                scale.X = 1;
            if (scale.Y > 0.9999f && scale.Y <= 1.0001f)
                scale.Y = 1;
            if (scale.Z > 0.9999f && scale.Z <= 1.0001f)
                scale.Z = 1;
        }

        public Vector3 translation;
        public Vector3 scale;
        public Quaternion rotation;
        public float time;
    }
}
