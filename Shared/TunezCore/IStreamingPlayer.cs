using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tunez
{
	public interface IStreamingPlayer : IDisposable
	{
		event Action BufferingStarted;
		event Action BufferingCompleted;

		bool Paused { get; set; }
		Task PlayAsync (Stream stream, IProgress<TimeSpan> monitor, CancellationToken token);
	}
}

