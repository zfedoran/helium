using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helium.serializable
{
    public class SerializableModel
    {
        public SerializableModel()
        {
            meshList = new List<SerializableMesh>();
            animationList = new List<SerializableAnimation>();
        }

        public string name;
        public SerializableSkeleton skeleton;
        public List<SerializableMesh> meshList;
        public List<SerializableAnimation> animationList;
    }
}
