using System;
using Tunez;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace Tunez
{
	public class RequestHandler
	{
		public Catalog Catalog {
			get; set;
		}

		public int ListenPort {
			get { return 51986; }
		}

		public async Task BeginListeningAsync (CancellationToken token)
		{
			var tcpListener = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var tcp6Listener = new Socket (AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

			using (tcpListener)
			using (tcp6Listener) {
				tcpListener.Bind (new IPEndPoint (IPAddress.Any, ListenPort));
				tcp6Listener.Bind (new IPEndPoint (IPAddress.IPv6Any, ListenPort));

				await Task.WhenAll (
					BeginListeningAsync (tcp6Listener, token),
					BeginListeningAsync (tcpListener, token),
				).ConfigureAwait (false);
			}
		}

		async Task BeginListeningAsync (Socket listener, CancellationToken token)
		{
			try {
				listener.Listen (10);
				using (token.Register (() => listener.Dispose ())) {
					while (!token.IsCancellationRequested) {
						var socket = await Task.Factory.FromAsync (listener.BeginAccept (null, null), listener.EndAccept).ConfigureAwait (false);
						ContextReceived (Catalog, socket, token);
					}
				}
			} catch (ObjectDisposedException ex) {
				if (!token.IsCancellationRequested) {
					LoggingService.LogError (ex, "Unexpected ObjectDisposedException in BeginListeningAsync");
					throw;
				}
			} catch (OperationCanceledException) {
				// We cancelled successfully
			} catch (Exception ex) {
				LoggingService.LogError (ex, string.Format ("Unhandled exception in BeginListeningAsync. {0}", listener.AddressFamily));
			}
		}

		static async void ContextReceived (Catalog catalog, Socket socket, CancellationToken token)
		{
			try {
				LoggingService.LogInfo ("Received a new request");
				using (socket)
					await SendResponse (catalog, socket, token).ConfigureAwait (false);
			} catch (IOException) {
				LoggingService.LogInfo ("Client probably disconnected.");
			} catch (Exception ex) {
				socket.Close ();
				LoggingService.LogError (ex, "The connection died. Oops!");
			}
		}

		static async Task SendResponse (Catalog catalog, Socket socket, CancellationToken token)
		{
			try {
				var request = await DeserializeRequest (socket).ConfigureAwait (false);

				using (var responseStream = HandleRequest (request, catalog)) {
					var responseLength = responseStream.Length - responseStream.Position;
					await EnsureWrite (socket, BitConverter.GetBytes (IPAddress.HostToNetworkOrder (responseLength)));
					using (var outStream =  new NetworkStream (socket, true))
						await responseStream.CopyToAsync (outStream, 4096, token).ConfigureAwait (false);
				}
			} catch {
				socket.Close ();
				throw;
			}
		}

		static async Task<string> DeserializeRequest (Socket socket)
		{
			var messageLengthHeader = new byte [4];
			await EnsureRead (socket, messageLengthHeader);

			var messageLength = IPAddress.NetworkToHostOrder (BitConverter.ToInt32 (messageLengthHeader, 0));
			var message = new byte [messageLength];
			await EnsureRead (socket, message);

			return Encoding.UTF8.GetString (message);
		}

		static Stream HandleRequest (string request, Catalog catalog)
		{
			LoggingService.LogInfo ("Handling request: {0}", request);
			if (request.StartsWith (Messages.FetchTrack, StringComparison.Ordinal)) {
				var message = Newtonsoft.Json.JsonConvert.DeserializeObject<FetchTrackMessage> (request.Substring (Messages.FetchTrack.Length));
				var track = catalog.FindTrack (message.UUID);
				var stream = File.OpenRead (track.FilePath);
				stream.Seek (message.Offset, SeekOrigin.Begin);
				return stream;
			} else if (request.StartsWith (Messages.FetchCatalog)) {
				var message = Newtonsoft.Json.JsonConvert.DeserializeObject<FetchCatalogMessage> (request.Substring (Messages.FetchCatalog.Length));
				if (message?.UUID == catalog.UUID)
					return new MemoryStream (new byte [0]);
				return new MemoryStream (catalog.ToJson ());
			} else if (request.StartsWith (Messages.FetchGzipCompressedCatalog)) {
				var message = Newtonsoft.Json.JsonConvert.DeserializeObject<FetchCatalogMessage> (request.Substring (Messages.FetchGzipCompressedCatalog.Length));
				if (message?.UUID == catalog.UUID)
					return new MemoryStream (new byte [0]);
				return new MemoryStream (catalog.ToGzipCompressedJson ());
			} else {
				return Stream.Null;
			}
		}

		static async Task EnsureRead (Socket readStream, byte[] buffer)
		{
			await Task.Run (() => {
				int read = 0;
				while (buffer.Length - read > 0)
					read += readStream.Receive (buffer, read, buffer.Length - read, SocketFlags.None);
			});
		}

		static async Task EnsureWrite (Socket writeStream, byte[] data)
		{
			await Task.Run (() => {
				var written = 0;
				while (data.Length - written > 0)
					written += writeStream.Send (data, written, data.Length - written, SocketFlags.None);
			});
		}
	}
}
