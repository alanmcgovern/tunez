using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Tunez
{
	class MainClass
	{
		static UdpBroadcast announcer;
		static RequestHandler handler;

		public static void Main (string[] args)
		{
			var cts = new CancellationTokenSource ();
			var context = new ConsoleSyncContext ();
			Task.Run (() => context.Run ());
			Task initTask = null;

			context.Send (() => initTask = Initialize (cts.Token));
			while (true) {
				PrintCommands ();
				var command = Console.ReadLine ();
				if (command == null)
					continue;
				Console.Clear ();
				if (command == "q") {
					cts.Cancel ();
					initTask.WaitOrCanceled ().Wait ();
					context.Exit ();
					Environment.Exit (0);
				} else {
					Task handleTask = null;
					context.Send (t => handleTask = HandleKeyPress (command, cts.Token), null);
					handleTask.WaitOrCanceled ().Wait ();
				}
			}
		}

		static async Task Initialize (CancellationToken token)
		{
			var catalog = await LoadCatalog (token);

			handler = new RequestHandler {
				Catalog = catalog
			};
			announcer = new UdpBroadcast ();

			try {
				await Task.WhenAll (new [] {
					handler.BeginListeningAsync (token),
					announcer.BeginAnnouncingAsync (handler.ListenPort, token)
				});
			} catch (OperationCanceledException) {

			}
		}

		static async Task HandleKeyPress (string command, CancellationToken token)
		{
			if (command.StartsWith ("add", StringComparison.Ordinal)) {
				var paths = Caches.GetTrackLoaderPaths ();
				if (paths.Add (command.Substring ("add ".Length))) {
					Caches.StoreTrackLoaderPaths (paths);
					handler.Catalog = await LoadCatalog (token);
				}
			} else if (command.StartsWith ("remove", StringComparison.Ordinal)) {
				var paths = Caches.GetTrackLoaderPaths ();
				if (paths.Remove (command.Substring ("remove ".Length))) {
					Caches.StoreTrackLoaderPaths (paths);
					handler.Catalog = await LoadCatalog (token);
				}
			} else if (command.StartsWith ("scan", StringComparison.Ordinal)) {
				Caches.DeleteCachedCatalog ();
				handler.Catalog = await LoadCatalog (token);
			}
		}

		static async Task<Catalog> LoadCatalog (CancellationToken token)
		{
			var catalog = Caches.GetCachedCatalog ();
			if (catalog != null)
				return catalog;

			var paths = Caches.GetTrackLoaderPaths ();
			if (!paths.Any ()) {
				LoggingService.LogInfo ("You need to add some search paths to load MP3s from");
				return new Catalog ();
			}

			LoggingService.LogInfo ("Preparing the music catalog...");
			LoggingService.LogInfo ("Loading music from:");
			foreach (var path in paths)
				LoggingService.LogInfo ("\t{0}", path);

			var loader = new Progress<float> ();
			loader.ProgressChanged += (sender, e) => LoggingService.LogInfo ("Progress: {0:P}", e);
			var tracks = await Task.Run (() => (TrackLoader.Load (paths, loader, token)));
			catalog = new Catalog (tracks);
			Caches.StoreCachedCatalog (catalog);
			return catalog;
		}

		static void PrintCommands ()
		{
			Console.Write (@"
Type 'scan' to regenerate the catalog by scanning the disk.
Type 'add <path>' to recursively scan for mp3s to add to the catalog.
Type 'remove <path>' to remove a path from the catalog.
Press 'q' to quit.
$ ");
		}
	}
}
