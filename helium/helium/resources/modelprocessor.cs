using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

using helium.animation;
using helium.serializable;
using Microsoft.Xna.Framework.Graphics;


namespace helium.resources
{
	public class ModelProcessor
	{
		public ModelProcessor(ContentManager content)
		{ this.content = content; }

		public AnimationModelContent Load(string assetName)
		{
			SerializableModel modelContent = content.Load<SerializableModel>(assetName);
			AnimationModelContent model = new AnimationModelContent(modelContent.name);

			SetSkeleton(model, modelContent);
			SetMeshList(model, modelContent);
			SetAnimationList(model, modelContent);

			string textureDirectory = assetName.Replace(modelContent.name, "");
			SetTextures(model, content, textureDirectory);

			return model;
		}

		private static void SetTextures(AnimationModelContent model, ContentManager content, string textureDirectory)
		{
			for (int i = 0; i < model.GetMeshCount(); i++)
			{
				AnimationMesh mesh = model.GetMesh(i);
				string textureName = mesh.GetTextureName();
				Texture2D texture = content.Load<Texture2D>(textureDirectory + RemoveFileExtension(textureName)); 
				mesh.SetTexture(texture);
			}
		}

		public static string RemoveFileExtension(string filename)
		{
			string[] name = filename.Split('.');
			if (name.Length > 0)
				return name[0];

			return "";
		}

		private static void SetSkeleton(AnimationModelContent model, SerializableModel modelContent)
		{
			AnimationSkeleton skeleton = CreateSkeleton(modelContent.skeleton);
			model.SetSkeleton(skeleton);
		}

		private static AnimationSkeleton CreateSkeleton(SerializableSkeleton skeletonContent)
		{
			AnimationSkeleton skeleton = new AnimationSkeleton();
			SerializableBone rootContent = skeletonContent.rootBone;
			AnimationBone root = new AnimationBone(rootContent.name, skeleton);
            root.SetLocalTransform(rootContent.matrixLocalTransform);
			skeleton.AddBone(root);
			CreateChildBones(rootContent, root);

			return skeleton;
		}

		private static void CreateChildBones(SerializableBone parentBoneContent, AnimationBone parentBone)
		{
			foreach (SerializableBone childBoneContent in parentBoneContent.children)
			{
				AnimationBone childBond = parentBone.CreateChildBone(childBoneContent.name);
				childBond.SetLocalTransform(ref childBoneContent.matrixLocalTransform);

				CreateChildBones(childBoneContent, childBond);
			}
		}

		private static void SetMeshList(AnimationModelContent model, SerializableModel modelContent)
		{
			foreach (SerializableMesh meshContent in modelContent.meshList)
			{
				AnimationMesh mesh = CreateMesh(meshContent);
				model.AddMesh(mesh);
			}
		}

		private static AnimationMesh CreateMesh(SerializableMesh meshContent)
		{
			AnimationMesh mesh = new AnimationMesh(meshContent.name);
			AnimationVertex[] vertices = new AnimationVertex[meshContent.vertices.Length];

			for (int i = 0; i < vertices.Length; i++)
			{
				SerializableVertex vertexContent = meshContent.vertices[i];
				vertices[i] = new AnimationVertex
				{
					position = vertexContent.position,
					normal = vertexContent.normal,
					texture = vertexContent.texture,
					blendweights = vertexContent.blendweights,
					blendindices = vertexContent.blendindices
				};
			}

			mesh.SetIndices(meshContent.indices);
			mesh.SetVertices(vertices);
			mesh.SetTextureName(meshContent.textureName);

			return mesh;
		}

		private static void SetAnimationList(AnimationModelContent model, SerializableModel modelContent)
		{
			foreach (SerializableAnimation animationContent in modelContent.animationList)
			{
				Animation animation = CreateAnimation(animationContent);
				model.AddAnimation(animation);
			}
		}

		private static Animation CreateAnimation(SerializableAnimation animationContent)
		{
			Animation animation = new Animation(animationContent.name);
            animation.SetLength(animationContent.length);

			foreach (SerializableTrack trackContent in animationContent.tracks)
			{
				AnimationTrack track = animation.CreateTrack(trackContent.name);

				for (int i = 0; i < trackContent.keyframes.Count; i++)
				{
					SerializableKeyFrame keyFrameContent = trackContent.keyframes[i];
					AnimationKeyFrame keyFrame = new AnimationKeyFrame 
					{ 
						rotation = keyFrameContent.rotation,
						scale = keyFrameContent.scale,
						translation = keyFrameContent.translation,
						time = keyFrameContent.time
					};

					track.AddKeyFrame(keyFrame);
				}
			}

			return animation;
		}

		private ContentManager content;
	}
}
