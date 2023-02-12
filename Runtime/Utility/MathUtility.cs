using System;

namespace Stratus.Utilities
{
	public static class MathUtility
	{
		/// <summary>
		/// A permutation is a way to select a part of a collection, or a set of things in which the order matters
		/// </summary>
		/// <param name="n">Number of objects to choose from</param>
		/// <param name="r">How many to choose</param>
		/// <param name="repeating">If repetition of the same element is allowed within the permutation, such as [1,1] </param>
		/// <returns>The number of ways the objects can be selected</returns>
		/// <remarks>Used for ordered lists</remarks>
		public static int Permutations(int n, int r, bool repeating)
		{
			if (repeating)
			{
				return (int)Pow(n, (double)r);
			}
			return Factorial(n) / Factorial(n - r);
		}

		public static int Factorial(int n)
		{
			if (n == 1)
			{
				return 1;
			}

			int result = 1;
			while (n > 0)
			{
				result *= n;
				n--;
			}

			return result;
		}

		public static double Pow(double x, double y)
		{
			return Math.Pow(x, y);
		}

		public static float Pow(float x, float y)
		{
			return (float)Pow(x, y);
		}

		public static float Sin(float a)
		{
			return (float)Math.Sin(a);
		}

		public static float Cos(float a)
		{
			return (float)Math.Cos(a);
		}

		public const float PI = (float)Math.PI;
	}
}