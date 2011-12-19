using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helium.serializable
{
    public class SerializableAnimation
    {
        public SerializableAnimation()
        { tracks = new List<SerializableTrack>(); }

        public string name;
        public float length;
        public List<SerializableTrack> tracks;
    }
}
