using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helium.serializable
{
    public class SerializableTrack
    {
        public SerializableTrack()
        {
            maxFrameTime = 0;
            keyframes = new List<SerializableKeyFrame>();
        }

        public void AddKeyFrame(SerializableKeyFrame frame)
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

        public string name;
        public float maxFrameTime;
        public List<SerializableKeyFrame> keyframes;
    }
}
