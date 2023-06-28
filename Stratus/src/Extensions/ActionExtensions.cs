using System;

namespace Stratus.Extensions
{
	public static class ActionExtensions
	{
		public static T GetValueOrDefault<T>(this Func<T> func, T defauultValue = default)
		{
			return func != null ? func() : defauultValue;
		}

		public static Action Append(this Action a, Action b)
		{
			return () =>
			{
				a?.Invoke();
				b();
			};
		}

		public static Action Prepend(this Action a, Action b)
		{
			return () =>
			{
				b();
				a?.Invoke();
			};
		}
	}
}