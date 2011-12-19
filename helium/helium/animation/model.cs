using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helium.animation
{
	public class AnimationModelContent
	{
		//////////////////////////////////////////////////////////////////////////
		// CONSTRUCTORS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public AnimationModelContent(string name)
		{ 
			this.name = name;
			meshList = new List<AnimationMesh>();
			animationList = new List<Animation>();
			meshIndexByName = new Dictionary<string, int>();
			animationIndexByName = new Dictionary<string, int>();
		}

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PUBLIC METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public void SetSkeleton(AnimationSkeleton skeleton)
		{ this.skeleton = skeleton; }

		//-----------------------------------------------------------------------

		public AnimationSkeleton GetSkeleton()
		{ return skeleton; }

		//-----------------------------------------------------------------------

		public void AddAnimation(Animation animation)
		{
			animationList.Add(animation);
			animationIndexByName.Add(animation.GetName(), animationList.Count - 1);
		}


		//-----------------------------------------------------------------------

		public void AddMesh(AnimationMesh mesh)
		{
			meshList.Add(mesh);
			meshIndexByName.Add(mesh.GetName(), meshList.Count - 1);
		}

		//-----------------------------------------------------------------------

		public Animation GetAnimation(int index)
		{ return animationList[index]; }

		//-----------------------------------------------------------------------

		public AnimationMesh GetMesh(int index)
		{ return meshList[index]; }

		//-----------------------------------------------------------------------

		public Animation GetAnimationFromName(string name)
		{
			if (animationIndexByName.ContainsKey(name))
			{
				int index = animationIndexByName[name];
				return animationList[index];
			}

			return null;
		}

		//-----------------------------------------------------------------------

		public AnimationMesh GetMeshFromName(string name)
		{
			if (animationIndexByName.ContainsKey(name))
			{
				int index = meshIndexByName[name];
				return meshList[index];
			}

			return null;
		}

		//-----------------------------------------------------------------------

		public int GetAnimationIndex(string name)
		{
			if (animationIndexByName.ContainsKey(name))
			{
				int index = animationIndexByName[name];
				return index;
			}

			return -1;
		}

		//-----------------------------------------------------------------------

		public int GetMeshIndex(string name)
		{
			if (meshIndexByName.ContainsKey(name))
			{
				int index = meshIndexByName[name];
				return index;
			}

			return -1;
		}

		//-----------------------------------------------------------------------

		public void ClearAnimations()
		{
			animationList.Clear();
			animationIndexByName.Clear();
		}

		//-----------------------------------------------------------------------

		public void ClearMeshes()
		{
			meshList.Clear();
			meshIndexByName.Clear();
		}

		//-----------------------------------------------------------------------

		public int GetAnimationCount()
		{ return animationList.Count; }

		//-----------------------------------------------------------------------

		public int GetMeshCount()
		{ return meshList.Count; }

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PRIVATE METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		private string name;
		private AnimationSkeleton skeleton;
		private List<AnimationMesh> meshList;
		private List<Animation> animationList;
		private Dictionary<string, int> meshIndexByName;
		private Dictionary<string, int> animationIndexByName;
	}
}
