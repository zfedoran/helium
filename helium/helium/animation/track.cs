using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace helium.animation
{
    public class AnimationTrack
    {
        //////////////////////////////////////////////////////////////////////////
        // CONSTRUCTORS
        //////////////////////////////////////////////////////////////////////////

        //-----------------------------------------------------------------------

        public AnimationTrack(string name, Animation parent)
        { 
            this.name = name; 
            this.parent = parent; 
            maxFrameTime = 0; 
            keyframes = new List<AnimationKeyFrame>();
        }

        //-----------------------------------------------------------------------

        //////////////////////////////////////////////////////////////////////////
        // PUBLIC METHODS
        //////////////////////////////////////////////////////////////////////////

        //-----------------------------------------------------------------------

        public void AddKeyFrame(AnimationKeyFrame frame)
        {
            float time = frame.time;

            if (time > maxFrameTime || keyframes.Count == 0)
            {
                keyframes.Add(frame);
                maxFrameTime = time;
            }
            else
            {
                int i;
                for (i = 0; i < keyframes.Count; i++)
                {
                    if (time < keyframes[i].time)
                        break;
                }
                keyframes.Insert(i, frame);
            }
        }

        //-----------------------------------------------------------------------

        public string GetName()
        { return name; }

        public AnimationKeyFrame GetKeyFrame(int index)
        { return keyframes[index]; }

        public int GetKeyFrameCount()
        { return keyframes.Count; }

        //-----------------------------------------------------------------------

        public AnimationKeyFrame GetInterpolatedKeyFrame(float time)
        {
            AnimationKeyFrame resultKeyFrame = new AnimationKeyFrame(time);

            AnimationKeyFrame keyFrameA;
            AnimationKeyFrame keyFrameB;

            float frameTime = GetKeyFramesAtTime(time, out keyFrameA, out keyFrameB);

            if(frameTime == 0.0f)
            {
                resultKeyFrame.rotation = keyFrameA.rotation;
                resultKeyFrame.scale = keyFrameA.scale;
                resultKeyFrame.translation = keyFrameA.translation;
            }
            else
            {	
                resultKeyFrame.rotation = Quaternion.Lerp(keyFrameA.rotation, keyFrameB.rotation, frameTime);
                resultKeyFrame.scale = keyFrameA.scale * (1 - frameTime) + keyFrameB.scale * frameTime;
                resultKeyFrame.translation = keyFrameA.translation * (1 - frameTime) + keyFrameB.translation * frameTime;
            }

            return resultKeyFrame;
        }

        //-----------------------------------------------------------------------

        public float GetKeyFramesAtTime(float time, out AnimationKeyFrame keyframeA, out AnimationKeyFrame keyframeB)
        {
            //this code might be wrong
            // https://github.com/FrictionalGames/HPL1Engine/blob/master/sources/graphics/AnimationTrack.cpp#L152

            float totalAnimLength = parent.GetLength();
            time = MathHelper.Clamp(time, 0, totalAnimLength);

            //If longer than max time return last frame and first
            if(time >= maxFrameTime)
            {
                keyframeA = keyframes[keyframes.Count-1];
                keyframeB = keyframes[0];
                return 0.0f;
            }

            //Find the second frame.
            int keyframeIndexB=-1;
            for(int i=0; i< keyframes.Count; i++)
            {
                if (time <= keyframes[i].time)
                {
                    keyframeIndexB = i;
                    break;
                }
            }

            //If first frame was found, the lowest time is not 0. 
            //If so return the first frame only.
            if(keyframeIndexB == 0)
            {
                keyframeA = keyframes[0];
                keyframeB = keyframes[0];
                return 0.0f;
            }

            //Get the frames
            keyframeA = keyframes[keyframeIndexB - 1];
            keyframeB = keyframes[keyframeIndexB];

            float deltaTime = keyframeB.time - keyframeA.time;
        
            return (time - keyframeA.time) / deltaTime;
        }
        
        //-----------------------------------------------------------------------

        //////////////////////////////////////////////////////////////////////////
        // PRIVATE METHODS
        //////////////////////////////////////////////////////////////////////////

        //-----------------------------------------------------------------------

        private string name;
        private float maxFrameTime;
        private List<AnimationKeyFrame> keyframes;
        private Animation parent;
    }
}
