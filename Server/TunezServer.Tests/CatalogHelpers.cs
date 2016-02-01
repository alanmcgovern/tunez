using System;
using System.Collections.Generic;
using System.Linq;
using Tunez;

namespace TunezServer.Tests
{
	public static class CatalogHelpers
	{
		static Random Random = new Random (0);

		public static Catalog Create ()
		{
			var uuid = 0;

			var tracks = CreateTracks (ref uuid, "Artist1", "Album1", 12)
				.Concat (CreateTracks (ref uuid, "Artist1", "Album2", 13))
				.Concat (CreateTracks (ref uuid, "Artist2", "Album1", 14));
			return new Catalog (tracks);
		}

		public static Catalog Create (int artists, int albums, int tracks)
		{
			var uuid = 0;

			var results = new List<Track> ();
			for (int artist = 0; artist < artists; artist ++) {
				for (int album = 0; album < albums; album ++) {
					results.AddRange (CreateTracks (ref uuid, "Artist" + artist, "Album" + album, tracks));
				}
			}
			return new Catalog (results);
		}

		static IEnumerable<Track> CreateTracks (ref int uuid, string albumArtist, string album, int trackCount)
		{
			var results = new List<Track> ();
			for (int i = 1; i <= trackCount; i ++) {
				results.Add (new Track {
					AlbumArtist = albumArtist,
					TrackArtist = albumArtist,
					Album = album,
					Disc = 1,
					Duration = Random.Next (0, 300),
					FilePath = string.Format ("path/to/Track{0}.mp3", i),
					Name = string.Format ("Track{0}.mp3", i),
					Number = i,
					UUID = uuid ++
				});
			}
			return results;
		}
	}
}

