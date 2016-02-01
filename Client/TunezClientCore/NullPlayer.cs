using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tunez
{
	public class NullPlayer : IStreamingPlayer
	{
		#pragma warning disable 0067
		public event Action BufferingStarted;
		public event Action BufferingCompleted;
		#pragma warning restore 0067

		public bool Paused {
			get; set;
		}

		public Task PlayAsync (Stream stream, IProgress<TimeSpan> monitor, CancellationToken token)
		{
			return Tasks.CompletedTask;
		}


		public void Dispose ()
		{
		}
	}
}

