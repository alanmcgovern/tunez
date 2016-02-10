using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tunez;

namespace TunezServer
{
	[TestFixture]
	public class EnumerableExtensionsTests
	{
		[Test]
		public void Parition_EmptyArray ()
		{
			var partition = new int [0].Partition (50);
			Assert.IsFalse (partition.Any (), "#1");
		}

		[Test]
		public void Parition_ArrayMatchesPartitionSize ()
		{
			var partition = new int [50].Partition (50);
			Assert.AreEqual (1, partition.Count (), "#1");
		}

		[Test]
		public void Partition_TwoGroups ()
		{
			var partition = new int [50].Partition (25);
			Assert.AreEqual (2, partition.Count (), "#1");
			foreach (var p in partition)
				Assert.AreEqual (25, p.Count (), "#2");
		}

		[Test]
		public void Permutations_ThreeItems ()
		{
			var permutations = Enumerable.Range (1, 3).AllPermutations ().ToArray ();
			Assert.AreEqual (6, permutations.Length, "#1");
			for (int i = 0; i < permutations.Length; i ++) {
				for (int j = 0; j < permutations.Length; j ++) {
					if (i != j)
						Assert.IsFalse (permutations [i].SequenceEqual (permutations [j]));
				}
			}
		}

		[Test]
		public void Permutations_FourItems ()
		{
			var permutations = Enumerable.Range (1, 4).AllPermutations ().ToArray ();
			Assert.AreEqual (24, permutations.Length, "#1");
			for (int i = 0; i < permutations.Length; i ++) {
				for (int j = 0; j < permutations.Length; j ++) {
					if (i != j)
						Assert.IsFalse (permutations [i].SequenceEqual (permutations [j]));
				}
			}
		}
	}
}

