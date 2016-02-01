using System;
using System.Collections;
using System.Collections.Generic;

namespace Tunez
{
	[Newtonsoft.Json.JsonObject]
	public class Album : IEnumerable<Track>
	{
		string name;
		public string Name {
			get { return name ?? "Unknown"; }
			set { name = value; }
		}

		public Track[] Tracks {
			get; set;
		}

		public Album ()
		{
		}

		IEnumerator<Track> IEnumerable<Track>.GetEnumerator ()
		{
			foreach (var track in Tracks)
				foreach (var item in track)
					yield return item;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<Track>) this).GetEnumerator ();
		}
	}
}
