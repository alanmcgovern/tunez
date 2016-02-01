using System;
using AppKit;

namespace Tunez
{
	public class PlayingProgressMonitor : TrackMonitor
	{
		NSTextField Label {
			get; set;
		}

		NSProgressIndicator Progress {
			get; set;
		}

		NSToolbarItem ProgressItem {
			get; set;
		}

		public PlayingProgressMonitor (NSToolbarItem progressItem, NSTextField label)
		{
			ProgressItem = progressItem;
			Progress = (NSProgressIndicator) progressItem.View;
			Label = label;
			ProgressChanged += (o, e) => UpdateLabels (e);
		}

		public override void Initialize (Track track)
		{
			base.Initialize (track);
			Progress.MinValue = 0;
			Progress.MaxValue = (track?.Duration).GetValueOrDefault (1);
			UpdateLabels (TimeSpan.Zero);
		}

		void UpdateLabels (TimeSpan e)
		{
			if (Track == null) {
				Label.StringValue = "";
				Progress.DoubleValue = 0;
				ProgressItem.Label = "";
			} else {
				Label.StringValue = string.Format ("{0} - {1} - {2}", Track.AlbumArtist, Track.Album, Track.Name);
				Progress.DoubleValue = e.TotalSeconds;
				ProgressItem.Label = e.ToString (@"mm\:ss") + "/" + TimeSpan.FromSeconds (Track.Duration).ToString (@"mm\:ss");
			}
		}
	}
}
