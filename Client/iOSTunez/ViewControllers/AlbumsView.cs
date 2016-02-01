using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Tunez;

namespace iOSTunez
{
	public partial class AlbumsView : TableViewControllerWithPlayQueue
	{
		public Artist Artist {
			get; set;
		}

		public AlbumsView (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.DataSource = new AlbumsDataSource (Artist);
			View.ReloadData ();
		}

		async partial void PlayAllPressed (UIButton sender)
		{
			try {
				await PlayQueue.PlayAsync (Artist);
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Unexpected error encountered when playing all albums");
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.DestinationViewController is TracksView)
				((TracksView) segue.DestinationViewController).Album = ((AlbumCell) sender).Album;
		}
	}

	class AlbumsDataSource : UITableViewDataSource
	{
		public Artist Artist {
			get; set;
		}

		List<Tuple<char, Album[]>> Lookup {
			get; set;
		}

		public AlbumsDataSource (Artist artist)
		{
			Lookup = new List<Tuple<char, Album[]>> ();
			foreach (var group in artist.Albums.GroupBy (t => (t.Name).FirstOrDefault ()))
				Lookup.Add (Tuple.Create (group.Key, group.OrderBy (t => (t.Name)).ToArray ()));
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return Lookup.Count;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Lookup [(int) section].Item2.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var albums = Lookup [indexPath.Section].Item2;

			var cell = (AlbumCell) tableView.DequeueReusableCell ("AlbumCell");
			cell.Album = albums [indexPath.Row];
			return cell;
		}
	}
}
