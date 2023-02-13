using System.Collections;
using System.Collections.Generic;
using System;
using Stratus.Extensions;

namespace Stratus
{
	/// <summary>
	/// Used for managing default instances of the subclasses of a given class
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusTypeInstancer<T> where T : class
	{
		private Lazy<Type[]> _types;
		private Lazy<Dictionary<Type, T>> _instancesByType;
		private Lazy<Dictionary<string, T>> _instancesByName;

		public Type baseType { get; private set; }
		public Type[] types => _types.Value;

		public StratusTypeInstancer(params object[] ctor)
		{
			baseType = typeof(T);
			_types = new Lazy<Type[]>(() => Utilities.StratusTypeUtility.SubclassesOf<T>());
			_instancesByType = new Lazy<Dictionary<Type, T>>(() => types.ToDictionaryFromKey((Type t) => (T)Activator.CreateInstance(t, ctor)));
			_instancesByName = new Lazy<Dictionary<string, T>>(() => _instancesByType.Value.Values.ToDictionary(i => i.GetType().Name));
		}

		public IEnumerable<T> GetAll() => _instancesByType.Value.Values;

		public U Get<U>() where U : T
		{
			return (U)_instancesByType.Value.GetValueOrDefault(typeof(U));
		}

		public T Get(Type type)
		{
			return _instancesByType.Value.GetValueOrDefault(type);
		}

		public T Get(string name)
		{
			return _instancesByName.Value.GetValueOrDefault(name);
		}
	}

	public class ImplementationTypeInstancer<T> where T : class
	{

	}
}