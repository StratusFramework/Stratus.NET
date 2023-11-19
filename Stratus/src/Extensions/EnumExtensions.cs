using System;

namespace Stratus.Extensions
{
	public static class EnumExtensions
	{
		public static bool Equals<TEnum>(this TEnum source, params TEnum[] values)
			where TEnum : Enum
		{
			foreach(var value in values)
			{
				if (source.Equals(value))
				{
					return true;
				}
			}
			return false;
		}
	}
}