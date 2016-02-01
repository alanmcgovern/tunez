﻿using System;
using Tunez;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;

namespace Tunez
{
	public class RequestHandler
	{
		public int ListenPort {
			get { return 51986; }
		}

		public async Task BeginListeningAsync (Catalog catalog, CancellationToken token)
		{
			var listener = new HttpListener ();
			listener.Prefixes.Add (string.Format ("http://*:{0}/", ListenPort));

			try {
				using (token.Register (() => listener.Abort ()))
				using (listener) {
					listener.Start ();
					while (!token.IsCancellationRequested) {
						var context = await listener.GetContextAsync ().ConfigureAwait (false);
						ContextReceived (catalog, context, token);
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
				LoggingService.LogError (ex, "Unhandled exception in BeginListeningAsync");
			}
		}

		static async void ContextReceived (Catalog catalog, HttpListenerContext context, CancellationToken token)
		{
			try {
				LoggingService.LogInfo ("Received a new request");
				using (context.Response)
					await SendResponse (catalog, context, token).ConfigureAwait (false);
			} catch (IOException) {
				LoggingService.LogInfo ("Client probably disconnected.");
			} catch (Exception ex) {
				context.Response.Abort ();
				LoggingService.LogError (ex, "The connection died. Oops!");
			}
		}

		static async Task SendResponse (Catalog catalog, HttpListenerContext context, CancellationToken token)
		{
			try {
				var request = await DeserializeRequest (context.Request).ConfigureAwait (false);

				using (var responseStream = HandleRequest (request, catalog)) {
					context.Response.SendChunked = true;
					context.Response.ContentLength64 = responseStream.Length - responseStream.Position;
					context.Response.StatusCode = (int)HttpStatusCode.OK;
					await responseStream.CopyToAsync (context.Response.OutputStream, 4096, token).ConfigureAwait (false);
				}
			} catch {
				context.Response.Abort ();
				throw;
			}
		}

		static async Task<string> DeserializeRequest (HttpListenerRequest request)
		{
			int offset = 0;
			var buffer = new byte [(int) request.ContentLength64];
			while (offset < buffer.Length) {
				var read = await request.InputStream.ReadAsync (buffer, offset, buffer.Length - offset).ConfigureAwait (false);
				if (read <= 0)
					throw new EndOfStreamException ("We could not read all the content");
				offset += read;
			}

			return Encoding.UTF8.GetString (buffer);
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
	}
}
