using NUnit.Framework;

using Stratus.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Editor.Tests
{
	public class StratusEnumerableExtensionTests
	{
		[Test]
		public void TestToString()
		{
			string a = "ABCD", b = "EDFG";
			TestDataObject[] values = new TestDataObject[]
			{
					new TestDataObject(a, 1),
					new TestDataObject(b, 2),
			};
			Assert.AreEqual(new string[] { a, b, }, values.ToStringArray());
			Assert.AreEqual(new string[] { a, b, }, values.ToStringArray(x => x.name));
		}

		[Test]
		public void TestJoin()
		{
			string a = "a", b = "b", c = "c", d = "d";
			string[] sequence = new string[] { a, b, c, d };
			string separator = ",";
			Assert.AreEqual($"{a}{separator}{b}{separator}{c}{separator}{d}", sequence.ToStringJoin());
		}

		[Test]
		public void TestTypeNames()
		{
			// Typenames
			{
				object[] values1 = new object[]
				{
					"Hello",
					1,
					2.5f
				};
				Type[] values1Types = new Type[]
				{
					typeof(string),
					typeof(int),
					typeof(float),
				};

				Assert.AreEqual(new string[]
				{
				values1Types[0].Name,
				values1Types[1].Name,
				values1Types[2].Name,
				}, values1.TypeNames());

				Assert.AreEqual(new string[]
				{
				values1Types[0].Name,
				values1Types[1].Name,
				values1Types[2].Name,
				}, values1Types.TypeNames());
			}
		}

		[Test]
		public void TestDuplicateKeys()
		{
			int[] input;
			Func<int, string> keyFunction = x => x.ToString();

			input = new int[] { 2, 3, 4, 2 };
			Assert.True(input.HasDuplicateKeys(keyFunction));
			Assert.True(input.HasDuplicateKeys());
			input = new int[] { 2, 3, 4, 5, 6, 7 };
			Assert.False(input.HasDuplicateKeys(keyFunction));
			Assert.False(input.HasDuplicateKeys());
		}

		[Test]
		public void TestFindFirst()
		{
			{
				int[] values = new int[] { 1, 2, 3, 3, 4 };
				Assert.True(values.HasDuplicateKeys());
				Assert.AreEqual(3, values.FindFirstDuplicate());
			}
			{
				TestDataObject a = new TestDataObject("a", 3);
				TestDataObject b = new TestDataObject("b", 7);
				TestDataObject c = new TestDataObject("c", 5);
				var values = new TestDataObject[] { a, b, c, a, b };
				Assert.AreEqual(a, values.FindFirstDuplicate(x => x.name));
			}
			{
				string a = "12", b = "34", c = "56";
				string[] values = new string[] { a, b, c };
				Assert.AreEqual(b, values.FindFirst(x => x.Contains("3")));
			}
		}

		[Test]
		public void TestForEach()
		{
			int a = 1, b = 2, c = 3;
			int[] values = new int[] { a, b, c };
			List<int> values2 = new List<int>();
			values.ForEach((x) => values2.Add(x + 1));
			Assert.AreEqual(new int[] { a + 1, b + 1, c + 1 }, values2.ToArray());
		}

		[Test]
		public void TestForEachReverse()
		{
			List<int> pool = new List<int>();
			int[] values = new int[] { 1, 2, 3, 4 };
			values.ForEachReverse(x => pool.Add(x));
			Assert.AreEqual(pool.ToArray(), values.Reverse().ToArray());
		}

		[Test]
		public void TestForEachParallel()
		{
			int[] A = new int[] { 1, 2, 3 };
			int[] B = new int[] { 1, 2, 3 };
			A.ForEachParallel(B, (a, b) => Assert.AreEqual(a, b));
		}

		[Test]
		public void TestForEachIndexed()
		{
			TestDataObject[] values = new TestDataObject[]
			{
				new TestDataObject("foo", 3),
				new TestDataObject("bar", 7),
			};
			int[] target = values.Transform(x => x.value).ToArray();
			values.ForEachIndexed((v, i) => Assert.AreEqual(v.value, target[i]));
		}

		[Test]
		public void TestForEachNotNull()
		{
			string[] values = new string[] { "foo", null, "bar", null };
			{
				List<string> result = new List<string>();
				values.ForEachNotNull(x => result.Add(x));
				result.ForEach(x => Assert.NotNull(x));
			}
		}

		[Test]
		public void TestAppend()
		{
			int[] first = new int[] { 1, 2, 3 };
			int[] second = new int[] { 4, 5, 6 };
			int[] result = ((IEnumerable<int>)first).Append((IEnumerable<int>)second).ToArray();
			result.ForEachIndexed((x, i) => Assert.AreEqual(x, i + 1));
		}

		[Test]
		public void TestAppendWhere()
		{
			int[] first = new int[] { 2, 8, 10 };
			int[] second = new int[] { 4, 2, 6, 7 };
			int[] result = ((IEnumerable<int>)first).AppendWhere(second, x => x % 2 == 0).ToArray();
			result.ForEach(x => Assert.IsTrue(x % 2 == 0));
		}

		[Test]
		public void TestDictionary()
		{
			string a = "1", b = "2", c = "3";
			int aValue = 1, bValue = 2, cValue = 3;

			TestDataObject[] values = new TestDataObject[]
			{
					new TestDataObject(a, aValue),
					new TestDataObject(b, bValue),
					new TestDataObject(c, cValue),
			};
			Dictionary<string, TestDataObject> dict;

			void test()
			{
				Assert.AreEqual(3, dict.Count);
				Assert.AreEqual(aValue, dict[a].value);
				Assert.AreEqual(bValue, dict[b].value);
				Assert.AreEqual(cValue, dict[c].value);
			}

			dict = values.ToDictionary<string, TestDataObject>((x) => x.name);
			test();

			dict = new string[] { a, b, c }.ToDictionaryFromKey(x => new TestDataObject(x, int.Parse(x)));
			test();
		}

		[Test]
		public void TestToArray()
		{
			int[] a = new int[] { 1, 2, 3, };
			string[] b = a.ToArray<int, string>((x) => x.ToString());
			Assert.AreEqual(new string[] { "1", "2", "3" }, b);
		}

		[Test]
		public void TestTruncateNull()
		{
			string a = "Hello", b = "Goodbye";
			string[] values = new string[] { a, null, b };
			Assert.AreEqual(new string[] { a, b }, values.TruncateNull().ToArray());
		}

		[Test]
		public void TestConvert()
		{
			int[] before = new int[] { 1, 2, 3 };
			string[] after = new string[] { "1", "2", "3" };
			Assert.AreEqual(before.Transform(x => x.ToString()), after);
		}

		[Test]
		public void TestConvertNotNull()
		{
			string[] values = new string[] { "foo", null, "bar", null };
			TestDataObject[] result = values.ConvertNotNull(x => new TestDataObject(x, Stratus.Models.Math.RandomUtility.Range(1, 5))).ToArray();
			Assert.True(result.Length == 2);
			Assert.AreEqual(result[0].name, "foo");
			Assert.AreEqual(result[1].name, "bar");
		}
	}
}