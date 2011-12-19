using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace helium.animation
{
	public class Animation
	{
		//////////////////////////////////////////////////////////////////////////
		// CONSTRUCTORS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public Animation(string name)
		{
			this.name = name;
			tracks = new List<AnimationTrack>();
		}

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PUBLIC METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		public float GetLength()
		{ return length; }

		public void SetLength(float time)
		{ length = time; }

		public int GetTrackCount()
		{ return tracks.Count; }

		public string GetName()
		{ return name; }

		public void SetName(string name)
		{ this.name = name; }

		//-----------------------------------------------------------------------

		public AnimationTrack CreateTrack(string name)
		{
			AnimationTrack track = new AnimationTrack(name, this);
			tracks.Add(track);
			return track;
		}

		public AnimationTrack GetTrack(int index)
		{ 
			return tracks[index]; 
		}

		public AnimationTrack GetTrackByName(string name)
		{
			for (int i = 0; i < tracks.Count; i++)
			{
				AnimationTrack track = tracks[i];
				if (track.GetName() == name)
					return track;
			}

			return null;
		}

		//-----------------------------------------------------------------------

		//////////////////////////////////////////////////////////////////////////
		// PRIVATE METHODS
		//////////////////////////////////////////////////////////////////////////

		//-----------------------------------------------------------------------

		private string name;
		private float length;
		private List<AnimationTrack> tracks;
	}
}
