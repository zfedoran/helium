using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helium.serializable
{
    public class SerializableMesh
    {
        public string name;
        public SerializableVertex[] vertices;
        public int[] indices;
        public string textureName;
    }
}
