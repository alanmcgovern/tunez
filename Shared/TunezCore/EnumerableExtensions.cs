using System;
using System.Linq;
using System.Collections.Generic;

namespace Tunez
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Randomize<T> (this IEnumerable<T> enumerable)
		{
			Random random = new Random ();
			var allTracks = enumerable.ToList ();
			while (allTracks.Count > 0) {
				var index = random.Next (0, allTracks.Count);
				yield return allTracks [index];
				allTracks.RemoveAt (index);
			}
		}

		public static IEnumerable<IEnumerable<T>> Partition<T> (this IEnumerable<T> enumerable, int partitionSize)
		{
			var parts = new List<T> ();
			foreach (var item in enumerable) {
				parts.Add (item);
				if (parts.Count == partitionSize) {
					yield return parts.ToArray ();
					parts.Clear ();
				}
			}

			if (parts.Count == 0)
				yield break;
			else
				yield return parts;
		}
	}
}
