using Foundation;
using System;
using UIKit;
using Tunez;

namespace iOSTunez
{
	partial class ArtistCell : UITableViewCell
	{
		Artist artist;
		public Artist Artist {
			get { return artist; }
			set {
				artist = value;
				TextLabel.Text = artist.Name;
			}
		}

		public ArtistCell (IntPtr handle) : base (handle)
		{
		}
	}
}
