using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Tunez
{
	[Newtonsoft.Json.JsonObject]
	public class Catalog : IEnumerable<Track>
	{
		Dictionary<int, Track> uuidToTrack;
		Dictionary<int, Track> UuidToTrack {
			get { 
				if (uuidToTrack == null) {
					uuidToTrack = new Dictionary<int, Track> ();
					foreach (var artist in Artists)
						foreach (var album in artist.Albums)
							foreach (var track in album.Tracks)
								uuidToTrack.Add (track.UUID, track);
				}
				return uuidToTrack;
			}
		}

		public Artist[] Artists {
			get; set;
		}

		[Newtonsoft.Json.JsonIgnore]
		public int Count {
			get { return UuidToTrack.Count; }
		}

		public long UUID {
			get; set;
		}

		public Catalog ()
		{
			UUID = DateTime.Now.Ticks;
		}

		public Catalog (IEnumerable<Track> tracks)
		{
			Artists = tracks
				.GroupBy (t => t.AlbumArtist ?? t.TrackArtist)
				.Select (ToArtist)
				.OrderBy (t => t.SortKey)
				.ToArray ();
			UUID = DateTime.Now.Ticks;
		}

		public Track FindTrack (int uuid)
		{
			Track result;
			return UuidToTrack.TryGetValue (uuid, out result) ? result : null;
		}

		public byte[] ToGzipCompressedJson ()
		{
			var data = ToJson ();
			var result = new MemoryStream ();
			using (var compressor = new GZipStream (result, CompressionLevel.Optimal))
				compressor.Write (data, 0, data.Length);
			return result.ToArray ();
		}

		public byte[] ToJson ()
		{
			return Encoding.UTF8.GetBytes (Newtonsoft.Json.JsonConvert.SerializeObject (this));
		}

		IEnumerator<Track> IEnumerable<Track>.GetEnumerator ()
		{
			foreach (var artist in Artists)
				foreach (var track in artist)
					yield return track;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<Track>) this).GetEnumerator ();
		}

		public static Catalog FromGzipCompressedJson (byte[] compressedJson)
		{
			var result = new MemoryStream ();
			using (var decompressor = new GZipStream (new MemoryStream (compressedJson), CompressionMode.Decompress))
				decompressor.CopyTo (result);
			return Catalog.FromJson (result.ToArray ());
		}

		public static Catalog FromJson (byte[] json)
		{
			return FromJson (Encoding.UTF8.GetString (json));
		}

		static Catalog FromJson (string json)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Catalog> (json);
		}

		static Artist ToArtist (IGrouping<string, Track> artistData)
		{
			var albums = artistData
				.GroupBy (t => t.Album)
				.Select (ToAlbum)
				.OrderBy (t => t.Name)
				.ToArray ();

			return new Artist {
				Name = artistData.Key,
				Albums = albums
			};
		}

		static Album ToAlbum (IGrouping<string, Track> albumData)
		{
			// 1) Group the tracks in this album by disc number
			// 2) Sort it so the discs are in numerical order
			// 3) Sort the contents of each disc so the tracks are ordered by track number
			// 4) Flatten out the groups into a single IEnumerable<Track> again
			// 5) Make it an array!
			var tracks = albumData
				.GroupBy (t => t.Disc)
				.OrderBy (t => t.Key)
				.Select (t => t.OrderBy (track => track.Number))
				.Flatten ()
				.ToArray ();

			return new Album {
				Name = albumData.Key,
				Tracks = tracks
			};
		}
	}
}
