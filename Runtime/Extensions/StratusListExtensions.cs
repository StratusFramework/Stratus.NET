using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Extensions
{
	public static class StratusListExtensions
	{
		/// <summary>
		/// Removes all null values from this list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns>The number of null elements removed</returns>
		public static int RemoveNull<T>(this List<T> list) where T : class
		{
			return list.RemoveAll(x => x == null || x.Equals(null));
		}

		/// <summary>
		/// Adds the given elements into the list (params T[] to IEnumerable<T>)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static void AddRange<T>(this List<T> list, params T[] values)
		{
			list.AddRange(values);
		}

		/// <summary>
		/// Iterates over the given list, removing any invalid elements (described hy the validate functon)
		/// Returns true if any elements were removed.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="predicate"></param>
		/// <param name="action"></param>
		public static bool ForEachRemoveInvalid<T>(this List<T> list,
			Action<T> action,
			Predicate<T> predicate)
			where T : class
		{
			bool removed = false;
			List<T> invalid = new List<T>();
			foreach (T element in list)
			{
				// Remove invalid elements
				bool valid = predicate(element);
				removed |= valid;

				if (!valid)
				{
					invalid.Add(element);
					continue;
				}

				// Apply the iteration function
				action(element);
			}

			list.RemoveAll(x => invalid.Contains(x));
			return removed;
		}

		/// <summary>
		/// Iterates over the given list, removing any invalid elements (described hy the validate functon)    
		/// Returns true if any elements were removed
		/// </summary>    
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="predicate"></param>
		/// <param name="iterateFunction"></param>
		public static bool RemoveInvalid<T>(this List<T> list, Predicate<T> predicate)
		{
			bool removed = false;
			List<T> invalid = new List<T>();
			foreach (T element in list)
			{
				// Remove invalid elements
				bool valid = predicate(element);
				removed |= valid;

				if (!valid)
				{
					invalid.Add(element);
				}
			}

			list.RemoveAll(x => invalid.Contains(x));
			return removed;
		}

		/// <summary>
		/// Clones all the elements of this list, if they are cloneable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="listToClone"></param>
		/// <returns></returns>
		public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}

		/// <summary>
		/// Adds all elements not already present into the given list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> values)
		{
			list.AddRange(values.Where(x => !list.Contains(x)));
		}

		/// <summary>
		/// Adds all elements not already present into the given list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static void AddRangeUnique<T>(this List<T> list, params T[] values)
		{
			list.AddRangeUnique((IEnumerable<T>)values);
		}

		/// <summary>
		/// Adds all values that fulfill the given predicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static int AddRangeWhere<T>(this List<T> list, Predicate<T> predicate, IEnumerable<T> values)
		{
			int count = 0;
			values.ForEach((x) =>
			{
				if (predicate(x))
				{
					list.Add(x);
					count++;
				}
			});
			return count;
		}

		/// <summary>
		/// Adds all values that fulfill the given predicate
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		public static int AddRangeWhere<T>(this List<T> list, Predicate<T> predicate, params T[] values)
		{
			return list.AddRangeWhere(predicate, (IEnumerable<T>)values);
		}

		/// <summary>
		/// Adds the items from another sequence, except those that are null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns>True if any null elements were detected</returns>
		public static void AddRangeNotNull<T>(this List<T> list, IEnumerable<T> values)
			where T : class
		{
			list.AddRange(values.Where(x => x != null));
		}

		/// <summary>
		/// Adds the items from another list, except null ones
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns>True if any null elements were detected</returns>
		public static void AddRangeNotNull<T>(this List<T> list, params T[] values)
			where T : class
		{
			list.AddRangeNotNull((IEnumerable<T>)values);
		}

		/// <summary>
		/// Adds the element if it's not null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool AddIfNotNull<T>(this List<T> list, T item)
			where T : class
		{
			if (item != null)
			{
				list.Add(item);
				return true;
			}
			return false;
		}



		/// <summary>
		/// Swaps 2 elements in a list by index
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="indexA"></param>
		/// <param name="indexB"></param>
		public static void SwapAtIndex<T>(this IList<T> list, int indexA, int indexB)
		{
			T tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}

		/// <summary>
		/// Swaps 2 elements in a list by looking up the index of the values
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static void Swap<T>(this IList<T> list, T a, T b)
		{
			int indexA = list.IndexOf(a);
			int indexB = list.IndexOf(b);
			list.SwapAtIndex(indexA, indexB);
		}

		/// <summary>
		/// Returns the last index of the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static int LastIndex<T>(this IList<T> list)
		{
			return list.Count - 1;
		}

		/// <summary>
		/// Returns the first element if the list if there's one, or null 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static T FirstOrDefault<T>(this IList<T> list)
		{
			return list.IsValid() ? list[0] : default;
		}

		/// <summary>
		/// Removes the last element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void RemoveFirst<T>(this IList<T> list)
		{
			if (list.IsValid())
			{
				list.RemoveAt(0);
			}
		}

		/// <summary>
		/// Removes the last element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void RemoveLast<T>(this IList<T> list)
		{
			if (list.IsValid())
			{
				list.RemoveAt(list.Count - 1);
			}
		}

		/// <summary>
		/// Returns the element at the given index, or the default (null for class types)
		/// </summary>
		public static T AtIndexOrDefault<T>(this IList<T> list, int index)
		{
			return list.ContainsIndex(index) ? list[index] : default;
		}

		/// <summary>
		/// Returns the element at the given index, or the given default value
		/// </summary>
		public static T AtIndexOrDefault<T>(this IList<T> list, int index, T defaultValue)
		{
			return list.ContainsIndex(index) ? list[index] : defaultValue;
		}

		/// <summary>
		/// Determines whether a sequence contains a specified element
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool Contains<T>(this IList<T> source, T value)
		{
			return source.IsValid() && source.Contains(value);
		}

		/// <summary>
		/// Returns true if the <see cref="IList"/> contains the given element
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool ContainsIndex<T>(this IList<T> source, int index)
		{
			if (source == null || source.Count == 0 || index < 0)
			{
				return false;
			}

			return index <= source.Count - 1;
		}

		/// <summary>
		/// Checks if the list contains any of the given values
		/// </summary>
		public static bool ContainsAny<T>(this IList<T> list, IEnumerable<T> values)
		{
			return values.Any(x => list.Contains(x));
		}

		/// <summary>
		/// Checks if the list contains any of the given values
		/// </summary>
		public static bool ContainsAll<T>(this IList<T> list, IEnumerable<T> values)
		{
			return values.All(x => list.Contains(x));
		}

		/// <summary>
		/// Shuffles the list using a randomized range based on its size.
		/// </summary>
		/// <typeparam name="T">The type of the list.</typeparam>
		/// <param name="list">A reference to the list.</param>
		/// <remarks>Courtesy of Mike Desjardins #UnityTips</remarks>
		/// <returns>A new, shuffled list.</returns>
		public static void Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; ++i)
			{
				T index = list[i];
				int randomIndex = StratusRandom.Range(i, list.Count);
				list[i] = list[randomIndex];
				list[randomIndex] = index;
			}
		}

		/// <summary>
		/// Returns a random element from the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T Random<T>(this IList<T> list)
		{
			int randomSelection = StratusRandom.Range(0, list.Count);
			return list[randomSelection];
		}

	}

}