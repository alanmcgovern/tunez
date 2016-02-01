using System;
namespace Tunez
{
	public abstract class TrackMonitor : Progress<TimeSpan>
	{
		protected Track Track {
			get; private set;
		}

		public virtual void Initialize (Track track)
		{
			Track = track;
		}
	}
}
