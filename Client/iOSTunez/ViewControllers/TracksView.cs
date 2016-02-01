using System;
using System.Linq;
using UIKit;
using Tunez;
using Foundation;

namespace iOSTunez
{
	public partial class TracksView : TableViewControllerWithPlayQueue
	{
		public Album Album {
			get; set;
		}

		public TracksView (IntPtr handle) : base (handle)
		{
		}

		async partial void PlayAllPressed (UIButton sender)
		{
			try {
				await PlayQueue.PlayAsync (Album);
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Unexpected error encountered when playing all tracks");
			}
		}

		public async override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			try {
				var trackCell = (TrackCell) tableView.CellAt (indexPath);
				var track = trackCell.Track;
				await PlayQueue.PlayAsync (Album.Tracks.SkipWhile (t => t != track));
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Unexpected error encountered when playing to end of album");
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.DataSource = new TracksDataSource (Album);
			View.ReloadData ();
		}
	}

	class TracksDataSource : UITableViewDataSource
	{
		Album Album {
			get; set;
		}

		public TracksDataSource (Album album)
		{
			Album = album;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Album.Tracks.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (TrackCell) tableView.DequeueReusableCell ("TrackCell");
			cell.Track = Album.Tracks [indexPath.Row];
			return cell;
		}
	}
}
