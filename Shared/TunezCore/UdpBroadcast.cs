using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;

namespace Tunez
{
	public class UdpBroadcast
	{
		static readonly IPEndPoint BroadcastEndpoint = new IPEndPoint (IPAddress.Parse ("239.0.0.222"), 19860);
		const string Header = "Tunez-";

		public event EventHandler<ServerDetails> ServerFound;

		public UdpBroadcast ()
		{
		}

		/// <summary>
		/// Sending an announcement every 3 seconds until the token is cancelled.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="token">Token.</param>
		public async Task BeginAnnouncingAsync (int port, CancellationToken token)
		{
			var data = Encoding.UTF8.GetBytes (Header + port);
			var client = new UdpClient (AddressFamily.InterNetwork);
			client.MulticastLoopback = true;
			client.JoinMulticastGroup (BroadcastEndpoint.Address);
			token.Register (() => client.Close ());

			try {
				while (true) {
					await client.SendAsync (data, data.Length, BroadcastEndpoint).ConfigureAwait (false);
					await Task.Delay (TimeSpan.FromSeconds (1), token).ConfigureAwait (false);
				}
			} catch (ObjectDisposedException) {
				token.ThrowIfCancellationRequested ();
				throw;
			}
		}

		public async Task BeginListeningAsync (CancellationToken token)
		{
			var client = new UdpClient (BroadcastEndpoint);
			client.JoinMulticastGroup (BroadcastEndpoint.Address);
			token.Register (() => client.Close ());

			while (true) {
				token.ThrowIfCancellationRequested ();
				try {
					var result = await client.ReceiveAsync ();
					var data = Encoding.UTF8.GetString (result.Buffer);
					if (data.StartsWith (Header, StringComparison.Ordinal)) {
						if (ServerFound != null) {
							var details = new ServerDetails {
								Hostname = result.RemoteEndPoint.Address.ToString (),
								Port = int.Parse (data.Substring (Header.Length))
							};
							LoggingService.LogInfo ("Found TunezServer at {0}", details.FullAddress);
							ServerFound (this, details);
						}
					}
				} catch (ObjectDisposedException) {
					token.ThrowIfCancellationRequested ();
					throw;
				} catch (SocketException) {
					token.ThrowIfCancellationRequested ();
					// Ignore this
				} catch (Exception ex) {
					token.ThrowIfCancellationRequested ();
					LoggingService.LogInfo ("Ignoring bad UDP {0}", ex);
				}
			}
		}
	}
}
