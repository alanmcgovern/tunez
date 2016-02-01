using System;
using System.Collections;
using System.Collections.Generic;

namespace Tunez
{
	[Newtonsoft.Json.JsonObject]
	public class Track : IEnumerable<Track>
	{
		/// <summary>
		/// The name of the album the track belongs to
		/// </summary>
		/// <value>The album.</value>
		public string Album {
			get; set;
		}


		/// <summary>
		/// The name of artist which created the album
		/// </summary>
		/// <value>The album.</value>
		public string AlbumArtist {
			get; set;
		}

		/// <summary>
		/// If this track is part of a multi-disc album, this is the disc the track belongs to
		/// </summary>
		/// <value>The disc.</value>
		public int Disc {
			get; set;
		}

		/// <summary>
		/// The duration of the track in seconds
		/// </summary>
		/// <value>The length.</value>
		public int Duration {
			get; set;
		}

		/// <summary>
		/// The path to the file on disk
		/// </summary>
		/// <value>The file path.</value>
		public string FilePath {
			get; set;
		}

		public long FileSize {
			get; set;
		}

		/// <summary>
		/// The position of the track in the album
		/// </summary>
		/// <value>The number.</value>
		public int Number {
			get; set;
		}

		/// <summary>
		/// The name of the track.
		/// </summary>
		/// <value>The title.</value>
		string name;
		public string Name {
			get { return name ?? "Unknown"; }
			set { name = value; }
		}

		public string MusicBrainzId {
			get; set;
		}

		/// <summary>
		/// The time when the track was last played in UNIX epoch time.
		/// </summary>
		/// <value>The timestamp.</value>
		public int Timestamp {
			get; set;
		}

		/// <summary>
		/// The name of the artist the trakc belongs to
		/// </summary>
		/// <value>The artist.</value>
		public string TrackArtist {
			get; set;
		}

		/// <summary>
		/// ID used between the client and server to uniquely identify tracks
		/// </summary>
		/// <value>The UUID.</value>
		public int UUID {
			get; set;
		}

		public Track ()
		{
		}

		public override string ToString()
		{
			return string.Format("[Track: Album={0}, AlbumArtist={1}, Number={2}, Name={3}, MusicBrainzId={4}, TrackArtist={5}]", Album, AlbumArtist, Number, Name, MusicBrainzId, TrackArtist);
		}

		IEnumerator<Track> IEnumerable<Track>.GetEnumerator ()
		{
			yield return this;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<Track>) this).GetEnumerator ();
		}
	}
}
