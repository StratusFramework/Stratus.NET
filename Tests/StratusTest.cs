using NUnit.Framework;

using Stratus.Extensions;
using Stratus.Reflection;

using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
	public abstract class StratusTest
	{
		public static void AssertContains<T, V>(T key, IReadOnlyDictionary<T, V> dictionary)
		{
			Assert.True(dictionary.ContainsKey(key), $"Dictionary contains {dictionary.ToStringJoin()}");
		}

		public static void AssertContains<T>(HashSet<T> set, T key)
		{
			Assert.True(set.Contains(key), $"Hashset contains {set.ToStringJoin()}");
		}

		public static void AssertContainsExactly<T, V>(IReadOnlyDictionary<T, V> dictionary, params T[] keys)
		{
			Assert.AreEqual(keys.Length, dictionary.Count, $"Dictionary contains {dictionary.ToStringJoin()}");
			foreach(var key in keys)
			{
				AssertContains(key, dictionary);
			}
		}

		public static void AssertSuccess(StratusOperationResult result)
		{
			Assert.True(result.valid, result.message);
		}

		public static void AssertFailure(StratusOperationResult result)
		{
			Assert.False(result.valid, result.message);
		}

		public static void AssertEquality<T>(ICollection<T> expected, ICollection<T> actual)
		{
			AssertSuccess(expected.IsEqualInValues(actual));
		}

		public static void AssertEquality<T>(T[] expected, T[] actual)
		{
			AssertSuccess(expected.IsComparableByValues(actual));
		}

		public static void AssertLength<T>(int expected, IList<T> list)
		{
			Assert.AreEqual(expected, list.Count, $"List contained {list.ToStringJoin().Enclose(StratusStringEnclosure.SquareBracket)}");
		}

		public static void AssertEqualFields<T>(T a, T b)
		{
			StratusTypeInfo info = StratusTypeInfo.From(a);
			foreach(var field in info.fields)
			{
				object aValue = field.GetValue(a);
				object bValue = field.GetValue(b);
				Assert.AreEqual(aValue, bValue, $"{a} did not match {b}");
			}
		}
	}
}