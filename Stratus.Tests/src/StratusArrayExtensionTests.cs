using NUnit.Framework;

using Stratus.Extensions;

using System;
using System.Linq;

namespace Stratus.Tests
{
	public class StratusArrayExtensionTests
	{
		private static readonly int a = 1, b = 2, c = 3, d = 4;
		private static readonly int[] first = new int[] { a, b };
		private static readonly int[] second = new int[] { c, d };

		[Test]
		public void TestFind()
		{
			// FindIndex
			{
				int[] values = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				Assert.AreEqual(4, values.FindIndex(5));
			}

			// FindIndex, Find, Exists
			{
				string[] values = new string[] { "Hello", "There", "Brown", "Cow" };
				Assert.AreEqual(2, values.FindIndex("Brown"));
				Assert.AreEqual("Hello", values.Find((x) => x.Contains("llo")));
				Assert.IsTrue(values.Contains((x) => x.Equals("There")));
			}
		}

		[Test]
		public void TestSort()
		{
			float a = 1.25f, b = 2.5f, c = 3.75f, d = 5.0f;
			// Custom comparer
			{
				float[] values = new float[] { d, b, a, c };
				values.SortInPlace((x, y) => x > y ? 1 : x < y ? -1 : 0);
				Assert.AreEqual(new float[] { a, b, c, d }, values);
			}
			// Default (from interface)
			{
				float[] values = new float[] { d, b, a, c };
				values.SortInPlace();
				Assert.AreEqual(new float[] { a, b, c, d }, values);
			}
		}

		[Test]
		public void TestTruncate()
		{
			int[] values = new int[] { 1, 2, 3 };
			Assert.AreEqual(new int[] { 2, 3 }, values.TruncateFront());
			Assert.AreEqual(new int[] { 1, 2 }, values.TruncateBack());
			Assert.AreEqual(new int[] { 1, 3 }, values.Truncate(2));
			Assert.AreEqual(new int[] { 1, 2 }, values.Truncate(3));
			Assert.AreEqual(new int[] { 2, 3 }, values.Truncate(1));
		}

		[Test]
		public void TestAppend()
		{
			int[] third = first.Append(second).ToArray();
			Assert.AreEqual(new int[] { a, b, c, d }, third);
		}

		[Test]
		public void TestAppendWhere()
		{
			int[] fifth = first.AppendWhere((x) => x < 4, second);
			Assert.AreEqual(new int[] { a, b, c }, fifth);
		}

		[Test]
		public void TestPrepend()
		{
			int[] fourth = first.Prepend(second).ToArray();
			Assert.AreEqual(new int[] { c, d, a, b }, fourth);
		}

		[Test]
		public void TestPrependWhere()
		{
			int[] sixth = first.PrependWhere((x) => x > 3, second).ToArray();
			Assert.AreEqual(new int[] { d, a, b }, sixth);
		}

		[Test]
		public void TestConcat()
		{
			int[] a = new int[] { 1, 3, 5 }, b = new int[] { 2, 4, 6 };
			Assert.AreEqual(new int[] { 1, 3, 5, 2, 4, 6 }, a.Concat(b));
		}

		[Test]
		public void ReverseInPlace()
		{
			int a = 1, b = 2, c = 3;
			int[] values = new int[] { a, b, c };
			values.ReverseInPlace();
			Assert.AreEqual(c, values[0]);
			Assert.AreEqual(b, values[1]);
			Assert.AreEqual(a, values[2]);
		}

	}

}