using System;
using AppKit;
using Foundation;
using System.Collections.Generic;
using Tunez;

namespace MacTunez
{
	[Register ("CatalogBrowser")]
	public class CatalogBrowser : NSBrowser
	{
		public event EventHandler<IEnumerable<Track>> TracksQueued;

		protected CatalogBrowser (IntPtr handle)
			: base (handle)
		{
		}

		public override void DoDoubleClick (NSObject sender)
		{
			if (TracksQueued != null && SelectionIndexPath != null) {
				var selectedItem = ItemAtIndexPath (SelectionIndexPath);
				if (selectedItem is IEnumerable<Track>) {
					TracksQueued (this, (IEnumerable<Track>) selectedItem);
				}
			}
		}
	}
}

