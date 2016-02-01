using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Tunez;

namespace iOSTunez
{
	public partial class ArtistsView : TableViewControllerWithPlayQueue
	{
		new CatalogController NavigationController {
			get { return (CatalogController) base.NavigationController; }
		}

		public ArtistsView (IntPtr handle) : base (handle)
		{
		}

		async partial void RandomPlayAllPressed (UIButton sender)
		{
			try {
				await PlayQueue.PlayAsync (NavigationController.Catalog.Randomize ());
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Unexpected error encountered when playing all artists");
			}
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			if (segue.DestinationViewController is AlbumsView)
				((AlbumsView) segue.DestinationViewController).Artist = ((ArtistCell) sender).Artist;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.DataSource = new ArtistsDataSource (NavigationController.Catalog);
			View.ReloadData ();
		}
	}

	class ArtistsDataSource : UITableViewDataSource
	{
		public Catalog Catalog {
			get; set;
		}

		List<Tuple<string, Artist[]>> Lookup {
			get; set;
		}

		public ArtistsDataSource (Catalog catalog)
		{
			Lookup = new List<Tuple<string, Artist[]>> ();
			if (catalog != null)
				foreach (var group in catalog.Artists.GroupBy (t => (t.SortKey.First ().ToString ().ToUpper ())))
					Lookup.Add (Tuple.Create (group.Key, group.OrderBy (t => (t.SortKey)).ToArray ()));
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
			var artists = Lookup [indexPath.Section].Item2;

			var cell = (ArtistCell) tableView.DequeueReusableCell ("ArtistCell");
			cell.Artist = artists [indexPath.Row];
			return cell;
		}

		public override string[] SectionIndexTitles(UITableView tableView)
		{
			return Lookup.Select (s => s.Item1).ToArray ();
		}

		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return Lookup [(int)section].Item1;
		}
	}
}