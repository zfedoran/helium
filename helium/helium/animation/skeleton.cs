using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.animation
{
	public class AnimationSkeleton
	{
		//////////////////////////////////////////////////////////////////////////
		// CONSTRUCTORS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public AnimationSkeleton()
		{
			boneList = new List<AnimationBone>();
			boneIndexByName = new Dictionary<string, int>();
		}

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PUBLIC METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public void AddBone(AnimationBone bone)
		{
            if (boneList.Count == 0)
                rootBone = bone;

			boneList.Add(bone);
			boneIndexByName.Add(bone.GetName(), boneList.Count - 1);
		}

		//-----------------------------------------------------------------------

		public void RemoveBone(AnimationBone bone)
		{
			boneList.Remove(bone);
			boneIndexByName.Clear();

			for (int i = 0; i < boneList.Count; i++)
			{
				boneIndexByName.Add(boneList[i].GetName(), i);
			}
		}

		//-----------------------------------------------------------------------

		public AnimationBone GetRootBone()
		{ return rootBone; }

		//-----------------------------------------------------------------------

		public AnimationBone GetBoneByIndex(int index)
		{ return boneList[index]; }

		//-----------------------------------------------------------------------

		public AnimationBone GetBoneByName(string name)
		{
			int index = GetBoneIndexByName(name);
			if (index < 0)
				return null;

			return boneList[index];
		}

		//-----------------------------------------------------------------------

		public int GetBoneIndexByName(string name)
		{
			if (!boneIndexByName.ContainsKey(name))
				return -1;

			return boneIndexByName[name];
		}

		//-----------------------------------------------------------------------

		public int GetBoneCount()
		{ return boneList.Count; }

		//-----------------------------------------------------------------------

        public int[] GetHierarchy()
        {
            int[] hierarchy = new int[boneList.Count];

            for (int i = 0; i < hierarchy.Length; i++)
            {
                hierarchy[i] = GetParentBoneIndexByIndex(i);
            }

            return hierarchy;
        }

        //-----------------------------------------------------------------------

        public int GetParentBoneIndexByIndex(int index)
        {
            AnimationBone bone = GetBoneByIndex(index);
            AnimationBone parent = bone.GetParent();

            if (parent != null)
                return boneIndexByName[parent.GetName()];
            else
                return -1;
        }

        //-----------------------------------------------------------------------

        public int GetParentBoneIndexByName(string name)
        {
            AnimationBone bone = GetBoneByName(name);
            AnimationBone parent = bone.GetParent();

            if (parent != null)
                return boneIndexByName[parent.GetName()];
            else
                return -1;
        }

        //-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PRIVATE METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		private AnimationBone rootBone;
		private List<AnimationBone> boneList;
		private Dictionary<string, int> boneIndexByName;
	}
}
