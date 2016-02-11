using System;
using System.Collections;
using System.Collections.Generic;

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
			get { return Name; }
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
