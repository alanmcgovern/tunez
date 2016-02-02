using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Tunez
{
	public static class Caches
	{
		public static string CacheRoot {
			get { return Path.Combine (Path.GetDirectoryName (typeof (Caches).Assembly.Location), "Caches"); }
		}

		static string TrackLoaderPathsCache {
			get { return Path.Combine (CacheRoot, "trackloader.cache"); }
		}

		public static HashSet<string> GetTrackLoaderPaths ()
		{
			if (File.Exists (TrackLoaderPathsCache)) {
				try {
					var data = File.ReadAllText (TrackLoaderPathsCache, Encoding.UTF8);
					return new HashSet<string> (JsonConvert.DeserializeObject<string[]> (data));
				} catch {
					File.Delete (TrackLoaderPathsCache);
				}
			}

			return new HashSet<string> ();
		}

		public static void StoreTrackLoaderPaths (HashSet<string> paths)
		{
			Directory.CreateDirectory (Path.GetDirectoryName (TrackLoaderPathsCache));
			File.WriteAllText (TrackLoaderPathsCache, JsonConvert.SerializeObject (paths.ToArray ()), Encoding.UTF8);
		}
	}
}

