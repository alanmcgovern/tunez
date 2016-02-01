using System;
using Tunez;
using UIKit;

namespace iOSTunez
{
	public abstract class TableViewControllerWithPlayQueue : UITableViewController
	{
		public PlayQueue<StreamingPlayer> PlayQueue {
			get { return ((CatalogController) NavigationController).PlayQueue; }
		}

		public new UITableView View {
			get { return (UITableView) base.View; }
		}

		protected TableViewControllerWithPlayQueue (IntPtr ptr)
			: base (ptr)
		{
		}
	}
}

