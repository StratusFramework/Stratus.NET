using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Given a sequence, returns an array of each elements <see cref="object.ToString"/>'s method
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] ToStringArray<T>(this IEnumerable<T> enumerable)
			=> enumerable.ToStringArray(x => x.ToString());

		/// <summary>
		/// Given a sequence, returns an array of each elements stringified by the provided function
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] ToStringArray<T>(this IEnumerable<T> enumerable, Func<T, string> stringFunction)
		{
			List<string> values = new List<string>();
			foreach (T entry in enumerable)
			{
				values.Add(stringFunction(entry));
			}
			return values.ToArray();
		}

		/// <summary>
		/// Given a sequence of elements, stringifies them before joining them together into a single string.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string ToStringJoin<T>(this IEnumerable<T> enumerable,
											 string separator = ",",
											 StratusStringEnclosure enclosure = StratusStringEnclosure.None,
											 Func<T, string> stringFunction = null)
		{
			return stringFunction != null ?
				enumerable.ToStringArray(stringFunction).Join(separator)
				: enumerable.ToStringArray().Join(separator).Enclose(enclosure);
		}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] TypeNames<T>(this IEnumerable<T> enumerable)
		{
			List<string> names = new List<string>();
			foreach (T item in enumerable)
			{
				names.Add(item.GetType().Name);
			}
			return names.ToArray();
		}

		/// <summary>
		/// Returns an array of strings, consisting of the names identified on their name property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static string[] TypeNames(this IEnumerable<Type> enumerable)
		{
			List<string> names = new List<string>();
			foreach (Type item in enumerable)
			{
				names.Add(item.Name);
			}
			return names.ToArray();
		}

		/// <summary>
		/// Returns the last element of a sequence
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T LastOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return Enumerable.LastOrDefault(source, predicate);
		}

		/// <summary>
		/// Checks whether this enumerables has elements with duplicate keys (uniquely identifying properties),
		/// given a function that determines the key for each element
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static bool HasDuplicateKeys<T>(this IEnumerable<T> enumerable, Func<T, string> keyFunction)
		{
			HashSet<string> hashset = new HashSet<string>();
			foreach (T element in enumerable)
			{
				string key = keyFunction(element);
				if (hashset.Contains(key))
				{
					return true;
				}
				hashset.Add(key);
			}
			return false;
		}

		/// <summary>
		/// Checks whether this enumerables has elements with duplicate keys (uniquely identifying properties),
		/// using the default comprarer
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static bool HasDuplicateKeys<T>(this IEnumerable<T> enumerable)
			where T : IComparable
		{
			return !enumerable.All(new HashSet<T>().Add);
		}

		/// <summary>
		/// Finds the first element of this enumerable that matches the predicate function
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static T FindFirst<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
		{
			foreach (T element in enumerable)
			{
				if (predicate(element))
				{
					return element;
				}
			}
			return default;
		}

		/// <summary>
		/// Returns the first element in this list that has a duplicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static T FindFirstDuplicate<T>(this IEnumerable<T> enumerable,
			Func<T, string> keyFunction)
		{
			HashSet<string> hashset = new HashSet<string>();
			foreach (T element in enumerable)
			{
				string key = keyFunction(element);
				if (hashset.Contains(key))
				{
					return element;
				}
				hashset.Add(key);
			}
			return default;
		}

		/// <summary>
		/// Returns the first element in this list that has a duplicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static T FindFirstDuplicate<T>(this IEnumerable<T> enumerable)
		{
			HashSet<T> hashset = new HashSet<T>();
			foreach (T element in enumerable)
			{
				if (hashset.Contains(element))
				{
					return element;
				}
				hashset.Add(element);
			}
			return default;
		}


		/// <summary>
		/// Returns an array with no null elements
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <returns></returns>
		public static IEnumerable<T> TruncateNull<T>(this IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				if (item != null)
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Converts an enumerable from one type to another through a conversion function
		/// </summary>
		public static IEnumerable<U> Transform<T, U>(this IEnumerable<T> source, Func<T, U> function)
		{
			foreach (T item in source)
			{
				yield return function(item);
			}
		}

		/// <summary>
		/// Perform an action on each item that isn't null
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<U> ConvertNotNull<T, U>(this IEnumerable<T> source, Func<T, U> func)
			where T : class
		{
			if (source == null)
			{
				yield break;
			}

			foreach (T item in source)
			{
				if (item != null)
				{
					yield return func(item);
				}
			}
		}

		/// <summary>
		/// Inverts the order of the elements in a sequence.
		/// </summary>
		public static IEnumerable<T> Reverse<T>(this IList<T> source)
		{
			return Enumerable.Reverse(source);
		}

		/// <summary>
		/// Converts an enumerable from one type to an array of another through a conversion function
		/// </summary>
		public static U[] ToArray<T, U>(this IEnumerable<T> source, Func<T, U> function)
		{
			return source.Transform(function).ToArray();
		}

		/// <summary>
		/// Perform an action on each item.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source != null)
			{
				foreach (T item in source)
				{
					action(item);
				}
			}
			return source;
		}

		/// <summary>
		/// Perform an action on each item.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEach<T>(this IEnumerable<T> source, params Action<T>[] actions)
		{
			var actionEnumerator = actions.GetEnumerator();
			foreach (T item in source)
			{
				actionEnumerator.MoveNext();
				var action = actionEnumerator.Current as Action<T>;
				action(item);
			}
		}

		/// <summary>
		/// Perform an action on each item, in reverse
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEachReverse<T>(this IEnumerable<T> source, Action<T> action)
		{
			source.Reverse().ForEach(action);
		}

		/// <summary>
		/// Perform an action on each item that isn't null
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static void ForEachNotNull<T>(this IEnumerable<T> source, Action<T> action)
			where T : class
		{
			if (source == null)
			{
				return;
			}

			foreach (T item in source)
			{
				if (item != null)
				{
					action(item);
				}
			}
		}

		/// <summary>
		/// Perform an action on each item, with an iteration counter
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="action">The action to perform.</param>
		public static IEnumerable<T> ForEachIndexed<T>(this IEnumerable<T> source, Action<T, int> action)
		{
			int counter = 0;

			foreach (T item in source)
			{
				action(item, counter++);
			}

			return source;
		}

		/// <summary>
		/// Executes an action on 2 sequences in parallel
		/// </summary>
		public static void ForEachParallel<T, U>(this IEnumerable<T> source, IEnumerable<U> other, Action<T, U> action)
		{
			var firstEnumerator = source.GetEnumerator();
			var secondEnumerator = other.GetEnumerator();

			if (!firstEnumerator.MoveNext() || secondEnumerator.MoveNext())
			{
				return;
			}

			do
			{
				action(firstEnumerator.Current, secondEnumerator.Current);
			}
			while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext());
		}

		/// <summary>
		/// Add a collection to the end of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="append">The collection to append.</param>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, IEnumerable<T> append)
		{
			foreach (T item in source)
			{
				yield return item;
			}

			foreach (T item in append)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Add a collection to the end of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="append">The collection to append.</param>
		public static IEnumerable<T> AppendWhere<T>(this IEnumerable<T> source, IEnumerable<T> append, Predicate<T> predicate)
		{
			foreach (T item in source)
			{
				yield return item;
			}

			foreach (T item in append)
			{
				if (predicate(item))
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Add a collection to the beginning of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="prepend">The collection to append.</param>
		public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, IEnumerable<T> prepend)
		{
			foreach (T item in prepend)
			{
				yield return item;
			}

			foreach (T item in source)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Add a collection to the beginning of another collection.
		/// </summary>
		/// <param name="source">The collection.</param>
		/// <param name="prepend">The collection to append.</param>
		public static IEnumerable<T> PrependWhere<T>(this IEnumerable<T> source, Predicate<T> predicate, IEnumerable<T> prepend)
		{
			foreach (T item in prepend)
			{
				if (predicate(item))
				{
					yield return item;
				}
			}

			foreach (T item in source)
			{
				yield return item;
			}
		}

		/// <summary>
		/// Filters a sequence of values based on a predicate. 
		/// Each element's index is used in the logic of the predicate function.
		/// </summary>
		public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return source.Where(predicate);
		}

		/// <summary>
		/// Finds the intersection of a group of groups.
		/// </summary>
		public static IEnumerable<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> groups)
		{
			HashSet<T> hashSet = null;

			foreach (IEnumerable<T> group in groups)
			{
				if (hashSet == null)
				{
					hashSet = new HashSet<T>(group);
				}
				else
				{
					hashSet.IntersectWith(group);
				}
			}

			return hashSet == null ? Enumerable.Empty<T>() : hashSet.AsEnumerable();
		}

		/// <summary>
		/// Sorts the elements of a sequence in ascending or descending order. 
		/// </summary>
		public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.OrderBy(selector);
			}
			else
			{
				return source.OrderByDescending(selector);
			}
		}

		/// <summary>
		/// Performs a subsequent ordering of the elements in a sequence in ascending or descending order.
		/// </summary>
		public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector, bool ascending)
		{
			if (ascending)
			{
				return source.ThenBy(selector);
			}
			else
			{
				return source.ThenByDescending(selector);
			}
		}

		/// <summary>
		/// Returns a dictionary from the given enumerable, given a function to get the key for each value
		/// </summary>
		public static Dictionary<Key, Value> ToDictionary<Key, Value>(this IEnumerable<Value> source,
			Func<Value, Key> keyFunction,
			bool unique = true)
		{
			Dictionary<Key, Value> dictionary = new Dictionary<Key, Value>();
			if (source != null)
			{
				if (unique)
				{
					dictionary.AddRange(keyFunction, source);
				}
				else
				{
					dictionary.AddRangeUnique(keyFunction, source);
				}
			}

			return dictionary;
		}

		/// <summary>
		/// Returns a dictionary from the given enumerable, given a function to get the key for each value
		/// </summary>
		public static Dictionary<TKey, TValue> ToDictionary<TElement, TKey, TValue>(this IEnumerable<TElement> source,
			Func<TElement, (TKey key, TValue value)> selector)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			if (source != null)
			{
				foreach(var element in source)
				{
					if (element != null)
					{
						var kvp = selector(element);
						dictionary.Add(kvp.key, kvp.value);
					}
				}
			}
			return dictionary;
		}

		/// <summary>
		/// Returns a dictionary from the given enumerable, given a function to get a value for each key and a predicate
		/// </summary>
		public static Dictionary<Key, Value> ToDictionaryFromKey<Key, Value>(this IEnumerable<Key> source,
			Func<Key, Value> valueFunction,
			Predicate<Key> predicate = null)
		{
			Dictionary<Key, Value> dictionary = new Dictionary<Key, Value>();
			if (predicate != null)
			{
				dictionary.AddRangeByKey(valueFunction, predicate, source);
			}
			else
			{
				dictionary.AddRange(valueFunction, source);
			}

			return dictionary;
		}

		/// <summary>
		/// Merges two sequences together using a given selector
		/// </summary>
		public static IEnumerable<TResult> Merge<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
																	IEnumerable<TSecond> second,
																	Func<TFirst, TSecond, TResult> selector)
		{
			var firstEnumerator = first.GetEnumerator();
			var secondEnumerator = second.GetEnumerator();
			while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
			{
				yield return selector(firstEnumerator.Current, secondEnumerator.Current);
			}
		}

		public static Dictionary<TKey, TValue[]> ToDictionary<TKey, TValue>(this IEnumerable<TKey> keys, Func<TKey, TValue[]> selector)
		{
			Dictionary<TKey, TValue[]> result = new Dictionary<TKey, TValue[]>();
			foreach (var key in keys)
			{
				result.Add(key, selector(key));
			}
			return result;
		}
	}
}