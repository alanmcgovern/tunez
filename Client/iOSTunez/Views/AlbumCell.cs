using System;
using UIKit;
using Tunez;

namespace iOSTunez
{
	partial class AlbumCell : UITableViewCell
	{
		Album album;
		public Album Album {
			get { return album; }
			set {
				album = value;
				TextLabel.Text = album.Name;
			}
		}

		public AlbumCell (IntPtr handle) : base (handle)
		{
		}
	}
}
