using System.Linq;
using NUnit.Framework;
using Tunez;

namespace Tunez
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
	}
}

