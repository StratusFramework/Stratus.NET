using System;

namespace Stratus.Extensions
{
    public static class StratusFloatExtensions
    {
		/// <summary>
		/// Converts this value to its percentage (dividing by 100)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static float ToPercent(this float f) => (f * 0.01f).Round();

		/// <summary>
		/// Converts this value from a percentage (multiplying by 100)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static float FromPercent(this float f) => (f * 100f).Round();

		/// <summary>
		/// Given a percentage float value, returns the string.
		/// (e.g: 1.5f -> 150.00%)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string FromPercentString(this float f)
		{
			return $"{f.FromPercent():0.00}%";
		}

		/// <summary>
		/// Given a percentage float value rounded, returns the string.
		/// (e.g: 1.5f -> 150%)
		/// </summary>
		/// <param name="f"></param>
		/// <returns></returns>
		public static string FromPercentRoundedString(this float f)
		{
			return $"{f.FromPercent():0}%";
		}

		/// <summary>
		/// Rounds the given float value by N decimal places
		/// </summary>
		public static float Round(this float f, int digits = 2) => (float)System.Math.Round(f, digits, MidpointRounding.AwayFromZero);

		public static bool IsEven(this int value) => value % 2 == 0;
		public static bool IsOdd(this int value) => value % 2 != 0;
	}
}

namespace Stratus
{
	public enum StratusRoundingMethod
	{
		Default,
		Symmetrical,
	}
}