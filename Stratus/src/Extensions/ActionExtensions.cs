using System;

namespace Stratus.Extensions
{
	public static class ActionExtensions
	{
		public static T GetValueOrDefault<T>(this Func<T> func, T defauultValue = default)
		{
			return func != null ? func() : defauultValue;
		}
	}
}