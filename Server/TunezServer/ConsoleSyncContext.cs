using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Tunez
{
	class ConsoleSyncContext : SynchronizationContext
	{
		BlockingCollection<Action> queue = new BlockingCollection<Action> ();

		public override void Post (SendOrPostCallback d, object state)
		{
			queue.Add (() => d (state));
		}

		public void Send (Action action)
		{
			Send (d => action (), null);
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			Exception ex = null;
			using (var waiter = new ManualResetEventSlim (false)) {
				Post (wrapper => {
					try {
						d.Invoke (state);
					} catch (Exception e) {
						ex = e;
					} finally {
						waiter.Set ();
					}
				}, null);
				waiter.Wait ();
			}

			if (ex != null)
				throw ex;
		}

		public void Run ()
		{
			SetSynchronizationContext (this);

			Action item;
			while (true) {
				try {
					item = queue.Take ();
				} catch (InvalidOperationException) {
					// The collection has been closed, so let's exit!
					return;
				}

				item ();
			}
		}

		public void Exit ()
		{
			queue.CompleteAdding ();
		}
	}
}
