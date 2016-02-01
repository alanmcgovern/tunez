using System;

namespace Tunez
{
	public class PlayingProgress : Progress<TimeSpan>
	{
		Scrobbler Scrobbler {
			get; set;
		}
		Track Track {
			get; set;
		}

		public PlayingProgress (Scrobbler scrobbler, Track track)
		{
			Scrobbler = scrobbler;
			Track = track;
		}

		public void Report (TimeSpan progress)
		{
			OnReport (progress);
		}

		protected override void OnReport(TimeSpan progress)
		{
			base.OnReport(progress);
			if (Track.Duration > progress.TotalSeconds / 2) {
				// Undo redo is screwed up in this file
				// That was me pressing 'undo'.
				// I did it again
				// Redo does nothing (as expected because i'm typign)
			}
		}
	}
}

