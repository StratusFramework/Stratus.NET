using Stratus.Extensions;
using Stratus.Reflection;
//using Stratus.OdinSerializer.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stratus.Utilities
{
	/// <summary>
	/// Utility methods for <see cref="Type"/>
	/// </summary>
	public static class StratusTypeUtility
	{
		#region Properties
		private static Dictionary<Type, Type[]> genericTypeDefinitions { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, string[]> subclassNames { get; set; } = new Dictionary<Type, string[]>();
		private static Dictionary<Type, Type[]> _subClasses { get; set; } = new Dictionary<Type, Type[]>();
		private static Dictionary<Type, Dictionary<Type, Type[]>> interfacesImplementationsByBaseType { get; set; } = new Dictionary<Type, Dictionary<Type, Type[]>>();
		private static Dictionary<Type, Type[]> interfaceImplementations { get; set; } = new Dictionary<Type, Type[]>();

		private static Lazy<List<Type>> classes = new Lazy<List<Type>>(() =>
			allAssemblies.SelectMany(a => a.GetTypes()).ToList());

		private static Assembly[] _allAssemblies;
		public static Assembly[] allAssemblies
		{
			get
			{
				if (_allAssemblies == null)
				{
					_allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				}

				return _allAssemblies;
			}
		}

		private static Lazy<Dictionary<Type, List<Type>>> typesWithAttribute
			= new Lazy<Dictionary<Type, List<Type>>>(() =>
			{
				Dictionary<Type, List<Type>> result = new Dictionary<Type, List<Type>>();
				foreach (var type in classes.Value)
				{
					var attributes = type.GetCustomAttributes();
					foreach (var attr in attributes)
					{
						var attrType = attr.GetType();
						if (!result.ContainsKey(attr.GetType()))
						{
							result.Add(attrType, new List<Type>());
						}
						result[attr.GetType()].Add(type);
					}
				}
				return result;
			});

		private static Lazy<Dictionary<Assembly, Type[]>> typesByAssembly = new Lazy<Dictionary<Assembly, Type[]>>
			(() => allAssemblies.ToDictionary(a => a.GetTypes()));

		private static Lazy<Type[]> _types = new Lazy<Type[]>(() =>
			typesByAssembly.Value.SelectMany(a => a.Value).ToArray());

		//private static Lazy<Dictionary<Type, List<Type>>> typesWithAttribute =
		//	new Lazy<Dictionary<Type, List<Type>>>(() =>
		//	{
		//		Dictionary<Type, List<Type>> result = new Dictionary<Type, List<Type>>();

		//		return result;
		//	});

		public static Type[] types => _types.Value;

		#endregion

		#region Methods
		public static Type[] GetTypesFromAssembly(Assembly assembly)
			=> typesByAssembly.Value.GetValueOrDefault(assembly);

		public static Type[] GetAllTypes() => _types.Value;

		/// <summary>
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] SubclassNames(Type baseType, bool includeAbstract = false)
		{
			string[] typeNames;
			if (!subclassNames.ContainsKey(baseType))
			{
				Type[] types = SubclassesOf(baseType, includeAbstract);
				//Type[] types = Assembly.GetAssembly(baseType).GetTypes();
				typeNames = (from Type type in types where type.IsSubclassOf(baseType) && !type.IsAbstract select type.Name).ToArray();
				subclassNames.Add(baseType, typeNames);
			}

			return subclassNames[baseType];
		}

		/// <summary>
		/// Get the name of all classes derived from the given one
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static string[] SubclassNames<T>(bool includeAbstract = false)
		{
			Type baseType = typeof(T);
			return SubclassNames(baseType, includeAbstract);
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] SubclassesOf<TClass>(bool includeAbstract = false)
		{
			return SubclassesOf(typeof(TClass), includeAbstract);
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] SubclassesOf(Type type, bool includeAbstract = false)
		{
			// Done the first time this type is queried, in order to cache
			if (!_subClasses.ContainsKey(type))
			{
				List<Type> types = new List<Type>();
				foreach (Assembly assembly in allAssemblies)
				{
					Type[] subclasses = typesByAssembly.Value[assembly]
						.Where(t =>
						{
							if (type.IsGenericType)
							{
								if (t.BaseType == type)
								{
									return true;
								}

								Type current = t.BaseType;
								while (current != null && current.IsGenericType)
								{
									if (current.GetGenericTypeDefinition() == type)
									{
										return true;
									}
									current = current.BaseType;
								}
								return false;
							}

							return t.IsSubclassOf(type);
						}).ToArray();

					types.AddRange(subclasses);
				}
				_subClasses.Add(type, types.ToArray());
			}

			return includeAbstract
				? _subClasses[type]
				: _subClasses[type].Where(t => !t.IsAbstract).ToArray();
		}

		/// <summary>
		/// For a given generic type, returns all the types that use its definition,
		/// mapped by the parameter. For example:
		/// [int : DerivedInt1, DerivedInt2>
		/// [bool : DerivedBool1, DerivedBool2]
		/// </summary>
		public static Dictionary<Type, Type[]> TypeDefinitionParameterMap(Type baseType, bool recursive = true)
		{
			if (!baseType.IsGenericType)
			{
				throw new ArgumentException($"The given type {baseType} is not generic!");
			}
			Dictionary<Type, List<Type>> result = new Dictionary<Type, List<Type>>();
			Type[] definitions = TypesDefinedFromGeneric(baseType);
			foreach (var type in definitions)
			{
				if (type.IsAbstract)
				{
					var derived = TypeDefinitionParameterMap(type, recursive);
					result.AddFrom(derived);
				}

				var typeArgs = type.BaseType.GenericTypeArguments;
				if (typeArgs.Length == 1)
				{
					Type paramType = typeArgs[0];
					if (!result.ContainsKey(paramType))
					{
						result.Add(paramType, new List<Type>());
					}
					result[paramType].Add(type);
				}
			}
			return result.ToDictionary(kp => kp.Key, kp => kp.Value.ToArray());
		}

		/// <summary>
		/// For a given generic type, returns all the types that use its definition.
		/// </summary>
		public static Type[] TypesDefinedFromGeneric(Type genericType)
		{
			return SubclassesOf(genericType);
		}

		/// <summary>
		/// Gets all the types that have at least one attribute in the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static IEnumerable<Type> TypesWithAttribute(Type attribute)
		{
			return typesWithAttribute.Value[attribute];
		}

		public static IEnumerable<Type> TypesWithAttribute<TAttribute>()
			where TAttribute : Attribute
			=> TypesWithAttribute(typeof(TAttribute));


		private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

		/// <summary>
		/// Attempts to resolve the <see cref="Type"/> from the given type name
		/// </summary>
		public static Type ResolveType(string typeName)
		{
			if (!typeMap.TryGetValue(typeName, out Type type))
			{
				type = !string.IsNullOrEmpty(typeName) ? Type.GetType(typeName) : null;
				typeMap[typeName] = type;
			}
			return type;
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <typeparam name="ClassType"></typeparam>
		/// <param name="includeAbstract"></param>
		/// <returns></returns>
		public static Type[] InterfaceImplementations(Type baseType, Type interfaceType, bool includeAbstract = false)
		{
			// First, map into the selected interface type
			if (!interfacesImplementationsByBaseType.ContainsKey(interfaceType))
			{
				interfacesImplementationsByBaseType.Add(interfaceType, new Dictionary<Type, Type[]>());
			}

			// Now for a selected interface type, find all implementations that derive from the base type
			if (!interfacesImplementationsByBaseType[interfaceType].ContainsKey(baseType))
			{
				Type[] implementedTypes = (from Type t
										   in SubclassesOf(baseType)
										   where t.IsSubclassOf(baseType) && t.GetInterfaces().Contains((interfaceType))
										   select t).ToArray();
				interfacesImplementationsByBaseType[interfaceType].Add(baseType, implementedTypes);
			}

			return interfacesImplementationsByBaseType[interfaceType][baseType];
		}

		/// <summary>
		/// Get an array of types of all the classes derived from the given one
		/// </summary>
		/// <returns></returns>
		public static Type[] GetInterfaces(Type interfaceType, bool includeAbstract = false)
		{
			// First, map into the selected interface type
			if (!interfaceImplementations.ContainsKey(interfaceType))
			{
				var _types = typesByAssembly.Value
					.SelectMany(t => t.Value)
					.Where(t => t.GetInterfaces().Contains((interfaceType)) && (t.IsAbstract == includeAbstract))
					.ToArray();

				interfaceImplementations.Add(interfaceType, _types.ToArray());
			}

			return interfaceImplementations[interfaceType];
		}

		/// <summary>
		/// Gets an array of all the types that implement generic type for a given type T
		/// </summary>
		public static Type[] ImplementationsOf(Type genericType, Type typeArgument)
		{
			var types = SubclassesOf(genericType);
			return types.Where(t =>
			{
				if (t.IsGenericType)
				{
					return false;
				}

				Type baseType = t.BaseType;
				while (!baseType.IsGenericType)
				{
					baseType = baseType.BaseType;
				}

				return baseType.GenericTypeArguments[0] == typeArgument;
			}).ToArray();
		}
		#endregion

		/// <summary>
		/// Finds the element type of the given collection
		/// </summary>
		[System.Diagnostics.DebuggerHidden]
		public static Type GetElementType(this ICollection collection)
		{
			PropertyInfo propertyInfo = collection == null ? null : collection.GetType().GetProperty("Item");
			return propertyInfo == null ? null : propertyInfo.PropertyType;
		}

		public static Type GetPrivateType(string name, Type source)
		{
			Assembly assembly = source.Assembly;
			return assembly.GetType(name);
		}

		public static Type GetPrivateType(string fqName)
		{
			return Type.GetType(fqName);
		}

		public static StratusTypeInfo TypeInfo<T>() => new StratusTypeInfo(typeof(T));
	}

}