using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tunez
{
	public static class Tasks
	{
		public static Task CompletedTask {
			get { return Task.FromResult (true); }
		}

		public static void Post (this SynchronizationContext context, Action action)
		{
			context.Post (s => action (), null);
		}

		public static void Send (this SynchronizationContext context, Action action)
		{
			context.Send (d => action (), null);
		}

		public static async Task WaitOrCanceled (this Task task)
		{
			try {
				await task;
			} catch (OperationCanceledException) {
				// Ignore
			}
		}
	}
}

