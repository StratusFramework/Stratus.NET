using NUnit.Framework;

using Stratus.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Editor.Tests
{
	public partial class StratusDictionaryExtensionsTests
	{
		private static readonly TestDataObject a = new TestDataObject("A", 3);
		private static readonly TestDataObject b = new TestDataObject("B", 5);
		private static readonly TestDataObject c = new TestDataObject("C", 7);

		private static Func<TestDataObject, string> keyFunction = (x) => x.name;

		[Test]
		public void AddRange()
		{
			Dictionary<string, TestDataObject> values = new Dictionary<string, TestDataObject>();
			TestDataObject[] range = new TestDataObject[] { a, b, c };
			values.AddRange(keyFunction, range);
			Assert.AreEqual(3, values.Count);

			values.Clear();
			values.AddRange(keyFunction, a, b, c);
			Assert.AreEqual(3, values.Count);
		}

		[Test]
		public void AddRangeUnique()
		{
			Dictionary<string, TestDataObject> dict = new Dictionary<string, TestDataObject>();
			Assert.True(dict.AddUnique(keyFunction(a), a));
			Assert.True(dict.AddUnique(keyFunction(b), b));
			Assert.True(dict.AddUnique(keyFunction(c), c));
			Assert.False(dict.AddUnique(keyFunction(a), a));
			Assert.False(dict.AddUnique(keyFunction(b), b));
			Assert.False(dict.AddUnique(keyFunction(c), c));
			Assert.AreEqual(3, dict.Count);
		}

		[Test]
		public void TryInvokeFunction()
		{
			Dictionary<string, TestDataObject> values = new Dictionary<string, TestDataObject>();
			values.AddRange(keyFunction, a, b, c);
			int result;

			result = values.TryInvoke(a.name, (x) => x.value);
			Assert.AreEqual(result, a.value);

			result = -1;
			values.TryInvoke("NULL", (x) => { result = x.value; });
			Assert.AreNotEqual(result, b.value);
		}

		[Test]
		public void TryInvokeActionOnElement()
		{
			Dictionary<string, TestDataObject> values = new Dictionary<string, TestDataObject>();
			values.AddRange(keyFunction, a, b, c);
			
			int result = 0;

			void action(TestDataObject obj)
			{
				result = obj.value;
			}

			values.TryInvoke(a.name, action);
			Assert.AreEqual(result, a.value);
		}

		[Test]
		public void TryInvokeActionOnListOfElements()
		{
			Dictionary<string, List<TestDataObject>> values = new Dictionary<string, List<TestDataObject>>();
			string key = "foo";
			values.Add(key, new List<TestDataObject>());
			values[key].AddRange(a, b, c);

			int result = 0;

			void action(TestDataObject obj)
			{
				result += obj.value;
			}

			values.TryInvoke(key, action);
			Assert.AreEqual(result, values[key].Sum(x => x.value));
		}

		[TestCase(1, 1, false)]
		[TestCase(1, 2, true)]
		public void AddUnique(int first, int second, bool expected)
		{
			Dictionary<int, string> values = new Dictionary<int, string>();
			Assert.True(values.AddUnique(first, first.ToString()));
			Assert.That(values.AddUnique(second, second.ToString()) == expected);
		}

		public void AddOrUpdate()
		{
			Dictionary<int, int> values = new Dictionary<int, int>();
			int key = 7;
			values.AddOrUpdate(key, 3);
			Assert.AreEqual(3, values[key]);
			values.AddOrUpdate(key, 2);
			Assert.AreEqual(2, values[key]);
		}

		[TestCase(11, 2, 3, 5, 1)]
		[TestCase(40, 20, 20)]
		[TestCase(0, 0, 0, 0)]
		public void AddOrIncrement(int expected, params int[] values)
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			string key = "foo";
			values.ForEach(x => dict.AddOrIncrement(key, x));
			Assert.AreEqual(expected, dict[key]);
		}

		[TestCase(3, 1, 2, 3)]
		[TestCase(2, 1, 2)]
		[TestCase(0)]
		public void AddRangeFromValueToKey(int count, params int[] values)
		{
			Dictionary<int, int> dict = new Dictionary<int, int>();
			dict.AddRange(x => x, values);
			Assert.AreEqual(count, dict.Count);
		}

		[TestCase(3, 1, 2, 3)]
		[TestCase(2, 1, 2)]
		[TestCase(0)]
		public void AddRangeFromKeyToValue(int count, params int[] values)
		{
			Dictionary<string, int> dict = new Dictionary<string, int>();
			dict.AddRange(x => x.ToString(), values);
			Assert.AreEqual(count, dict.Count);
		}

		[TestCase(5, 3, 5, 10, 15)]
		public void AddRangeByValue(int cutoff, int count, params int[] values)
		{
			Predicate<int> predicate = (int i) => i >= cutoff;
			Func<int, int> selector = x => x * 2;
			Dictionary<int, int> dict = new Dictionary<int, int>();
			dict.AddRangeByValue(selector, predicate, values);
			Assert.AreEqual(dict.Count, count);
		}

		[TestCase(5, 3, 5, 10, 15)]
		public void AddRangeByKey(int cutoff, int count, params int[] values)
		{
			Predicate<int> predicate = (int i) => i >= cutoff;
			Func<int, int> selector = x => x * 2;
			Dictionary<int, int> dict = new Dictionary<int, int>();
			dict.AddRangeByKey(selector, predicate, values);
			Assert.AreEqual(dict.Count, count);
		}

		[Test]
		public void GetValueOrDefault()
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			string key = "foo", value = "bar", defaultValue = "foobar";
			Assert.AreEqual(default, dict.GetValueOrDefault(key));
			Assert.AreEqual(defaultValue, dict.GetValueOrDefault(key, defaultValue));
			dict.Add(key, value);
			Assert.AreEqual(value, dict.GetValueOrDefault(key));
		}

		[Test]
		public void GetValueOrGenerate()
		{
			int key = 7;
			string value = "foobar";
			Func<int, string> valueFunction = (x) => value;

			Dictionary<int, string> dict = new Dictionary<int, string>();
			dict.Add(key, value);
			Assert.AreEqual(value, dict.GetValueOrGenerate(key, valueFunction));
			dict.Remove(key);
			Assert.AreEqual(value, dict.GetValueOrGenerate(key, valueFunction));
		}

		[Test]
		public void ToStringWithSeparator()
		{
			string separator = ":";
			Dictionary<int, int> dict = new Dictionary<int, int>();
			dict.Add(3, 7);
			dict.Add(5, 5);
			string expected = $"[3{separator}7]{Environment.NewLine}[5{separator}5]{Environment.NewLine}";
			string actual = dict.ToString(separator);
			Assert.AreEqual(expected, actual);
		}
	}
}