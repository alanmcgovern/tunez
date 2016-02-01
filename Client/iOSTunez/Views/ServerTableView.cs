// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using Tunez;

namespace iOSTunez
{
	public partial class ServerTableView : UITableView
	{
		public event Action<ServerDetails> ServerSelected;

		public ServerTableView (IntPtr handle) : base (handle)
		{
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			Delegate = new ServerTableViewDelegate ();
		}

		class ServerTableViewDelegate : UITableViewDelegate
		{
			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = (ServerCell) tableView.CellAt (indexPath);
				if (((ServerTableView) tableView).ServerSelected != null)
					((ServerTableView) tableView).ServerSelected (cell.Server);
			}
		}
	}
}