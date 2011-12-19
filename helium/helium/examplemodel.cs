using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using helium.animation;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace helium
{
    public class ExampleModelClass
    {
        public ExampleModelClass(AnimationModelContent model, SkinnedEffect effect)
        {
            this.model = model;
            this.effect = effect;

            skeleton = model.GetSkeleton();
            animationStates = new List<AnimationState>();

            for (int i = 0; i < model.GetAnimationCount(); i++)
            {
                Animation animation = model.GetAnimation(i);
                AnimationState state = new AnimationState(animation);
                animationStates.Add(state);
            }

            animationTransforms = new Matrix[skeleton.GetBoneCount()];
        }

        public void PlayAnimation(int index, float speed, bool looping)
        {
            AnimationState state = animationStates[index];
            state.SetActive(true);
            state.SetLoop(looping);
            state.SetSpeed(speed);

            for (int i = 0; i < animationTransforms.Length; i++)
            {
                AnimationBone bone = skeleton.GetBoneByIndex(i);
                bone.GetLocalTransform(out animationTransforms[i]);
            }
        }

        public void StopAnimation(int index)
        {
            AnimationState state = animationStates[index];
            state.SetActive(false);
        }

        public void SetWorld(Matrix world)
        { this.world = world; }

        public void Update(float elapsed)
        {
            foreach (AnimationState state in animationStates)
            {
                if (!state.IsActive())
                    continue;

                Animation animation = state.GetAnimation();
                state.Update(elapsed);
                float timePosition = state.GetTimePosition();

                for (int i = 0; i < animation.GetTrackCount(); i++)
                {
                    AnimationTrack track = animation.GetTrack(i);
                    AnimationKeyFrame keyframe = track.GetInterpolatedKeyFrame(timePosition);
                    int boneIndex = skeleton.GetBoneIndexByName(track.GetName());
                    keyframe.GetMatrix(out animationTransforms[boneIndex]);
                }
            }

            // convert animation transforms from local to world coordinates
            for (int i = 0; i < animationTransforms.Length; i++)
            {
                int parent = skeleton.GetParentBoneIndexByIndex(i);
                if (parent == -1)
                    continue;

                Matrix.Multiply(ref animationTransforms[i], ref animationTransforms[parent], out animationTransforms[i]);
            }
                
            // create the skinning matrix 
            // the skinning matrix takes world space vertices to joint space and then back
            // to world space using the animation transform
            for (int i = 0; i < animationTransforms.Length; i++)
            {
                Matrix invWorldBoneMatrix;
                AnimationBone bone = skeleton.GetBoneByIndex(i);
                bone.GetInvWorldTransform(out invWorldBoneMatrix);

                Matrix.Multiply(ref invWorldBoneMatrix, ref animationTransforms[i], out animationTransforms[i]);
            }

            effect.SetBoneTransforms(animationTransforms);
            effect.World = world;
        }

        public void Draw(GraphicsDevice device)
        {
            for (int i = 0; i < model.GetMeshCount(); i++)
            {
                AnimationMesh mesh = model.GetMesh(i);
                effect.Texture = mesh.GetTexture();
                effect.CurrentTechnique.Passes[0].Apply();

                mesh.Draw(device);
            }
        }

        private SkinnedEffect effect;
        private AnimationModelContent model;
        private AnimationSkeleton skeleton;
        private List<AnimationState> animationStates;
        private Matrix[] animationTransforms;
        private Matrix world;
    }
}
