using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.serializable
{
    public struct SerializableVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texture;
        public Vector4 blendweights;
        public Vector4 blendindices;

        public override string ToString()
        { return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", position, normal, texture, blendweights, blendindices); }
    }
}
