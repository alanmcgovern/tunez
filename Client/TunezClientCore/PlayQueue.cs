using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tunez
{
	public class PlayQueue<T>
		where T : IStreamingPlayer, IDisposable, new ()
	{
		public event Action BufferingStarted;
		public event Action BufferingCompleted;
		public event Action Paused;
		public event Action Playing;
		public event Action Stopped;

		CancellationTokenSource CancellationTokenSource {
			get; set;
		}

		IStreamingPlayer CurrentPlayer {
			get; set;
		}

		public bool IsPaused {
			get { return CurrentPlayer.Paused; }
			set {
				CurrentPlayer.Paused = value;
				if (value && Paused != null)
					Paused ();
				if (!value && Playing != null)
					Playing ();
			}
		}

		INetworkChangedListener NetworkChangedListener {
			get; set;
		}

		TrackMonitor Monitor {
			get; set;
		}

		TunezServer Server {
			get; set;
		}

		public PlayQueue (TunezServer server, INetworkChangedListener networkChangedListener, TrackMonitor monitor)
		{
			CurrentPlayer = new NullPlayer ();
			Server = server;
			NetworkChangedListener = networkChangedListener;
			Monitor = monitor;

			CancellationTokenSource = new CancellationTokenSource ();
		}

		public async Task PlayAsync(IEnumerable<Track> tracks)
		{
			CancellationTokenSource.Cancel();
			CancellationTokenSource = new CancellationTokenSource();

			IStreamingPlayer player = new T ();
			CurrentPlayer = player;
			player.BufferingStarted += RaiseBufferingStarted;
			player.BufferingCompleted += RaiseBufferingCompleted;
			try {
				var token = CancellationTokenSource.Token;
				using (player) {
					if (Playing != null)
						Playing ();

					foreach (var track in tracks) {
						Monitor.Initialize (track);
						using (var stream = new ReconnectingStream (Server.ServerDetails, NetworkChangedListener, track))
							await player.PlayAsync (stream, Monitor, token);
					}
				}
				Monitor.Initialize (null);
				CurrentPlayer = new NullPlayer ();
				if (Stopped != null)
					Stopped ();
			} catch (OperationCanceledException) {
				// We stopped it!
			} catch (Exception ex) {
				LoggingService.LogError(ex, "Unexpected exception playing a track");
			} finally {
				player.BufferingStarted -= RaiseBufferingStarted;
				player.BufferingCompleted -= RaiseBufferingCompleted;
			}
		}

		void RaiseBufferingStarted ()
		{
			if (BufferingStarted != null) {
				LoggingService.LogInfo ("PlayQueue.RaiseBufferingStarted");
				BufferingStarted ();
			}
		}

		void RaiseBufferingCompleted ()
		{
			if (BufferingCompleted != null) {
				LoggingService.LogInfo ("PlayQueue.RaiseBufferingCompleted");
				BufferingCompleted ();
			}
		}

		public void Dispose ()
		{
			CancellationTokenSource.Cancel ();
			NetworkChangedListener.Dispose ();
		}
	}
}

