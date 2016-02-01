using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace Tunez
{
	public static class Caches
	{
		/// <summary>
		/// This directory is backed up by itunes and can be used to store application configuration related files
		/// </summary>
		/// <value>The application support.</value>
		static string ApplicationSupport {
			get { return Path.Combine (Library, "Application Support", Foundation.NSBundle.MainBundle.BundleIdentifier); }
		}

		static string Library {
			get {
#if __IOS__
				return Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), "..", "Library");
#else
				return Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "Library");
#endif
			}
		}

		static string KnownServers {
			get {
				return Path.Combine (ApplicationSupport, "knownservers.cache");
			}
		}

		public static string TunezCatalog {
			get { return Path.Combine (ApplicationSupport, "tunez.catalog"); }
		}

		public static string Scrobbler {
			get {
				return Path.Combine (ApplicationSupport, "Scrobbler");
			}
		}

		public static ServerDetails[] LoadKnownServers ()
		{
			return Load<ServerDetails[]> (KnownServers) ?? new ServerDetails [0];
		}

		public static void SaveKnownServers (ServerDetails[] servers)
		{
			Store (KnownServers, servers);
		}

		static T Load<T> (string path)
		{
			if (File.Exists (path))
				return JsonConvert.DeserializeObject<T> (File.ReadAllText (path, Encoding.UTF8));
			return default (T);
		}

		static void Store (string path, object value)
		{
			Directory.CreateDirectory (Path.GetDirectoryName (path));
			File.WriteAllText (path, JsonConvert.SerializeObject (value));
		}
	}
}
