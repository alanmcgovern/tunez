using System;
using AudioToolbox;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tunez;
using System.IO;

namespace Tunez
{
	/// <summary>
	/// A Class to hold the AudioBuffer with all setting together
	/// </summary>
	internal class AudioBuffer
	{
		public IntPtr Buffer { get; set; }

		public List<AudioStreamPacketDescription> PacketDescriptions { get; set; }

		public int CurrentOffset { get; set; }

		public bool IsInUse { get; set; }
	}

	/// <summary>
	/// Wrapper around OutputQueue and AudioFileStream to allow streaming of various filetypes
	/// </summary>
	public class StreamingPlayer : IStreamingPlayer, IDisposable
	{
		event EventHandler Finished;
		event Action<OutputAudioQueue> OutputReady;
		public event Action BufferingStarted = () => { };
		public event Action BufferingCompleted = () => { };
	
		readonly object locker = new object ();

		// the AudioToolbox decoder
		AudioFileStream fileStream;
		// The queue of buffers which are available to use
		Queue<AudioBuffer> availableBuffers;
		// All the buffers we created
		Dictionary<IntPtr, AudioBuffer> outputBuffers;

		public OutputAudioQueue outputQueue;
		CancellationTokenSource previousCancellation;
		Task previousPlayAsync;
		SynchronizationContext syncContext;


		bool buffering;
		public bool Buffering {
			get { return buffering; }
			set {
				if (buffering != value) {
					buffering = value;
					syncContext.Post (() => {
						if (value)
							BufferingStarted ();
						else
							BufferingCompleted ();
					});
				}
			}
		}

		bool paused;
		public bool Paused {
			get { return paused; }
			set {
				paused = value;
				if (paused)
					TryPause ();
				else
					TryResume ();
			}
		}

		float volume = 1;
		public float Volume {
			get { return volume; }
			set {
				volume = value;
				if (outputQueue != null)
					outputQueue.Volume = value;
			}
		}

		/// <summary>
		/// Defines the size for each buffer, when using a slow source use more buffers with lower buffersizes
		/// </summary>
		public int BufferSize {
			get; private set;
		}

		TaskCompletionSource<bool> needsBufferTask;
		Task<bool> NeedsBuffer (CancellationToken token)
		{
			lock (locker) {
				if (!needsBufferTask.Task.IsCompleted)
					token.Register (() => needsBufferTask.TrySetCanceled ());
				return needsBufferTask.Task;
			}
		}

		/// <summary>
		/// Defines the maximum Number of Buffers to use, the count can only change after Reset is called or the
		/// StreamingPlayback is freshly instantiated
		/// </summary>
		public int MaxBufferCount {
			get; set;
		}

		/// <summary>
		/// This is the minimum number of buffers we should have enqueued before playback starts.
		/// This is primarily used to automatically pause the player when we do not receive data
		/// fast enough to keep buffers queued up. Apple's APIs screw up if you let them 'play' but
		/// are not feeding them data. When you eventually start feeding in data it will keep skipping
		/// audio frames until it has caught up with where it should be. We don't want this behaviour!
		/// </summary>
		/// <value>The auto resume buffer level.</value>
		int AutoResumeBufferLevel {
			get; set;
		}

		public StreamingPlayer ()
		{
			syncContext = SynchronizationContext.Current;
			BufferSize = 64 * 1024;
			MaxBufferCount = 16;

			previousCancellation = new CancellationTokenSource ();
			previousPlayAsync = Tasks.CompletedTask;
		}

		void Reset ()
		{
			if (fileStream != null) {
				fileStream.Dispose ();
				fileStream = null;
			}

			if (outputQueue != null) {
				outputQueue.RemoveListener (AudioQueueProperty.IsRunning, EmitFinishedEvent);

				outputQueue.Stop (true);
				outputQueue.Reset ();
				foreach (AudioBuffer buf in outputBuffers.Values) {
					buf.PacketDescriptions.Clear ();
					outputQueue.FreeBuffer (buf.Buffer);
				}
				outputQueue.Dispose ();

				availableBuffers = null;
				outputBuffers = null;
				outputQueue = null;
			}
		}

		void TryPause ()
		{
			lock (locker) {
				if (outputQueue != null && (Paused || Buffering))
					outputQueue.Pause ();
			}
		}

		void TryResume ()
		{
			lock (locker) {
				if (outputQueue != null && !(Paused || Buffering))
					outputQueue.Start ();
			}
		}

		/// <summary>
		/// Begins playing the audio stroed in the supplied stream
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="stream">Stream.</param>
		/// <param name="token">Token.</param>
		public async Task PlayAsync (Stream stream, IProgress<TimeSpan> monitor, CancellationToken token)
		{
			// Cancel the previous 'PlayAsync' call and then set up the cancellation for this invocation
			var cancellation = CancellationTokenSource.CreateLinkedTokenSource (token);
			previousCancellation.Cancel ();
			previousCancellation = cancellation;

			await previousPlayAsync.WaitOrCanceled ();
			token.ThrowIfCancellationRequested ();

			Reset ();
			fileStream = new AudioFileStream (AudioFileType.MP3);
			fileStream.PacketDecoded += AudioPacketDecoded;
			fileStream.PropertyFound += AudioPropertyFound;

			previousPlayAsync = BeginPlayingAsync (stream, monitor, cancellation);
			await previousPlayAsync.WaitOrCanceled ();
			// If the original token is cancelled we should throw a cancelled exception.
			token.ThrowIfCancellationRequested ();
		}

		async Task BeginPlayingAsync (Stream stream, IProgress<TimeSpan> monitor, CancellationTokenSource cts)
		{
			AutoResumeBufferLevel = 2;
			Buffering = true;
			Paused = false;

			needsBufferTask = new TaskCompletionSource<bool> ();
			needsBufferTask.SetResult (true);

			var finishedTask = WaitForFinished (cts.Token);
			var reportProgressTask = ReportProgressAsync (monitor, cts.Token);

			var buffer = new byte [BufferSize];
			try {
				while (await NeedsBuffer (cts.Token).ConfigureAwait (false)) {
					var read = await stream.ReadAsync (buffer, 0, buffer.Length, cts.Token).ConfigureAwait (false);
					var lastPacket = read == 0 || (stream.CanSeek && stream.Position == stream.Length);
					CheckStatus (fileStream.ParseBytes (buffer, 0, read, read == 0));
					if (lastPacket) {
						// Do not autopause anymore, we won't be queueing any more buffers.
						AutoResumeBufferLevel = 0;
						EnqueueBuffer ();
						outputQueue.Stop (false);
						break;
					}
				}
			} catch (OperationCanceledException) {
				// If 'NeedsBuffer' is cancelled we should still `await` for the progress reporter to 
				// be cancelled too.
			}
			await finishedTask.WaitOrCanceled ();
			cts.Cancel ();
			await reportProgressTask.WaitOrCanceled ();
		}

		void CheckStatus (AudioFileStreamStatus status)
		{
			if (status != AudioFileStreamStatus.Ok)
				LoggingService.LogError (null, "Buggy status: {0}", status);
		}

		async Task WaitForOutputReady (CancellationToken token)
		{
			var finishedTask = new TaskCompletionSource<bool> ();
			using (token.Register (() => finishedTask.TrySetCanceled ())) {
				Action<OutputAudioQueue> finishedHandler = (e) => finishedTask.TrySetResult (true);
				try {
					OutputReady += finishedHandler;
					await finishedTask.Task;
				} finally {
					OutputReady -= finishedHandler;
				}
			}
		}

		async Task WaitForFinished (CancellationToken token)
		{
			var finishedTask = new TaskCompletionSource<bool> ();
			using (token.Register (() => finishedTask.TrySetCanceled ())) {
				EventHandler finishedHandler = (sender, e) => finishedTask.TrySetResult (true);
				try {
					Finished += finishedHandler;
					await finishedTask.Task;
				} finally {
					Finished -= finishedHandler;
				}
			}
		}

		async Task ReportProgressAsync (IProgress<TimeSpan> monitor, CancellationToken token)
		{
			bool discontinuity = false;
			AudioTimeStamp currentTime = default (AudioTimeStamp);

			try {
				await WaitForOutputReady (token).ConfigureAwait (false);
				while (true) {
					using (var timeline = outputQueue.CreateTimeline ())
						outputQueue.GetCurrentTime (timeline, ref currentTime, ref discontinuity);
					monitor.Report (TimeSpan.FromSeconds (currentTime.SampleTime / (float)outputQueue.SampleRate));
					await Task.Delay (TimeSpan.FromSeconds (1), token);
				}
			} catch (OperationCanceledException) {

			} catch (Exception ex) {
				LoggingService.LogError (ex, "Could not report progress");
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		/// <summary>
		/// Cleaning up all the native Resource
		/// </summary>
		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				previousCancellation.Cancel ();
				previousPlayAsync.WaitOrCanceled ().Wait ();

				Reset ();
				using (fileStream)
					fileStream = null;
				availableBuffers = null;
				outputBuffers = null;
			}
		}

		/// <summary>
		/// Saving the decoded Packets to our active Buffer, if the Buffer is full queue it into the OutputQueue
		/// and wait until another buffer gets freed up
		/// </summary>
		void AudioPacketDecoded (object sender, PacketReceivedEventArgs args)
		{
			lock (locker) {
				foreach (var p in args.PacketDescriptions) {
					AudioStreamPacketDescription pd = p;

					var currentBuffer = GetOrCreateAudioBuffer ();
					int left = BufferSize - currentBuffer.CurrentOffset;
					if (left < pd.DataByteSize) {
						EnqueueBuffer ();
						currentBuffer = GetOrCreateAudioBuffer ();
					}

					AudioQueue.FillAudioData (currentBuffer.Buffer, currentBuffer.CurrentOffset, args.InputData, (int)pd.StartOffset, pd.DataByteSize);
					// Set new offset for this packet
					pd.StartOffset = currentBuffer.CurrentOffset;
					// Add the packet to our Buffer
					currentBuffer.PacketDescriptions.Add (pd);
					// Add the Size so that we know how much is in the buffer
					currentBuffer.CurrentOffset += pd.DataByteSize;
				}
			}
		}

		/// <summary>
		/// Enqueue the active buffer to the OutputQueue
		/// </summary>
		void EnqueueBuffer ()
		{
			lock (locker) {
				var currentBuffer = availableBuffers.Dequeue ();
				outputQueue.EnqueueBuffer (currentBuffer.Buffer, currentBuffer.CurrentOffset, currentBuffer.PacketDescriptions.ToArray ());

				if (outputBuffers.Count - availableBuffers.Count > MaxBufferCount)
					needsBufferTask = new TaskCompletionSource<bool> ();

				if ((outputBuffers.Count - availableBuffers.Count) > AutoResumeBufferLevel) {
					Buffering = false;
					TryResume ();
				}
			}
		}

		/// <summary>
		/// When a AudioProperty in the fed packets is found this callback is called
		/// </summary>
		void AudioPropertyFound (object sender, PropertyFoundEventArgs args)
		{
			lock (locker) {
				if (args.Property == AudioFileStreamProperty.ReadyToProducePackets) {
					if (outputQueue != null)
						outputQueue.Dispose ();

					availableBuffers = new Queue<AudioBuffer> ();
					outputBuffers = new Dictionary<IntPtr, AudioBuffer> ();
					outputQueue = new OutputAudioQueue (fileStream.StreamBasicDescription);
					outputQueue.AddListener (AudioQueueProperty.IsRunning, EmitFinishedEvent);
					outputQueue.Volume = Volume;
					outputQueue.AddListener (AudioQueueProperty.ConverterError, (AudioQueueProperty property) => {
						LoggingService.LogInfo ("Got an error reading the file: {0}", outputQueue.ConverterError);
					});
					if (OutputReady != null)
						OutputReady (outputQueue);

					outputQueue.BufferCompleted += HandleBufferCompleted;
					outputQueue.MagicCookie = fileStream.MagicCookie;
				}
			}
		}

		void EmitFinishedEvent (AudioQueueProperty property)
		{
			var finished = Finished;
			if (finished != null) {
				if (!Paused && !outputQueue.IsRunning) {
					try {
						finished (this, EventArgs.Empty);
					} catch (Exception ex) {
						LoggingService.LogError (ex, "Unhandled exception emitting the Finished event");
					}
				}
			}
		}

		AudioBuffer GetOrCreateAudioBuffer ()
		{
			if (availableBuffers.Count == 0) {
				IntPtr outBuffer;
				outputQueue.AllocateBuffer (BufferSize, out outBuffer);
				var buffer = new AudioBuffer {
					Buffer = outBuffer,
					PacketDescriptions = new List<AudioStreamPacketDescription> ()
				};
				outputBuffers.Add (outBuffer, buffer);
				availableBuffers.Enqueue (buffer);
			}

			return availableBuffers.Peek ();
		}

		/// <summary>
		/// Is called when a buffer is completly read and can be queued for re-use
		/// </summary>
		void HandleBufferCompleted (object sender, BufferCompletedEventArgs e)
		{
			lock (locker) {
				var buffer = outputBuffers [e.IntPtrBuffer];
				buffer.PacketDescriptions.Clear ();
				buffer.CurrentOffset = 0;
				availableBuffers.Enqueue (buffer);

				// Signal that we should try to fill up some more buffers with audio
				// data. NOTE: When we parse a blob of data we will always create enough
				// buffers to fit the decoded data. That could make us create more buffers
				// than 'MaxBufferCount', so let's be careful to ensure we only start reading
				// more input data once enough buffers have been freed up.
				if (outputBuffers.Count - availableBuffers.Count < MaxBufferCount)
					needsBufferTask.TrySetResult (true);

				// If autoresume buffer level is zero then we're at the end of the stream and should
				// not pause the output because the number of enqueued buffers is low.
				if ((outputBuffers.Count - availableBuffers.Count) == 1 && AutoResumeBufferLevel > 0) {
					Buffering = true;
					TryPause ();
				}
			}
		}
	}
}