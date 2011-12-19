using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.animation
{
	public class AnimationBone
	{
		//////////////////////////////////////////////////////////////////////////
		// CONSTRUCTORS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public AnimationBone(string name, AnimationSkeleton skeleton)
		{ 
			this.name = name; 
			this.skeleton = skeleton; 
			needsUpdate = true;
			children = new List<AnimationBone>();
            matrixLocalTransform = Matrix.Identity;
		}

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PUBLIC METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public AnimationBone CreateChildBone(string name)
		{
			AnimationBone bone = new AnimationBone(name, skeleton);
			bone.parent = this;
			children.Add(bone);
			skeleton.AddBone(bone);

			return bone;
		}

		//-----------------------------------------------------------------------

		public void SetLocalTransform(ref Matrix transform)
		{ matrixLocalTransform = transform; needsUpdate = true; }

		public void SetLocalTransform(Matrix transform)
		{ matrixLocalTransform = transform; needsUpdate = true; }

		public void GetLocalTransform(out Matrix transform)
		{ transform = matrixLocalTransform; }

		//-----------------------------------------------------------------------

		public void GetWorldTransform(out Matrix transform)
		{
			UpdateMatrix();
			transform = matrixWorldTransform;
		}

		public void GetInvWorldTransform(out Matrix transform)
		{
			UpdateMatrix();
			transform = matrixInvWorldTransform; 
		}

		public string GetName() 
		{ return name; }

		public ICollection<AnimationBone> GetChildIterator()
		{ return children; }

		public AnimationBone GetParent() 
		{ return parent; }

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PRIVATE METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		private void UpdateMatrix()
		{ 
			if (needsUpdate == false) 
				return;

			if(parent == null)
			{
				matrixWorldTransform = matrixLocalTransform;
			}
			else
			{
				Matrix parentWorldTransform;
				parent.GetWorldTransform(out parentWorldTransform);
                Matrix.Multiply(ref matrixLocalTransform, ref parentWorldTransform, out matrixWorldTransform);
			}

			Matrix.Invert(ref matrixWorldTransform, out matrixInvWorldTransform);

			needsUpdate = false;
		}

		//-----------------------------------------------------------------------

		private void NeedsUpdate()
		{
			needsUpdate = true;

			for (int i = 0; i < children.Count; i++)
			{
				AnimationBone bone = children[i];
				bone.NeedsUpdate();
			}
		}

		//-----------------------------------------------------------------------

		private string name;

		private Matrix matrixLocalTransform;
		private Matrix matrixWorldTransform;
		private Matrix matrixInvWorldTransform;

		private AnimationBone parent;
		private List<AnimationBone> children;
		private AnimationSkeleton skeleton;
		private bool needsUpdate;
	}
}
