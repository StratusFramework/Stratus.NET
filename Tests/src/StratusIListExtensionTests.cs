using NUnit.Framework;

using Stratus.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Stratus.Editor.Tests
{
	public class StratusIListExtensionTests
	{
		[Test]
		public void TestLastIndex()
		{
			int[] values = new int[] { 1, 2, 3, 4, 5 };
			Assert.AreEqual(4, values.LastIndex());
		}

		[Test]
		public void TestShuffle()
		{
			int[] values = new int[] { 1, 2, 3, 4, 5 };
			int[] shuffled = (int[])values.Clone();
			shuffled.Shuffle();
			Assert.AreNotEqual(values, shuffled);
		}

		[Test]
		public void TestSwap()
		{
			int[] values = new int[] { 1, 2, 3, 4, 5 };
			values.SwapAtIndex(0, 4);
			Assert.AreEqual(values, new int[] { 5, 2, 3, 4, 1 });
			values.Swap(1, 5);
			Assert.AreEqual(values, new int[] { 1, 2, 3, 4, 5 });
		}

		[Test]
		public void TestRandom()
		{
			int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			Assert.AreEqual(1, values.First());
			Assert.AreEqual(9, values.Last());


			int a = values.Random();
			int b = -1;
			for (int i = 0; i < values.Length; ++i)
			{
				b = values.Random();
				if (a != b)
				{
					break;
				}
			}
			Assert.AreNotEqual(a, b);
		}

		[Test]
		public void TestRemove()
		{
			List<int> values = new List<int>() { 1, 2, 3, 4 };
			values.RemoveFirst();
			values.RemoveLast();
			Assert.AreEqual(2, values.First());
			Assert.AreEqual(3, values.Last());
		}



		[Test]
		public void TestFirstOrDefault()
		{
			string[] values;

			values = null;
			Assert.Null(values.FirstOrDefault());

			values = new string[] { };
			Assert.Null(values.FirstOrDefault());
			
			string testString = "foo";

			values = new string[] { testString };
			Assert.NotNull(values.FirstOrDefault());

			values = new string[] { null, testString };
			Assert.Null(values.FirstOrDefault());

			values = new string[] { testString, null };
			Assert.NotNull(values.FirstOrDefault());
		}

		[Test]
		public void ContainsIndex()
		{
			TestDataObject a = new TestDataObject("A", 1);
			TestDataObject[] values = new TestDataObject[]
			{
				a
			};

			Assert.False(values.ContainsIndex(-1));
			Assert.True(values.ContainsIndex(0));
			Assert.False(values.ContainsIndex(1));

			Assert.AreEqual(a, values.AtIndexOrDefault(0));
			Assert.AreEqual(a, values.AtIndexOrDefault(5, a));
		}

		[Test]
		public void Contains()
		{
			int a = 7;
			int b = 5;
			List<int> values = new List<int>();
			values.AddRange(a);
			Assert.True(values.Contains(a));
			Assert.False(values.Contains(b));
		}
	}
}