using System;
using System.Threading;
using System.Threading.Tasks;
using Tunez;

using CoreGraphics;
using UIKit;

namespace iOSTunez
{
	public class BufferingManager
	{
		nint? backgroundTask;

		PlayQueue<StreamingPlayer> PlayQueue {
			get; set;
		}

		CancellationToken Token {
			get; set;
		}

		public BufferingManager (PlayQueue<StreamingPlayer> playQueue, CancellationToken token)
		{
			playQueue.BufferingStarted += BufferingStarted;
			playQueue.BufferingCompleted += BufferingCompleted;
			Token = token;
			Token.Register (() => {
				playQueue.BufferingStarted -= BufferingStarted;
				playQueue.BufferingCompleted -= BufferingCompleted;
				BufferingCompleted ();
			});
		}

		void BufferingStarted ()
		{
			nint task = 0;
			task = UIApplication.SharedApplication.BeginBackgroundTask (() => {
				LoggingService.LogInfo ("Background task '{0}' expired", task);
				if (task == backgroundTask.GetValueOrDefault ())
					backgroundTask = null;
			});
			LoggingService.LogInfo ("Started background task '{0}'", task);
			backgroundTask = task;
		}

		async void BufferingCompleted ()
		{
			if (backgroundTask.HasValue) {
				var task = backgroundTask.Value;
				backgroundTask = null;

				await Task.Delay (TimeSpan.FromSeconds (10));
				LoggingService.LogInfo ("Ended background task '{0}'", task);
				UIApplication.SharedApplication.EndBackgroundTask (task);
			}
		}
	}
}

