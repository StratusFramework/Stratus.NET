using System;
using System.Collections.Generic;

namespace Stratus.Extensions
{
	public static class IntegerExtensions
	{
		/// <summary>
		/// Performs the action the specified number of times
		/// </summary>
		/// <param name="x"></param>
		/// <param name="action"></param>
		public static void Iterate(this int x, Action action)
		{
			for (int i = 0; i < x; ++i)
			{
				action();
			}
		}

		/// <summary>
		/// Performs the zero-indexed action the specified number of times
		/// </summary>
		/// <param name="x"></param>
		/// <param name="action"></param>
		public static void Iterate(this int x, Action<int> action)
		{
			for (int i = 0; i < x; ++i)
			{
				action(i);
			}
		}

		/// <summary>
		/// Performs the zero-indexed action the specified number of times,
		/// (From x-1 to 0)
		/// </summary>
		/// <param name="x"></param>
		/// <param name="action"></param>
		public static void IterateReverse(this int x, Action<int> action)
		{
			for (int i = x - 1; i >= 0; --i)
			{
				action(i);
			}
		}

		public static IEnumerable<T> For<T>(this int x, Func<int, T> func)
		{
			for (int i = 0; i < x; ++i)
			{
				yield return func(i);
			}
		}

		public static IEnumerable<T> For<T>(this int x, Func<T> func)
		{
			for (int i = 0; i < x; ++i)
			{
				yield return func();
			}
		}
	}

}