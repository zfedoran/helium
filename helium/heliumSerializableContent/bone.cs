using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.serializable
{
    public class SerializableBone
    {
        public SerializableBone()
        { children = new List<SerializableBone>(); }

        public string name;
        public Matrix matrixLocalTransform;
        public List<SerializableBone> children;
    }
}
