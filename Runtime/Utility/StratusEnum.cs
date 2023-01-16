using Stratus.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stratus
{
	public static class StratusEnum
	{
		private static Dictionary<Type, string[]> enumDisplayNames { get; set; } = new Dictionary<Type, string[]>();
		private static Dictionary<Type, Array> enumValues { get; set; } = new Dictionary<Type, Array>();
		private static Dictionary<Type, Enum[]> _enumValues { get; set; } = new Dictionary<Type, Enum[]>();

		public static T[] Values<T>() where T : Enum
		{
			return Values(typeof(T)).Cast<T>().ToArray();
			//return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
		}

		public static Enum[] Values(Type enumType)
		{
			return _enumValues.GetValueOrGenerate(enumType, t => Enum.GetValues(t).Cast<Enum>().ToArray());
		}

		public static Array ValuesArray(Type enumType)
		{
			return enumValues.GetValueOrGenerate(enumType, Enum.GetValues);
		}

		public static T Value<T>(int index) where T : Enum
		{
			return (T)Value(typeof(T), index);
		}

		public static Enum Value(Type enumType, int index)
		{
			return (Enum)ValuesArray(enumType).GetValue(index);
		}

		public static string[] Names<T>() where T : Enum
		{
			return Names(typeof(T));
		}

		public static string[] Names(Type enumType)
		{
			return enumDisplayNames.GetValueOrGenerate(enumType, Enum.GetNames);
		}

		/// <summary>
		/// </summary>
		/// <returns>All the flags of the given enum value. If there's no flags, returns itself.</returns>
		public static IEnumerable<TEnum> Flags<TEnum>(TEnum _value) where TEnum : Enum
		{
			ulong flag = 1;
			foreach (var value in Enum.GetValues(_value.GetType()).Cast<TEnum>())
			{
				ulong bits = Convert.ToUInt64(value);
				while (flag < bits)
				{
					flag <<= 1;
				}

				if (flag == bits && _value.HasFlag(value))
				{
					yield return value;
				}
			}
		}

		public static bool HasFlags<TEnum>(TEnum value) where TEnum : Enum
		{
			return Flags(value).Count() > 1;
		}

		public static Dictionary<TEnum, TValue> Dictionary<TEnum, TValue>(TValue defaultValue = default)
			where TEnum : Enum
		{
			Dictionary<TEnum, TValue> result = new Dictionary<TEnum, TValue>();
			Values<TEnum>().ForEach(e => result.Add(e, defaultValue));
			return result;
		}


		// Maps the attributes of an enum
		private static Dictionary<Type, Dictionary<Enum, Dictionary<Type, Attribute>>>
			enumValueAttributesByType = new Dictionary<Type, Dictionary<Enum, Dictionary<Type, Attribute>>>();

		// Cached version, ho!
		public static TAttribute GetAttributeCached<TEnum, TAttribute>(this TEnum value)
			where TEnum : Enum
			where TAttribute : Attribute
		{
			Type enumType = typeof(TEnum);
			Type attrType = typeof(TAttribute);
			TryAddToAttributeCache<TEnum>(enumType);
			return enumValueAttributesByType[enumType][value][attrType] as TAttribute;
		}

		public static Dictionary<TEnum, TAttribute> GetAttributeMap<TEnum, TAttribute>()
			where TEnum : Enum
			where TAttribute : Attribute
		{
			Type enumType = typeof(TEnum);
			Type attrType = typeof(TAttribute);
			TryAddToAttributeCache<TEnum>(enumType);
			Dictionary<TEnum, TAttribute> map = new Dictionary<TEnum, TAttribute>();
			foreach (var kvp in enumValueAttributesByType[enumType])
			{
				TEnum value = (TEnum)kvp.Key;
				if (!kvp.Value.ContainsKey(attrType))
				{
					continue;
				}
				TAttribute attr = kvp.Value[attrType] as TAttribute;
				map.Add(value, attr);
			}
			return map;
		}

		private static void TryAddToAttributeCache<TEnum>(Type enumType) where TEnum : Enum
		{
			if (!enumValueAttributesByType.ContainsKey(enumType))
			{
				// Add the enum type
				enumValueAttributesByType.Add(enumType, new Dictionary<Enum, Dictionary<Type, Attribute>>());
				// Add all its values and their attributes
				foreach (var v in Values<TEnum>())
				{
					// Add the value...
					enumValueAttributesByType[enumType].Add(v, new Dictionary<Type, Attribute>());
					// and its attributes
					var valueMemberInfo = enumType.GetMember(v.ToString()).First();
					foreach (var attr in valueMemberInfo.GetCustomAttributes())
					{
						enumValueAttributesByType[enumType][v].Add(attr.GetType(), attr);
					}
				}
			}
		}

		public static TEnum GetHighestValue<TEnum>(params TEnum[] values)
		{
			return values.Max();
		}

		public static bool GreaterOrEqualThan<TEnum>(TEnum first, TEnum second)
			where TEnum : Enum
		{
			return ((int)(object)first) >= ((int)(object)second);
		}

		public static TEnum Increase<TEnum>(TEnum value)
			where TEnum : Enum
		{
			int count  = StratusEnum.Values<TEnum>().Length;
			return FromInteger<TEnum>(Math.Min(count - 1, ToInteger(value) + 1));
		}

		public static TEnum Decrease<TEnum>(TEnum value)
			where TEnum : Enum
		{
			return FromInteger<TEnum>(Math.Max(0, ToInteger(value) - 1));
		}

		public static int ToInteger<TEnum>(TEnum value)
			where TEnum : Enum
		{
			return Convert.ToInt32(value);
		}

		public static TEnum FromInteger<TEnum>(int value)
			where TEnum : Enum
		{
			return Values<TEnum>()[value];
		}
	}
}