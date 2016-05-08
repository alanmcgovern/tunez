using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AppKit;
using Foundation;

using Tunez;

namespace MacTunez
{
	public class MacStreamingPlayer : StreamingPlayer
	{
		public MacStreamingPlayer ()
		{
			Volume = 0.1f;
		}
	}

	public partial class ViewController : NSViewController
	{
		const int FileMenuItemTag = 1;
		const int ServerMenuItemTag = 2;

		PlayQueue<MacStreamingPlayer> playQueue;

		string CacheDirectory {
			get { return Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Library", "Caches", "tunez"); }
		}

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			PrepareServerWindow ();

			this.CatalogBrowser.TracksQueued += async (sender, tracks) => {
				try {
					await playQueue.PlayAsync (tracks);
				} catch (OperationCanceledException) {
					// This is ok, let's ignore it!
				} catch (Exception ex) {
					LoggingService.LogError (ex, "An unhandled exception happened playing a song");
				}
			};

			var serverDetails = Caches.LoadKnownServers ().FirstOrDefault (t => t.Default);
			var server = new TunezServer (serverDetails);
			var catalog = await server.FetchCatalog (Path.Combine (CacheDirectory, "tunez.catalog"), System.Threading.CancellationToken.None);

			// Urgh, figure out proper initialization order to avoid IndexOutOfRangeExceptions accessing the toolbar items
			// There's zero elements if i do this too quickly
			await Task.Delay (100);
			var playPauseItem = (NSToolbarItem) NSApplication.SharedApplication.Windows[0].Toolbar.Items[0];
			var playPauseButton = (NSButton) NSApplication.SharedApplication.Windows[0].Toolbar.Items[0].View;
			var progress = NSApplication.SharedApplication.Windows[0].Toolbar.Items[1];
			var label = (NSTextField)NSApplication.SharedApplication.Windows[0].Toolbar.Items[2].View;
			var monitors = new TrackMonitorGroup {
				new PlayingProgressMonitor (progress, label),
			};
			if (!string.IsNullOrEmpty (LastFMAppCredentials.APIKey))
				monitors.Add (new ScrobblingMonitor (new Scrobbler (Caches.Scrobbler)));

			playQueue = new PlayQueue<MacStreamingPlayer> (server, new NullNetworkChangedListener (), monitors);

			PlayPauseButton (playPauseItem, playPauseButton);
			PopulateBrowser (catalog);
			LoggingService.LogInfo ("Initialization complete...");
		}

		void PlayPauseButton (NSToolbarItem playPauseItem, NSButton playPauseButton)
		{
			const string resume = "Resume";
			const string pause = "Pause";

			playQueue.Paused += () => playPauseItem.Label = resume;
			playQueue.Stopped += () => playPauseItem.Label = pause;
			playQueue.Playing += () => playPauseItem.Label = pause;

			playQueue.Playing += () => playPauseItem.Enabled = true;
			playQueue.Stopped += () => playPauseButton.Enabled = false;

			playPauseItem.Label = pause;
			playPauseButton.Activated += (o, e) => {
				playQueue.IsPaused = !playQueue.IsPaused;
			};
		}

		void PopulateBrowser (Catalog catalog)
		{
			CatalogBrowser.Delegate = new CatalogBrowserDelegate (catalog);
			CatalogBrowser.ReloadColumn (0);
		}

		void PrepareServerWindow ()
		{
			var serverItem = NSApplication.SharedApplication
			                              .Menu
			                              .ItemWithTag (FileMenuItemTag)
			                              .Submenu
			                              .ItemWithTag (ServerMenuItemTag);

			serverItem.Activated += (sender, e) => {
				var mainStoryboard = NSStoryboard.FromName ("Main", null);
				var window = (NSWindowController) mainStoryboard.InstantiateControllerWithIdentifier ("SelectServerWindow");
				window.Window.ParentWindow = NSApplication.SharedApplication.MainWindow;
				window.ShowWindow (this);
			};
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
			}
		}
	}
}
