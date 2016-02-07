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

		void BufferingCompleted ()
		{
			if (backgroundTask.HasValue) {
				LoggingService.LogInfo ("Ended background task '{0}'", backgroundTask.Value);
				UIApplication.SharedApplication.EndBackgroundTask (backgroundTask.Value);
				backgroundTask = null;
			}
		}
	}
}

