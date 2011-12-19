using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using helium.serializable;

namespace heliumContentPipelineExtension
{
    [ContentProcessor(DisplayName = "Custom Model Processor")]
    public class CustomModelProcessor : ContentProcessor<NodeContent, SerializableModel>
    {
        private ContentProcessorContext context;
        private SerializableModel outputModel;

        private const int MAX_BONES = 59;

        public override SerializableModel Process(NodeContent xnaInputNode, ContentProcessorContext context)
        {
            //System.Diagnostics.Debugger.Launch();  

            this.context = context;

            outputModel = new SerializableModel();
            outputModel.name = RemoveFileExtension(GetFileName(context));

            ValidateMesh(xnaInputNode, context, null);

            // Find the root.
            BoneContent xnaRootBone = MeshHelper.FindSkeleton(xnaInputNode);
            
            if (xnaRootBone == null)
                throw new InvalidContentException("Input skeleton not found.");

            // We don't want to have to worry about different parts of the model being
            // in different local coordinate systems, so let's just bake everything.
            FlattenTransforms(xnaInputNode, xnaRootBone);

            // Read the bind pose and skeleton hierarchy data.
            IList<BoneContent> xnaBoneList = MeshHelper.FlattenSkeleton(xnaRootBone);

            if (xnaBoneList.Count > MAX_BONES)
                throw new InvalidContentException(string.Format("Skeleton has {0} bones, but the maximum supported is {1}.", xnaBoneList.Count, MAX_BONES));

            SerializableSkeleton skeleton = new SerializableSkeleton();
            SerializableBone root = new SerializableBone();
            root.name = xnaRootBone.Name;
            root.matrixLocalTransform = xnaRootBone.Transform;
            skeleton.rootBone = root;
            skeleton.AddBone(root);
            outputModel.skeleton = skeleton;

            CreateChildBones(xnaRootBone, root, skeleton);

            ProcessNode(xnaInputNode);
            ProcessAnimations(xnaRootBone.Animations, outputModel);

            return outputModel;
        }

        
        private void ProcessNode(NodeContent node)
        {
            // Is this node in fact a mesh?
            MeshContent mesh = node as MeshContent;
            
            if (mesh != null)
            {
                // Reorder vertex and index data so triangles will render in
                // an order that makes efficient use of the GPU vertex cache.
                MeshHelper.OptimizeForCache(mesh);

                // Process all the geometry in the mesh.
                foreach (GeometryContent geometry in mesh.Geometry)
                {
                    ProcessGeometry(geometry);
                }
            }

            // Recurse over any child nodes.
            foreach (NodeContent child in node.Children)
            {
                ProcessNode(child);
            }
        }


        private void ProcessGeometry(GeometryContent xnaGeometry)
        {
            // find and process the geometry's bone weights
            for (int i = 0; i < xnaGeometry.Vertices.Channels.Count; i++)
            {
                string channelName = xnaGeometry.Vertices.Channels[i].Name;
                string baseName = VertexChannelNames.DecodeBaseName(channelName);

                if (baseName == "Weights")
                {
                    ProcessWeightsChannel(xnaGeometry, i, outputModel.skeleton);
                }
            }
            

            // retrieve the four vertex channels we require for CPU skinning. we ignore any
            // other channels the model might have.
            string normalName = VertexChannelNames.EncodeName(Microsoft.Xna.Framework.Graphics.VertexElementUsage.Normal, 0);
            string texCoordName = VertexChannelNames.EncodeName(Microsoft.Xna.Framework.Graphics.VertexElementUsage.TextureCoordinate, 0);
            string blendWeightName = VertexChannelNames.EncodeName(Microsoft.Xna.Framework.Graphics.VertexElementUsage.BlendWeight, 0);
            string blendIndexName = VertexChannelNames.EncodeName(Microsoft.Xna.Framework.Graphics.VertexElementUsage.BlendIndices, 0);

            VertexChannel<Vector3> normals = xnaGeometry.Vertices.Channels[normalName] as VertexChannel<Vector3>;
            VertexChannel<Vector2> texCoords = xnaGeometry.Vertices.Channels[texCoordName] as VertexChannel<Vector2>;
            VertexChannel<Vector4> blendWeights = xnaGeometry.Vertices.Channels[blendWeightName] as VertexChannel<Vector4>;
            VertexChannel<Vector4> blendIndices = xnaGeometry.Vertices.Channels[blendIndexName] as VertexChannel<Vector4>;

            // create our array of vertices
            int triangleCount = xnaGeometry.Indices.Count / 3;
            SerializableVertex[] vertices = new SerializableVertex[xnaGeometry.Vertices.VertexCount];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new SerializableVertex
                {
                    position = xnaGeometry.Vertices.Positions[i],
                    normal = normals[i],
                    texture = texCoords[i],
                    blendweights = blendWeights[i],
                    blendindices = blendIndices[i]
                };
            }

            int[] indices = new int[xnaGeometry.Indices.Count];
            for (int i = 0; i < xnaGeometry.Indices.Count; i++)
                indices[i] = xnaGeometry.Indices[i];

            SerializableMesh mesh = new SerializableMesh();
            mesh.name = string.Format("mesh_{0}_{1}", outputModel.meshList.Count, xnaGeometry.Name);
            mesh.textureName = GetTextureName(xnaGeometry);
            mesh.vertices = vertices;
            mesh.indices = indices;
            outputModel.meshList.Add(mesh);
        }

        static void ProcessAnimations(AnimationContentDictionary xnaAnimations, SerializableModel model)
        {

            foreach (KeyValuePair<string, AnimationContent> xnaAnimation in xnaAnimations)
            {
                SerializableAnimation animation = ProcessAnimation(xnaAnimation.Value);
                model.animationList.Add(animation);
            }
        }

        static SerializableAnimation ProcessAnimation(AnimationContent xnaAnimation)
        {
            SerializableAnimation animation = new SerializableAnimation();
            animation.name = xnaAnimation.Name;
            animation.length = (float)xnaAnimation.Duration.TotalSeconds;

            foreach (KeyValuePair<string, AnimationChannel> xnaAnimationChannelPair in xnaAnimation.Channels)
            {
                AnimationChannel xnaAnimationChannel = xnaAnimationChannelPair.Value;
                string xnaAnimationChannelName = xnaAnimationChannelPair.Key;

                SerializableTrack track = new SerializableTrack();
                track.name = xnaAnimationChannelName;
                animation.tracks.Add(track);

                for (int i = 0; i < xnaAnimationChannel.Count; i++)
                {
                    AnimationKeyframe xnaKeyframe = xnaAnimationChannel[i];
                    SerializableKeyFrame keyframe = new SerializableKeyFrame((float)xnaKeyframe.Time.TotalSeconds, xnaKeyframe.Transform);
                    track.AddKeyFrame(keyframe);
                }
            }

            return animation;
        }

        static void ValidateMesh(NodeContent node, ContentProcessorContext context, string parentBoneName)
        {
            // Makes sure this mesh contains the kind of data we know how to animate.

            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                // Validate the mesh.
                if (parentBoneName != null)
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} is a child of bone {1}. SkinnedModelProcessor " +
                        "does not correctly handle meshes that are children of bones.",
                        mesh.Name, parentBoneName);
                }

                if (!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null,
                        "Mesh {0} has no skinning information, so it has been deleted.",
                        mesh.Name);

                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
            }
            else if (node is BoneContent)
            {
                // If this is a bone, remember that we are now looking inside it.
                parentBoneName = node.Name;
            }

            // Recurse (iterating over a copy of the child collection,
            // because validating children may delete some of them).
            foreach (NodeContent child in new List<NodeContent>(node.Children))
                ValidateMesh(child, context, parentBoneName);
        }

        static bool MeshHasSkinning(MeshContent mesh)
        {
            // Checks whether a mesh contains skininng information.

            foreach (GeometryContent geometry in mesh.Geometry)
            {
                if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()))
                    return false;
            }

            return true;
        }

        static void ProcessWeightsChannel(GeometryContent geometry, int vertexChannelIndex, SerializableSkeleton skeleton)
        {

            Dictionary<string, int> boneIndices = skeleton.boneIndexByName;
            
            // convert all of our bone weights into the correct indices and weight values
            VertexChannel<BoneWeightCollection> inputWeights = geometry.Vertices.Channels[vertexChannelIndex] as VertexChannel<BoneWeightCollection>;
            Vector4[] outputIndices = new Vector4[inputWeights.Count];
            Vector4[] outputWeights = new Vector4[inputWeights.Count];
            for (int i = 0; i < inputWeights.Count; i++)
            {
                ConvertWeights(inputWeights[i], boneIndices, outputIndices, outputWeights, i, geometry);
            }

            // create our new channel names
            int usageIndex = VertexChannelNames.DecodeUsageIndex(inputWeights.Name);
            string indicesName = VertexChannelNames.EncodeName(Microsoft.Xna.Framework.Graphics.VertexElementUsage.BlendIndices, usageIndex);
            string weightsName = VertexChannelNames.EncodeName(Microsoft.Xna.Framework.Graphics.VertexElementUsage.BlendWeight, usageIndex);

            // add in the index and weight channels
            geometry.Vertices.Channels.Insert(vertexChannelIndex + 1, indicesName, outputIndices);
            geometry.Vertices.Channels.Insert(vertexChannelIndex + 2, weightsName, outputWeights);

            // remove the original weights channel
            geometry.Vertices.Channels.RemoveAt(vertexChannelIndex);
        }

        static void ConvertWeights(BoneWeightCollection inputWeights, Dictionary<string, int> boneIndices, Vector4[] outIndices, Vector4[] outWeights, int vertexIndex, GeometryContent geometry)
        {
            // we only handle 4 weights per bone
            const int maxWeights = 4;

            // create some temp arrays to hold our values
            int[] tempIndices = new int[maxWeights];
            float[] tempWeights = new float[maxWeights];

            // cull out any extra bones
            inputWeights.NormalizeWeights(maxWeights);

            // get our indices and weights
            for (int i = 0; i < inputWeights.Count; i++)
            {
                BoneWeight weight = inputWeights[i];

                tempIndices[i] = boneIndices[weight.BoneName];
                tempWeights[i] = weight.Weight;
            }

            // zero out any remaining spaces
            for (int i = inputWeights.Count; i < maxWeights; i++)
            {
                tempIndices[i] = 0;
                tempWeights[i] = 0;
            }

            // output the values
            outIndices[vertexIndex] = new Vector4(tempIndices[0], tempIndices[1], tempIndices[2], tempIndices[3]);
            outWeights[vertexIndex] = new Vector4(tempWeights[0], tempWeights[1], tempWeights[2], tempWeights[3]);
        }

        static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            // Bakes unwanted transforms into the model geometry,
            // so everything ends up in the same coordinate system.

            foreach (NodeContent child in node.Children)
            {
                // Don't process the skeleton, because that is special.
                if (child == skeleton)
                    continue;

                // Bake the local transform into the actual geometry.
                MeshHelper.TransformScene(child, child.Transform);

                // Having baked it, we can now set the local
                // coordinate system back to identity.
                child.Transform = Matrix.Identity;

                // Recurse.
                FlattenTransforms(child, skeleton);
            }
        }

        static void CreateChildBones(BoneContent xnaParentBone, SerializableBone parentBone, SerializableSkeleton skeleton)
        {
            foreach (BoneContent xnaChildBone in xnaParentBone.Children)
            {
                SerializableBone childBone = new SerializableBone();
                childBone.name = xnaChildBone.Name;
                childBone.matrixLocalTransform = xnaChildBone.Transform;
                parentBone.children.Add(childBone);
                skeleton.AddBone(childBone);

                CreateChildBones(xnaChildBone, childBone, skeleton);
            }
        }

        public static string GetFileName(ContentProcessorContext context)
        {
            string[] path = context.OutputFilename.Split('\\');
            if (path.Length > 0)
                return path[path.Length - 1];

            return "";
        }

        public static string RemoveFileExtension(string filename)
        {
            string[] name = filename.Split('.');
            if (name.Length > 0)
                return name[0];

            return "";
        }

        static string GetTextureName(GeometryContent xnaGeometry)
        {
            if (xnaGeometry.Material != null)
            {
                string[] pathArray = xnaGeometry.Material.Textures["Texture"].Filename.Split('\\');
                if (pathArray.Length > 0)
                    return pathArray[pathArray.Length - 1];
            }

            return "";
        }
    }
}