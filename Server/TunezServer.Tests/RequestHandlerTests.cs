using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TunezServer.Tests;

namespace Tunez
{
	[TestFixture]
	public class RequestHandlerTests
	{
		Catalog Catalog {
			get; set;
		}

		CancellationTokenSource Cancellation {
			get; set;
		}

		RequestHandler Handler {
			get; set;
		}

		Task ListenTask {
			get; set;
		}

		string ServerAddress {
			get { return string.Format ("http://127.0.0.1:{0}", Handler.ListenPort); }
		}

		[SetUp]
		public void Setup ()
		{
			Cancellation = new CancellationTokenSource ();
			Catalog = CatalogHelpers.Create ();
			Handler = new RequestHandler ();
			ListenTask = Task.FromResult (true);
		}

		[TearDown]
		public void Teardown ()
		{
			Cancellation.Cancel ();
			ListenTask.WaitOrCanceled ().Wait ();
		}

		[Test]
		public async Task FetchLargeCatalog ()
		{
			Handler.Catalog = Catalog = CatalogHelpers.Create (100, 10, 10);
			ListenTask = Handler.BeginListeningAsync (Cancellation.Token);
			var json = await FetchCatalogJson ();
			Assert.AreEqual (Catalog.ToJson (), json, "#1");

		}

		[Test]
		//[Ignore ("MonoBug: Calling webrequest.abort can fail in many strange and unpredictable ways. Internal state gets screwed up and EndGetResponse can throw many types of exception, such as ObjectDisposed exceptions when a WebException is expected")]
		public async Task FetchLargeCatalog_AbortBeforeResponse ()
		{
			Handler.Catalog = Catalog = CatalogHelpers.Create (100, 10, 10);
			ListenTask = Handler.BeginListeningAsync (Cancellation.Token);

			for (int i = 0; i < 100; i ++) {
				var client = WebRequest.CreateHttp (ServerAddress);
				client.Method = "POST";
				using (var request = client.GetRequestStream ())
					request.Write (Encoding.UTF8.GetBytes (Messages.FetchCatalog), 0, Messages.FetchCatalog.Length);
				var asyncResult = client.BeginGetResponse (null, null);
				client.Abort ();
				try {
					var finalResult = client.EndGetResponse (asyncResult);
					Assert.Fail ("It should have aborted");
				} catch (WebException) {
					// Good!
				} catch (ObjectDisposedException) {

				}
			}

			var json = await FetchCatalogJson ();
			Assert.AreEqual (Catalog.ToJson (), json, "#1");

		}

		async Task<string> FetchCatalogJson ()
		{
			using (var client = new HttpClient ()) {
				var response = await client.PostAsync (ServerAddress, new StringContent (Messages.FetchCatalog, Encoding.UTF8));
				return Encoding.UTF8.GetString (await response.Content.ReadAsByteArrayAsync ());
			}
		}
	}
}

