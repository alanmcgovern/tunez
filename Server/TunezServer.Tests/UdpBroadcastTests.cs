using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

using Tunez;

 namespace TunezServer
{
	[TestFixture]
	public class UdpBroadcastTests
	{
		[Test]
		public async Task Announce_CancelAfter ()
		{
			var cts = new CancellationTokenSource();
			var announcer = new UdpBroadcast();
			var task = announcer.BeginAnnouncingAsync (12345, cts.Token);
			cts.Cancel();
			await Asserts.IsCancelled(task);
		}

		[Test]
		public async Task Announce_CancelBefore ()
		{
			var cts = new CancellationTokenSource();
			var announcer = new UdpBroadcast();
			cts.Cancel();
			var task = announcer.BeginAnnouncingAsync (12345, cts.Token);
			await Asserts.IsCancelled(task);
		}

		[Test]
		public async Task Listen_CancelAfter ()
		{
			var cts = new CancellationTokenSource();
			var announcer = new UdpBroadcast();
			var task = announcer.BeginListeningAsync(cts.Token);
			cts.Cancel();
			await Asserts.IsCancelled(task);
		}


		[Test]
		public async Task Listen_CancelBefore ()
		{
			var cts = new CancellationTokenSource();
			var announcer = new UdpBroadcast();
			cts.Cancel();
			var task = announcer.BeginListeningAsync(cts.Token);
			await Asserts.IsCancelled(task);
		}
	}
}

