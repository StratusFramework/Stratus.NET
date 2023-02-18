using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Stratus.Types
{
	/// <summary>
	/// Utility methods for instantiating and managing <see cref="object"/>s from their <see cref="Type"/>
	/// </summary>
	public static class ObjectUtility
	{
		public static object Instantiate(Type t)
		{
			if (t == typeof(string))
			{
				return Expression.Lambda<Func<string>>(Expression.Constant(string.Empty)).Compile();
			}

			if (t.HasDefaultConstructor())
			{
				return Activator.CreateInstance(t);
			}

			return FormatterServices.GetUninitializedObject(t);
		}

		public static T Instantiate<T>()
		{
			Type t = typeof(T);
			if (t == typeof(string))
			{
				return (T)(object)Expression.Lambda<Func<string>>(Expression.Constant(string.Empty)).Compile();
			}

			if (t.HasDefaultConstructor())
			{
				return (T)Activator.CreateInstance(t);
			}

			return (T)FormatterServices.GetUninitializedObject(t);
		}

		public static object Instantiate(Type type, params object[] parameters)
		{
			return Activator.CreateInstance(type, parameters);
		}

		public static T Instantiate<T>(params object[] parameters)
		{
			return (T)Instantiate(typeof(T), parameters);
		}

		public static IEnumerable<object> InstantiateRange(params Type[] types)
		{
			return types.Select(t => Instantiate(t));
		}

		public static IEnumerable<T> InstantiateRange<T>(params Type[] types)
		{
			return InstantiateRange(types).Select(obj => (T)obj);
		}
	}
}