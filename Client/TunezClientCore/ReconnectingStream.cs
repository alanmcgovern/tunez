using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tunez
{
	public class ReconnectingStream : Stream
	{
		CancellationTokenSource cts = new CancellationTokenSource ();

		IConnection Connection {
			get;
		}

		Track Track {
			get;
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanWrite {
			get { return false; }
		}

		public override bool CanSeek {
			get { return true; }
		}

		public override bool CanTimeout {
			get { return false; }
		}

		public override long Length {
			get { return Track.FileSize; }
		}

		long position;
		public override long Position
		{
			get { return position; }
			set {
				position = value;
				cts.Cancel ();
			}
		}

		INetworkChangedListener NetworkChangedListener {
			get; set;
		}

		Stream RemoteStream {
			get; set;
		}

		public ReconnectingStream (IConnection connection, INetworkChangedListener networkChangedListener, Track track)
		{
			Connection = connection;
			NetworkChangedListener = networkChangedListener;
			Track = track;

			NetworkChangedListener.NetworkChanged += () => {
				LoggingService.LogInfo ("The network route has changed, reconnecting...");
				cts.Cancel ();
			};
		}

		public override void Flush()
		{
			throw new NotSupportedException ();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException ();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException ();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		public override async Task<int> ReadAsync (byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			while (position < Length) {
				try {
					using (cancellationToken.Register (cts.Cancel)) {
						if (RemoteStream == null)
							RemoteStream = await FetchRemoteStream (cts.Token);
						var read = await RemoteStream.ReadAsync (buffer, offset, count, cts.Token);
						position += read;
						return read;
					}
				}
				catch {
					CleanUpPreviousRequest ();
					await Task.Delay (1000, cancellationToken);
				}
			}
			// We've reached the end of the stream now.
			return 0;
		}

		async Task<Stream> FetchRemoteStream (CancellationToken cancellationToken)
		{
			var fetchTrackMessage = new FetchTrackMessage { UUID = Track.UUID, Offset = Position };

			LoggingService.LogInfo ("Preparing to fetch remote stream...");
			var stream = RemoteStream = await Connection.GetStreamingResponse (Messages.FetchTrack, Newtonsoft.Json.JsonConvert.SerializeObject (fetchTrackMessage));
			LoggingService.LogInfo ("Retrieved the remote stream");
			return stream;
		}

		void CleanUpPreviousRequest ()
		{
			if (RemoteStream != null) {
				RemoteStream.Dispose ();
				RemoteStream = null;
			}

			if (cts.IsCancellationRequested)
				cts = new CancellationTokenSource ();
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				CleanUpPreviousRequest ();
		}
	}
}

