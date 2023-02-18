using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using Stratus;

namespace Stratus.Extensions
{
	public static class StratusCollectionExtensions
	{
		/// <summary>
		/// Returns true if the collection is empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The list.</param>
		/// <returns>True if the list is empty, false otherwise</returns>
		public static bool IsNullOrEmpty<T>(this Stack<T> collection)
		{
			return collection == null || collection.Count == 0;
		}

		/// <summary>
		/// Returns true if the collection is not empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The array.</param>
		/// <returns>True if the array is not empty, false otherwise</returns>
		public static bool IsValid<T>(this Stack<T> collection)
		{
			return collection != null && collection.Count > 0;
		}

		/// <summary>
		/// Returns true if the collection is empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The list.</param>
		/// <returns>True if the list is empty, false otherwise</returns>
		public static bool IsNullOrEmpty<T>(this Queue<T> collection)
		{
			return collection == null || collection.Count == 0;
		}

		/// <summary>
		/// Returns true if the collection is not empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The array.</param>
		/// <returns>True if the array is not empty, false otherwise</returns>
		public static bool IsValid<T>(this Queue<T> collection)
		{
			return collection != null && collection.Count > 0;
		}

		/// <summary>
		/// Returns true if the given collection is either null nor empty
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
			=> collection == null || collection.Count == 0;

		/// <summary>
		/// Returns true if the given collection is valid (not null or empty)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <returns></returns>
		public static bool IsValid<T>(this ICollection<T> collection)
			=> collection != null && collection.Count > 0;

		/// <summary>
		/// Returns the collection to an array (an empty one if it's null)
		/// </summary>
		public static T[] ToArrayOrEmpty<T>(this ICollection<T> collection)
		{
			if (collection == null)
			{
				return new T[] { };
			}

			return collection.ToArray();
		}


		/// <summary>
		/// Returns the length of the collection; if it's null it will return 0
		/// </summary>
		/// <returns>The length of the collection, 0 if null</returns>
		public static int LengthOrZero(this ICollection collection) => collection != null ? collection.Count : 0;

		/// <summary>
		/// Returns true if the collection is valid and contains the given value. 
		/// False otherwise.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryContains<T>(this ICollection<T> source, T value) => source.IsValid() && source.Contains(value);

		/// <summary>
		/// Pushes all the given elements onto the stack
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack"></param>
		/// <param name="values"></param>
		public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> values)
		{
			foreach (var element in values)
			{
				stack.Push(element);
			}
		}

		/// <summary>
		/// Pushes all the given elements onto the stack
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stack"></param>
		/// <param name="values"></param>
		public static void PushRange<T>(this Stack<T> stack, params T[] values)
		{
			stack.PushRange((IEnumerable<T>)values);
		}

		/// <summary>
		/// Enqueues all the given elements onto the queue
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queue"></param>
		/// <param name="values"></param>
		public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> values)
		{
			foreach (var element in values)
			{
				queue.Enqueue(element);
			}
		}

		/// <summary>
		/// Enqueues all the given elements onto the queue
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="queue"></param>
		/// <param name="values"></param>
		public static void EnqueueRange<T>(this Queue<T> queue, params T[] values)
		{
			queue.EnqueueRange((IEnumerable<T>)values);
		}

		public static T PopOrDefault<T>(this Stack<T> stack)
		{
			return stack.Count > 0 ? stack.Pop() : default;
		}

		/// <summary>
		/// Compares two arrays to determine whether they have equal values
		/// </summary>
		public static StratusOperationResult IsEqualInValues<T>(this ICollection<T> first, ICollection<T> second)
		{
			return first.ToArray().IsComparableByValues(second.ToArray());
		}
	}
}