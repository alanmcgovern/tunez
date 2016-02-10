using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

 namespace TunezServer
{
	public static class Asserts
	{
		public static Task IsCancelled(Task task)
		{
			return IsCancelled (task, TimeSpan.FromMinutes (1));
		}

		public static async Task IsCancelled(Task task, TimeSpan timeout)
		{
			var tcs = new TaskCompletionSource<bool>();
			var cts = new CancellationTokenSource(timeout);
			cts.Token.Register(() => tcs.TrySetResult(false));
			await task.ContinueWith(t => tcs.TrySetResult(t.IsCanceled));
			if (!await tcs.Task)
				Assert.Fail ("The task was not cancelled");
		}
	}
}

