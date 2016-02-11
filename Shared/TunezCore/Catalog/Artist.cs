using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tunez
{
	[Newtonsoft.Json.JsonObject]
	public class Artist : IEnumerable<Track>
	{
		public Album[] Albums {
			get; set;
		}

		string name;
		public string Name
		{
			get { return name ?? "Unknown"; }
			set { name = value; }
		}

		public string SortKey {
			get {
				var track = Albums.FirstOrDefault ()?.Tracks.FirstOrDefault ();
				return track?.AlbumArtistSortOrder ?? track?.TrackArtistSortOrder ?? Name;
			}
		}

		public Artist()
		{
		}

		IEnumerator<Track> IEnumerable<Track>.GetEnumerator ()
		{
			foreach (var album in Albums)
				foreach (var track in album)
					yield return track;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<Track>) this).GetEnumerator ();
		}
	}
}
