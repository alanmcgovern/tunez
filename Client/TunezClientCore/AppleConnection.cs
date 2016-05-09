using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreFoundation;
using Tunez;

namespace Tunez
{
	public class AppleConnection : IConnection
	{
		ServerDetails Server {
			get;
		}

		public AppleConnection (ServerDetails server)
		{
			Server = server;
		}

		async Task<Tuple<CFWriteStream, CFReadStream>> Connect ()
		{
			return await Task.Run (() => {
				CFReadStream readStream = null;
				CFWriteStream writeStream = null;
				CFStream.CreatePairWithSocketToHost (Server.Hostname, Server.Port, out readStream, out writeStream);
				readStream.EnableEvents (CFRunLoop.Main, CFRunLoop.ModeDefault);
				writeStream.EnableEvents (CFRunLoop.Main, CFRunLoop.ModeDefault);
				readStream.ReadDispatchQueue = DispatchQueue.MainQueue;
				writeStream.WriteDispatchQueue = DispatchQueue.MainQueue;

				foreach (var stream in new CFStream[] { readStream, writeStream }) {
					stream.ErrorEvent += (object sender, CFStream.StreamEventArgs e) => {
						Console.WriteLine ("Error: {0}", ((CFStream) sender).GetError ()); 
					};
					stream.OpenCompletedEvent += (object sender, CFStream.StreamEventArgs e) => {
						Console.WriteLine ("Opened: {0}", e.EventType);
					};
				}
				return Tuple.Create (writeStream, readStream);
			});
		}

		async Task Send (CFWriteStream writeStream, string dataString)
		{
			var data = System.Text.Encoding.UTF8.GetBytes (dataString); 
			var length = System.Net.IPAddress.HostToNetworkOrder (data.Length);
			await EnsureWrite (writeStream, BitConverter.GetBytes (length));
			await EnsureWrite (writeStream, data);
		}

		async Task EnsureRead (CFReadStream readStream, byte[] buffer)
		{
			await Task.Run (() => {
				int read = 0;
				while (buffer.Length - read > 0)
					read += (int) readStream.Read (buffer, read, buffer.Length - read);
			});
		}

		async Task EnsureWrite (CFWriteStream writeStream, byte[] data)
		{
			await Task.Run (async () => {
				for (int i = 0; i < 10; i ++) {
					if (!writeStream.CanAcceptBytes ())
						await Task.Delay (1000);
				}
				var written = 0;
				while (data.Length - written > 0)
					written += writeStream.Write (data, written, data.Length - written);
			});
		}

		public async Task<byte[]> GetResponse (string message, string payload)
		{
			var streams = await Connect ();
			try {
				streams.Item1.Open ();
				await Send (streams.Item1, message + payload);

				streams.Item2.Open ();
				var responseLengthHeader = new byte [4];
				await EnsureRead (streams.Item2, responseLengthHeader);

				var responseLength = System.Net.IPAddress.NetworkToHostOrder (BitConverter.ToInt32 (responseLengthHeader, 0));
				var response = new byte [responseLength];
				await EnsureRead (streams.Item2, response);
				return response;
			} finally {
				streams.Item1.Close ();
				streams.Item2.Close ();
			}
		}

		public async Task<Stream> GetStreamingResponse(string message, string payload)
		{
			var bytes = await GetResponse (message, payload);
			return new MemoryStream (bytes);
		}
	}
}

