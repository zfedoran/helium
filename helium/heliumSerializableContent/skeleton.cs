using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.serializable
{
    public class SerializableSkeleton
    {
        public SerializableSkeleton()
        { 
            boneList = new List<SerializableBone>();
            boneIndexByName = new Dictionary<string, int>(); 
        }

        public void AddBone(SerializableBone bone)
        {
            boneList.Add(bone);
            boneIndexByName.Add(bone.name, boneList.Count - 1);
        }

        public SerializableBone rootBone;
        public List<SerializableBone> boneList;
        public Dictionary<string, int> boneIndexByName;
    }
}
