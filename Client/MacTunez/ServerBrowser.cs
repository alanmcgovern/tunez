using System;
using AppKit;
using Foundation;
using System.Collections.Generic;
using Tunez;

namespace MacTunez
{
	[Register ("ServerBrowser")]
	public class ServerBrowser : NSBrowser
	{
		public event EventHandler<ServerDetails> ServerSelected;

		protected ServerBrowser (IntPtr handle)
			: base (handle)
		{
			Delegate = new ServerBrowserDelegate ();
		}

		public override void DoDoubleClick (NSObject sender)
		{
			if (ServerSelected != null && SelectionIndexPath != null) {
				var selectedItem = ItemAtIndexPath (SelectionIndexPath);
				if (selectedItem is NSServerItem) {
					ServerSelected (this, ((NSServerItem) selectedItem).Server);
				}
			}
			Window.Close ();
		}
	}
}
