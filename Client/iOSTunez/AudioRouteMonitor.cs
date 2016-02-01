using System;
using System.Threading;
using System.Threading.Tasks;

using AVFoundation;
using Tunez;

namespace iOSTunez
{
	public class AudioRouteMonitor : IDisposable
	{
		TaskCompletionSource<bool> tcs;

		public Task AudioRouteChanged {
			get { return tcs.Task; }
		}

		IDisposable Observer {
			get;
		}

		public AudioRouteMonitor()
		{
			tcs = new TaskCompletionSource<bool> ();
			var context = SynchronizationContext.Current;
			Observer = AVAudioSession.Notifications.ObserveRouteChange ((sender, e) => {
				if (e.Reason == AVAudioSessionRouteChangeReason.OldDeviceUnavailable) {
					if (e.PreviousRoute.Description.Contains ("Headphones")) {
						context.Post (() => {
							var previous = tcs;
							tcs = new TaskCompletionSource<bool> ();
							previous.TrySetResult (true);
						});
					}
				}
			});
		}

		void IDisposable.Dispose ()
		{
			Observer?.Dispose ();
			tcs.TrySetCanceled ();
		}
	}
}

