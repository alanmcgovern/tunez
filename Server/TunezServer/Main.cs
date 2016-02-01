using System;
using System.Threading.Tasks;
using System.Threading;
using Tunez;

namespace Tunez
{
	class MainClass
	{
		static UdpBroadcast announcer;
		static Catalog catalog;
		static RequestHandler handler;

		public static void Main (string[] args)
		{
			var cts = new CancellationTokenSource ();
			var context = new ConsoleSyncContext ();
			Task.Run (() => context.Run ());
			Task initTask = null;

			context.Post (() => initTask = Initialize (cts.Token));
			while (true) {
				PrintCommands ();
				var command = Console.ReadLine ();
				Console.Clear ();
				if (command == "q") {
					cts.Cancel ();
					initTask.WaitOrCanceled ().Wait ();
					context.Exit ();
					Environment.Exit (0);
				} else {
					context.Send (t => HandleKeyPress (command), null);
				}
			}
		}

		static async Task Initialize (CancellationToken token)
		{
			Progress<float> loader = new Progress<float> ();
			loader.ProgressChanged += (sender, e) => LoggingService.LogInfo ("Progress: {0:P}", e);
			LoggingService.LogInfo ("Preparing the music catalog...");
			await Task.Run (() => catalog = new Catalog (TrackLoader.Load ("/Users/alan/MusicStream", loader, token)));

			handler = new RequestHandler ();
			announcer = new UdpBroadcast ();

			try {
				await Task.WhenAll (new [] {
					handler.BeginListeningAsync (catalog, token),
					announcer.BeginAnnouncingAsync (handler.ListenPort, token)
				});
			} catch (OperationCanceledException) {

			}
		}

		static void HandleKeyPress (string command)
		{
		}

		static void PrintCommands ()
		{
			Console.Write (@"
Press 'q' to quit.
$ ");
		}
	}
}
