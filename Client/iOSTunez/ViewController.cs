using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AVFoundation;
using UIKit;

using Tunez;

namespace iOSTunez
{
	public partial class ViewController : UIViewController
	{
		BufferingManager BufferingManager {
			get; set;
		}

		CatalogController CatalogController {
			get; set;
		}

		ContentViewController ContentViewController {
			get; set;
		}

		TrackMonitorGroup Monitors {
			get; set;
		}

		NowPlayingStatusView NowPlayingStatusView {
			get; set;
		}

		Scrobbler Scrobbler {
			get; set;
		}

		SidebarNavigation.SidebarController SidebarController {
			get; set;
		}

		public TunezServer Server {
			get; set;
		}

		protected ViewController(IntPtr handle) : base(handle)
		{

		}

		public override void ViewDidLoad()
		{
			var menuController = StoryboardHelper.Main.CreateNavigationMenuController ();
			ContentViewController = new ContentViewController ();
			SidebarController = new SidebarNavigation.SidebarController (this, ContentViewController, menuController) {
				MenuLocation = SidebarNavigation.MenuLocations.Left
			};

			NowPlayingStatusView = NowPlayingStatusView.Create ();

			Monitors = new TrackMonitorGroup {
				new NowPlayingMonitor (View, SidebarController.View, NowPlayingStatusView),
			};
			if (!string.IsNullOrEmpty (LastFMAppCredentials.APIKey)) {
				Scrobbler = new Scrobbler (Caches.Scrobbler);
				Monitors.Add (new ScrobblingMonitor (Scrobbler));
			}

			NowPlayingStatusView.PlayPausePressed += (o, e) => CatalogController.PlayQueue.IsPaused = !CatalogController.PlayQueue.IsPaused;
			menuController.CatalogSelected += (o, e) => {
				SidebarController.CloseMenu (true);
				ContentViewController.Content = CatalogController;
			};
			if (Scrobbler == null) {
				menuController.LastFMSupported = false;
			} else {
				menuController.LastFMSupported = true;
				menuController.LastFmSelected += (o, e) => {
					SidebarController.CloseMenu (true);
					ContentViewController.Content = StoryboardHelper.Main.CreateLastFMLoginController (Scrobbler);
				};
			}
			menuController.SelectServerSelected += (o, e) => {
				SidebarController.CloseMenu (true);
				ChangeServerAsync ();
			};

			MonitorAudioRouteChangedAsync ();

			var server = Caches.LoadKnownServers ().FirstOrDefault (t => t.Default);
			if (server != null)
				ConnectToServer (server);
			else
				ChangeServerAsync ();
		}

		async void MonitorAudioRouteChangedAsync ()
		{
			using (var monitor = new AudioRouteMonitor ()) {
				try {
					while (true) {
						await monitor.AudioRouteChanged;
						if (CatalogController != null)
							CatalogController.PlayQueue.IsPaused = true;
					}
				} catch (OperationCanceledException) {
					
				}
			}
		}

		async void ChangeServerAsync ()
		{
			var vc = StoryboardHelper.Main.CreateSelectServerViewController ();
			ContentViewController.Content = vc;
			ConnectToServer (await vc.ServerSelected);
		}

		CancellationTokenSource connectingCancellation = new CancellationTokenSource ();
		async void ConnectToServer (ServerDetails serverDetails)
		{
			connectingCancellation.Cancel ();
			connectingCancellation = new CancellationTokenSource ();

			try {
				var token = connectingCancellation.Token;

				Server = new TunezServer (new AppleConnection (serverDetails));
				var connectingViewController = StoryboardHelper.Main.CreateConnectingViewController ();
				connectingViewController.IsConnecting = true;
				connectingViewController.LoadingMessage = string.Format ("Fetching catalog from {0}...", serverDetails.FullAddress);
				ContentViewController.Content = connectingViewController;

				var newCatalog = await Server.FetchCatalog (Caches.TunezCatalog, token);
				var playQueue = new PlayQueue<StreamingPlayer> (Server, new NetworkChangedListener (), Monitors);
				playQueue.Paused += () => NowPlayingStatusView.IsPaused = true;
				playQueue.Playing += () => NowPlayingStatusView.IsPaused = false;
				playQueue.BufferingStarted += () => NowPlayingStatusView.IsBuffering = true;
				playQueue.BufferingCompleted += () => NowPlayingStatusView.IsBuffering = false;

				BufferingManager = new BufferingManager (playQueue, token);
				Monitors.Initialize (null);

				if (CatalogController != null)
					CatalogController.PlayQueue.Dispose ();
				CatalogController = StoryboardHelper.Main.CreateCatalogController (newCatalog, playQueue);

				connectingViewController.IsConnecting = false;
				ContentViewController.Content = CatalogController;
			} catch (OperationCanceledException) {

			}
		}
	}
}

