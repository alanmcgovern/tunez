using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tunez
{
	public class TunezServer
	{
		public ServerDetails ServerDetails {
			get;
		}

		public TunezServer (ServerDetails serverDetails)
		{
			ServerDetails = serverDetails;
		}

		public async Task<Catalog> FetchCatalog (string cachedCatalogPath, CancellationToken token)
		{
			Catalog cachedCatalog = null;
			if (File.Exists (cachedCatalogPath)) {
				try {
					cachedCatalog = Catalog.FromGzipCompressedJson (File.ReadAllBytes (cachedCatalogPath));
				} catch (Exception ex) {
					LoggingService.LogError (ex, "Could not load the cached catalog... ignoring it");
				}
			}

			while (true) {
				try {
					return await FetchCatalog (cachedCatalogPath, cachedCatalog, token);
				} catch (WebException) {
					LoggingService.LogInfo ("Could not fetch the catalog, retrying in 1 second.");
					await Task.Delay (1000);
				}
			}
		}

		async Task<Catalog> FetchCatalog (string cachedCatalogPath, Catalog cachedCatalog, CancellationToken token)
		{
			var start = DateTime.UtcNow;
			using (var client = new HttpClient ()) {
				var message = Newtonsoft.Json.JsonConvert.SerializeObject (new FetchCatalogMessage { UUID = (cachedCatalog?.UUID).GetValueOrDefault (-1) });

				var response = await client.PostAsync (ServerDetails.FullAddress, new StringContent (Messages.FetchGzipCompressedCatalog + message, Encoding.UTF8), token);
				var compressedJson = await response.Content.ReadAsByteArrayAsync ();
				LoggingService.LogInfo ("Received {0}kb catalog in {1:0.00} seconds.", compressedJson.Length, (DateTime.UtcNow - start).TotalSeconds);
				if (compressedJson.Length == 0)
					return cachedCatalog;

				cachedCatalog = Catalog.FromGzipCompressedJson (compressedJson);
				Directory.CreateDirectory (Path.GetDirectoryName (cachedCatalogPath));
				File.WriteAllBytes (cachedCatalogPath, compressedJson);
				return cachedCatalog;
			}
		}
	}
}
