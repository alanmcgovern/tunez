using System;

namespace Tunez
{
	public class ScrobblingMonitor : TrackMonitor
	{
		static readonly DateTime EpochStart = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		bool Scrobbled {
			get; set;
		}

		IScrobbler Scrobbler {
			get; set;
		}

		public ScrobblingMonitor (IScrobbler scrobbler)
		{
			Scrobbler = scrobbler;
			ProgressChanged += ScrobblingMonitor_ProgressChanged;
		}

		public override void Initialize (Track track)
		{
			base.Initialize (track);
			Scrobbled = false;
		}

		void ScrobblingMonitor_ProgressChanged(object sender, TimeSpan e)
		{
			if (!Scrobbled && Track != null && e.TotalSeconds > (Track.Duration / 2.0)) {
				Scrobbled = true;
				Track.Timestamp = (int) (DateTime.UtcNow - EpochStart).TotalSeconds;
				Scrobbler.Scrobble (Track);
			}
		}
	}
}
