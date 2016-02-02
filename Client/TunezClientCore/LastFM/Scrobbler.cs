using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tunez
{
	public class Scrobbler : IScrobbler
	{
		static readonly Uri RestAPI = new Uri ("https://ws.audioscrobbler.com/2.0/");

		string username;
		string password;

		// The cache directory.
		readonly string CacheDirectory;

		// The Users login/password for their last.fm account
		public string Username {
			get { return username; }
			set { username = value; SessionKey = null; Files.Delete (SessionKeyFile); }
		}

		public string Password {
			get { return password; }
			set { password = value; SessionKey = null; Files.Delete (SessionKeyFile); }
		}

		// The session key to communicate with the last.fm aip
		string SessionKey;

		string LoginCacheFile {
			get { return Path.Combine (CacheDirectory, "lastfm.userlogin"); }
		}

		/// <summary>
		/// This file stores all the scrobbles we need to report to last.fm
		/// </summary>
		/// <value>The scrobble cache.</value>
		string ScrobbleCache {
			get { return Path.Combine (CacheDirectory, "lastfm.cachedscrobbles"); }
		}

		/// <summary>
		/// This is the file which stores the session key we get from last.fm
		/// </summary>
		/// <value>The session key cache file.</value>
		string SessionKeyFile {
			get { return Path.Combine (CacheDirectory, "lastfm.cachedsessionkey"); }
		}

		bool LoggedIn {
			get { return !string.IsNullOrEmpty (SessionKey); }
		}

		public Scrobbler (string cacheDirectory)
		{
			CacheDirectory = cacheDirectory;

			try {
				if (File.Exists (LoginCacheFile)) {
					var lines = File.ReadAllLines (LoginCacheFile);
					Username = lines [0];
					Password = lines [1];
				}
			} catch {
				Files.Delete (LoginCacheFile);
				Username = null;
				Password = null;
			}
		}

		public async Task<bool> Scrobble (Track track)
		{
			if (!await Login ()) {
				LoggingService.LogInfo ("Last.FM login failed... caching scrobble");
				SaveToCache (track);
				return false;
			}

			if (await ScrobbleFromCache ()) {
				Files.Delete (ScrobbleCache);
			} else {
				LoggingService.LogInfo ("Could not flush existing cache, appending to cache instead of scrobbling");
				SaveToCache (track);
				return false;
			}

			if (!await Scrobble (new [] { track })) {
				LoggingService.LogInfo ("Failed to scrobble: {0}. Caching it.", track);
				SaveToCache (track);
				return false;
			}
			LoggingService.LogInfo ("Successfully scrobbled: {0}", track);
			return true;
		}

		public async Task<bool> Login ()
		{
			if (string.IsNullOrEmpty (Username) || string.IsNullOrEmpty (Password))
				return false;

			try {
				if (SessionKey != null)
					return true;

				if (File.Exists (SessionKeyFile)) {
					SessionKey = File.ReadAllText (SessionKeyFile);
					return true;
				}
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Could not read the cached session key");
				Files.Delete (SessionKeyFile);
				LoggingService.LogError (ex, "Deleted the cached session key");
			}

			try {
				var uri = CreateAuthenticationUri ();
				var response = await PostAsync (uri);
				return HandleAuthResponse (response);
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Could not log in to last.fm");
			}

			return false;
		}

		async Task<bool> ScrobbleFromCache ()
		{
			try {
				return await Scrobble (ReadScrobblesFromCache ());
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Could not scrobble the cache");
				return false;
			}
		}

		IEnumerable<Track> ReadScrobblesFromCache ()
		{
			var serializer = new JsonSerializer();
			if (File.Exists (ScrobbleCache)) {
				using (var streamReader = new StreamReader (ScrobbleCache, Encoding.UTF8))
				using (var reader = new JsonTextReader (streamReader) { SupportMultipleContent = true }) {
					while (reader.Read ())
						yield return serializer.Deserialize<Track> (reader);
				}
			} else {
				yield break;
			}
		}

		void SaveToCache (Track track)
		{
			try {
				Directory.CreateDirectory (Path.GetDirectoryName (ScrobbleCache));
				File.AppendAllText (ScrobbleCache, JsonConvert.SerializeObject (track));
			} catch (Exception ex) {
				LoggingService.LogError (ex, "Could not append to the scrobble cache. Data is lost!");
			}
		}

		async Task<bool> Scrobble (IEnumerable<Track> tracks)
		{
			foreach (var batch in tracks.Partition (50)) {
				var scrobbleContent = CreateScrobbleQueryParams (batch);
				var result = await PostAsync (RestAPI, new StringContent (scrobbleContent, Encoding.UTF8));
				if (!HandleScrobbleResponse (result))
					return false;
			}
			return true;
		}

		string CreateScrobbleQueryParams (IEnumerable<Track> batch)
		{
			var queryParameters = new QueryParameters {
				Tuple.Create ("method", "track.scrobble"),
				Tuple.Create ("api_key", LastFMAppCredentials.APIKey),
				Tuple.Create ("sk", SessionKey),
			};

			int trackNumber = 0;
			foreach (var track in batch) {
				queryParameters.AddIndexed ("artist", track.TrackArtist, trackNumber);
				queryParameters.AddIndexed ("track", track.Name, trackNumber);
				queryParameters.AddIndexed ("timestamp", track.Timestamp.ToString (), trackNumber);
				queryParameters.AddIndexed ("album", track.Album, trackNumber);
				queryParameters.AddIndexed ("trackNumber", track.Number.ToString (), trackNumber);
				queryParameters.AddIndexed ("duration", track.Duration.ToString (), trackNumber);
				if (!string.IsNullOrEmpty (track.AlbumArtist))
					queryParameters.AddIndexed ("albumArtist", track.AlbumArtist, trackNumber);
				if (!string.IsNullOrEmpty (track.MusicBrainzId))
					queryParameters.AddIndexed ("mbid", track.MusicBrainzId, trackNumber);
				trackNumber ++;
			}

			return queryParameters.GenerateQueryString (LastFMAppCredentials.SharedSecret);
		}

		bool HandleAuthResponse (JObject json)
		{
			var session = json.GetValue ("session");
			if (session != null) {
				SessionKey = session.SelectToken ("key").ToString ();
				Directory.CreateDirectory (Path.GetDirectoryName (SessionKeyFile));
				File.WriteAllText (SessionKeyFile, SessionKey);

				Directory.CreateDirectory (Path.GetDirectoryName (LoginCacheFile));
				File.WriteAllLines (LoginCacheFile, new [] { Username, Password });
			} else {
				SessionKey = null;
				Files.Delete (SessionKeyFile);
				LoggingService.LogInfo ("Could not log in to last.fm: {0}", json.GetValue ("message"));
			}
			return SessionKey != null;
		}

		bool HandleScrobbleResponse (JObject json)
		{
			var error = json.GetValue ("error");
			if (error is JValue && ((JValue)error).Value.ToString () == "9") {
				SessionKey = null;
				Files.Delete (SessionKeyFile);
				return false;
			} else if (error != null) {
				LoggingService.LogInfo ("Could not scrobble: {0}", json.GetValue ("message"));
				return false;
			} else {
				return true;
			}
		}

		Uri CreateAuthenticationUri ()
		{
			var queryString = CreateAuthQueryParams ().GenerateQueryString (LastFMAppCredentials.SharedSecret);
			var authUri = new UriBuilder (RestAPI) {
				Query = queryString
			};
			return authUri.Uri;
		}

		QueryParameters CreateAuthQueryParams ()
		{
			return new QueryParameters {
				Tuple.Create ("api_key", LastFMAppCredentials.APIKey),
				Tuple.Create ("method", "auth.getMobileSession"),
				Tuple.Create ("username", Username),
				Tuple.Create ("password", Password),
			};
		}

		async Task<JObject> PostAsync (Uri uri, HttpContent content = null)
		{
			using (var client = new HttpClient (new ModernHttpClient.NativeMessageHandler ())) {
				client.DefaultRequestHeaders.Add ("User-Agent", "Tunez 1.0");
				var response = await client.PostAsync (uri, content);
				var responseString = await response.Content.ReadAsStringAsync ();
				return JsonConvert.DeserializeObject<JObject> (responseString);
			}
		}
	}
}
