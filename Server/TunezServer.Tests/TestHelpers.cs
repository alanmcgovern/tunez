using System;
using System.Linq;
using System.Collections.Generic;

namespace TunezServer
{
	public static class TestHelpers
	{
		public static IEnumerable<IEnumerable<T>> AllPermutations<T> (this IEnumerable<T> enumerable)
		{
			var input = enumerable.ToArray ();
			foreach (var v in AllPermutations (input, 0, input.Length))
				yield return v;
		}

		static IEnumerable<T[]> AllPermutations<T> (T[] array, int start, int end)
		{
			if (start == end - 1) {
				yield return array.ToArray (); // clone it
			} else {
				foreach (var v in AllPermutations (array, start + 1, end))
					yield return v;

				for (int i = start + 1; i < end; i ++) {
					// swap two to create a permutation
					var tmp = array [i];
					array [i] = array [start];
					array [start] = tmp;

					foreach (var v in AllPermutations (array, start + 1, end))
						yield return v;

					// Swap them back so we can try the next permutation
					array [start] = array [i];
					array [i] = tmp;
				}
			}
		}
	}
}

