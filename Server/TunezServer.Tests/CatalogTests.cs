using System;
using System.Linq;
using NUnit.Framework;
using Tunez;

namespace TunezServer
{
	[TestFixture]
	public class CatalogTests
	{
		int UUID;

		[SetUp]
		public void Setup ()
		{
			UUID = 0;
		}

		[Test]
		public void ArtistSortOrder_The ()
		{
			var tracks = new Track [] {
				BasicTrack (albumArtist: "A", trackArtist: "A"),
				BasicTrack (albumArtist: "The B", trackArtist: "The B"),
				BasicTrack (albumArtist: "C", trackArtist: "C"),
			};
			var catalog = new Catalog (tracks);
			Assert.AreEqual ("A", catalog.Artists [0].Name);
			Assert.AreEqual ("The B", catalog.Artists [1].Name);
			Assert.AreEqual ("C", catalog.Artists [2].Name);
		}

		[Test]
		public void MultiDisc ()
		{
			var tracks = new Track[] {
				BasicTrack ("d1t1", 1, 1),
				BasicTrack ("d1t2", 1, 2),
				BasicTrack ("d2t1", 2, 1),
				BasicTrack ("d2t2", 2, 2),
				BasicTrack ("d3t6", 3, 6),
			};

			foreach (var catalog in tracks.AllPermutations ().Select (p => new Catalog (p))) {
				Assert.AreEqual (1, catalog.Artists.Length, "#1");
				Assert.AreEqual (1, catalog.Artists [0].Albums.Length, "#2");

				var t = catalog.Artists.First ().Albums.First ().Tracks;
				Assert.That (tracks, Is.EqualTo (catalog.Artists.First ().Albums.First ().Tracks), "#track order");
			}
		}


		Track BasicTrack (string name = null, int? disc = null, int? track = null, string trackArtist = null, string albumArtist = null)
		{
			return new Track {
				Album = "Album",
				AlbumArtist = albumArtist ?? "Album Artist",
				Disc = disc.GetValueOrDefault (1),
				Duration = 1000,
				FilePath = "File/Path/" + UUID,
				FileSize = 1024,
				MusicBrainzId = "SomeMusicBrainzId",
				Name =  name ?? "Name",
				Number = track.GetValueOrDefault (1),
				TrackArtist = trackArtist ?? "Track Artist",
				UUID = UUID ++
			};
		}
	}
}

