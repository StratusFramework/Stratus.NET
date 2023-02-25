using System.Collections.Generic;
using System;
using Stratus.Extensions;
using System.Linq;

namespace Stratus.Types
{
	/// <summary>
	/// Utility for instantiating the implementation type T for a given generic type.
	/// So for an abstract resolver with a single type paramter, this will allow you to instantiate each of the found implementations.
	/// </summary>
	/// <typeparam name="TGeneric"></typeparam>
	public class ImplementationTypeInstancer<T>
	{
		private Lazy<Dictionary<Type, Type[]>> implementations;

		public ImplementationTypeInstancer(Type genericType)
		{
			implementations = new Lazy<Dictionary<Type, Type[]>>(() => TypeUtility.TypeDefinitionParameterMap(genericType));
		}

		/// <summary>
		/// Given a parameter type, returns the implementation class for it
		/// </summary>
		/// <param name="parameterType">A type parameter, such as <see cref="int"/> or <see cref="string"/></param>
		/// <returns></returns>
		public Type Resolve(Type parameterType)
		{
			var actionType = implementations.Value.GetValueOrDefault(parameterType).First();
			return actionType;
		}

		public bool TryResolve(Type parameterType, out T instance)
		{
			instance = default;
			Type implType = Resolve(parameterType);
			if (implType == null)
			{
				return false;
			}
			instance = Instantiate(implType);
			return true;
		}

		public T Instantiate(Type implType, params object[] ctor)
		{
			return (T)ObjectUtility.Instantiate(implType, ctor);
		}
	}
}