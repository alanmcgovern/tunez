// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Foundation;
using UIKit;
using Tunez;

namespace iOSTunez
{
	public partial class SelectServerViewController : UIViewController
	{
		TaskCompletionSource<ServerDetails> serverSelected = new TaskCompletionSource<ServerDetails> ();
		CancellationTokenSource searchCancellation = new CancellationTokenSource ();
		List<ServerDetails> Servers;

		public Task<ServerDetails> ServerSelected {
			get { return serverSelected.Task; }
		}

		public SelectServerViewController (IntPtr handle)
			: base (handle)
		{
		}

		partial void manualAdd_TouchUpInside (UIButton sender)
		{
			Servers.Add (new ServerDetails {
				Hostname = manualHostname.Text,
				Port = int.Parse(manualPort.Text),
				ManuallyEntered = true
			});
			serverTableView.ReloadData ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			Servers = new List<ServerDetails> (Caches.LoadKnownServers ());
			serverTableView.DataSource = new SelectServerViewDataSource (Servers);
			serverTableView.ReloadData ();
			serverTableView.ServerSelected += t => {
				foreach (var v in Servers)
					v.Default = v == t;

				var previous = serverSelected;
				serverSelected = new TaskCompletionSource<ServerDetails> ();
				previous.TrySetResult (t);
			};

			searchCancellation = new CancellationTokenSource ();
			SearchForServers();
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			Caches.SaveKnownServers (Servers.ToArray ());
			searchCancellation.Cancel ();
		}

		async void SearchForServers ()
		{
			var announceListener = new UdpBroadcast ();
			announceListener.ServerFound += (o, e) => {
				if (!Servers.Contains (e)) {
					Servers.Add (e);
					serverTableView.ReloadData ();
				}
			};

			await announceListener.BeginListeningAsync(searchCancellation.Token).WaitOrCanceled();
		}

		class SelectServerViewDataSource : UITableViewDataSource
		{
			List<ServerDetails> Servers
			{
				get;
			}

			public SelectServerViewDataSource (List<ServerDetails> servers)
			{
				Servers = servers;
			}

			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				return Servers [indexPath.Row].ManuallyEntered;
			}

			public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					Servers.RemoveAt ((int) indexPath.Row);
					tableView.DeleteRows (new[] { indexPath }, UITableViewRowAnimation.Left);
				}
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableView, nint section)
			{
				return Servers.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = (ServerCell) tableView.DequeueReusableCell ("ServerCell");
				cell.Server = Servers [indexPath.Row];
				return cell;
			}
		}
	}
}