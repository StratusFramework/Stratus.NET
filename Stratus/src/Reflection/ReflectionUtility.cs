using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

using Stratus.Extensions;

namespace Stratus.Reflection
{
	public static partial class ReflectionUtility
	{
		public static T GetField<T>(string name, Type type, bool isStatic = true, object instance = null)
		{
			BindingFlags bindflags = isStatic ? BindingFlags.NonPublic | BindingFlags.Static : BindingFlags.NonPublic | BindingFlags.Instance;
			FieldInfo field = type.GetField(name, bindflags);

			return (T)field.GetValue(instance);
		}

		public static void SetField<T>(string name, Type type, T value, bool isStatic = true, object instantce = null)
		{
			BindingFlags bindflags = isStatic ? BindingFlags.NonPublic | BindingFlags.Static : BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
			FieldInfo field = type.GetField(name, bindflags);

			if (instantce != null)
			{
				field = instantce.GetType().GetField(name, bindflags);
			}

			field.SetValue(instantce, value);
		}

		public static T GetProperty<T>(string name, Type type, object instance)
		{
			BindingFlags bindflags = BindingFlags.NonPublic | BindingFlags.Instance;
			PropertyInfo propInfo = type.GetProperty(name, bindflags);

			MethodInfo getAccessor = propInfo.GetGetMethod(true);

			return (T)getAccessor.Invoke(instance, null);
		}

		public static MethodInfo GetReflectedMethod(string name, Type type, bool isStatic = true, object instantce = null)
		{
			BindingFlags bindflags = isStatic ? BindingFlags.NonPublic | BindingFlags.Static : BindingFlags.NonPublic | BindingFlags.Instance;
			MethodInfo method = type.GetMethod(name, bindflags);

			return method;
		}

		/// <summary>
		/// Retrieves the name of this property / field as well as its owning object.
		/// Note: This is quite an expensive call so use sparingly.
		/// </summary>
		/// <param name="varExpr">A lambda expression capturing a reference to a field or property</param>
		/// <returns></returns>
		public static MemberReference GetReference<T>(Expression<Func<T>> varExpr)
		{
			// Slow, probs
			return MemberReference.Construct(varExpr);
		}

		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static class New<T>
		{
			public static readonly Func<T> Instance = Creator();

			private static Func<T> Creator()
			{
				Type t = typeof(T);
				if (t == typeof(string))
				{
					return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();
				}

				if (t.HasDefaultConstructor())
				{
					return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();
				}

				return () => (T)FormatterServices.GetUninitializedObject(t);
			}
		}

		/// <summary>
		/// Finds the most nested object inside of an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static T GetNestedObject<T>(this object obj, string path)
		{
			foreach (string part in path.Split('.'))
			{
				obj = obj.GetFieldOrPropertyValue<T>(part);
			}
			return (T)obj;
		}

		/// <summary>
		/// Gets a property or a field of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the field/property.</typeparam>
		/// <param name="obj">Object the field/property should be found in.</param>
		/// <param name="name">Name of the field/property.</param>
		/// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
		/// <returns>The field/property.</returns>
		public static T GetFieldOrPropertyValue<T>(this object obj, string memberName, bool includeAllBases = false,
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(memberName, bindingFlags);
			if (field != null)
			{
				return (T)field.GetValue(obj);
			}

			PropertyInfo property = obj.GetType().GetProperty(memberName, bindingFlags);
			if (property != null)
			{
				return (T)property.GetValue(obj, null);
			}

			if (includeAllBases)
			{
				Type objectType = obj.GetType();
				foreach (Type type in objectType.GetBaseClassesAndInterfaces())
				{
					field = type.GetField(memberName, bindingFlags);
					if (field != null)
					{
						return (T)field.GetValue(obj);
					}

					property = type.GetProperty(memberName, bindingFlags);
					if (property != null)
					{
						return (T)property.GetValue(obj, null);
					}
				}
			}

			throw new PropertyOrFieldNotFoundException($"Couldn't find a field or property with the name of {memberName} inside of the object {obj.GetType().Name}");
		}

		/// <summary>
		/// Gets a field inside of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the field.</typeparam>
		/// <param name="obj">Object the field should be found in.</param>
		/// <param name="name">Name of the field.</param>
		/// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>
		/// <returns>The field.</returns>
		public static T GetField<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			if (field != null)
			{
				return (T)field.GetValue(obj);
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Gets a property inside of an object by a name.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="obj">Object the property should be found in.</param>
		/// <param name="name">Name of the property.</param>
		/// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
		/// <returns>The property.</returns>
		public static T GetProperty<T>(this object obj, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
			if (property != null)
			{
				return (T)property.GetValue(obj, null);
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Sets a field or a property inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the field/property.</typeparam>
		/// <param name="obj">Object contaning the field/property.</param>
		/// <param name="name">Name of the field/property.</param>
		/// <param name="value">New value of the field/property.</param>
		/// <param name="bindingFlags">Filters for the field/property it can find. (optional)</param>
		public static bool SetFieldOrPropertyValue<T>(this object obj, string memberName, T value, bool includeAllBases = false,
			BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(memberName, bindings);
			if (field != null)
			{
				field.SetValue(obj, value);
				return true;
			}

			PropertyInfo property = obj.GetType().GetProperty(memberName, bindings);
			if (property != null)
			{
				property.SetValue(obj, value, null);
				return true;
			}

			if (includeAllBases)
			{
				Type objectType = obj.GetType();
				foreach (Type type in objectType.GetBaseClassesAndInterfaces())
				{
					field = type.GetField(memberName, bindings);
					if (field != null)
					{
						field.SetValue(obj, value);
						return true;
					}

					property = type.GetProperty(memberName, bindings);
					if (property != null)
					{
						property.SetValue(obj, value, null);
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Sets a field inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the field.</typeparam>
		/// <param name="obj">Object contaning the field.</param>
		/// <param name="name">Name of the field.</param>
		/// <param name="value">New value of the field.</param>
		/// <param name="bindingFlags">Filters for the fields it can find. (optional)</param>>
		public static void SetField<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			FieldInfo field = obj.GetType().GetField(name, bindingFlags);
			if (field != null)
			{
				field.SetValue(obj, value);
				return;
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new FieldNotFoundException("Couldn't find a field with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Sets a property inside of an object by name.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="obj">Object contaning the property.</param>
		/// <param name="name">Name of the property.</param>
		/// <param name="value">New value of the property.</param>
		/// <param name="bindingFlags">Filters for the properties it can find. (optional)</param>
		public static void SetProperty<T>(this object obj, string name, T value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Try getting the field and returning it.
			PropertyInfo property = obj.GetType().GetProperty(name, bindingFlags);
			if (property != null)
			{
				property.SetValue(obj, value, null);
				return;
			}

			// If a field couldn't be found. Throw an exception about it.
			throw new PropertyNotFoundException("Couldn't find a property with the name of '" + name + "' inside of the object '" + obj.GetType().Name + "'");
		}

		/// <summary>
		/// Gets all the properties and fields in obj of type T.
		/// </summary>
		/// <typeparam name="T">The type of the fields/properties.</typeparam>
		/// <param name="obj">Object to find the fields/properties in.</param>
		/// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
		/// <returns>The fields/properties found.</returns>
		public static IEnumerable<T> GetAllFieldsOrProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get the fields and the properties in the object.
			T[] fields = obj.GetAllFields<T>(bindingFlags).ToArray();
			T[] properties = obj.GetAllProperties<T>(bindingFlags).ToArray();

			// Only return the fields if fields were found.
			if (fields != null && fields.Length != 0)
			{
				// Loop through the fields and return each one.
				for (int i = 0; i < fields.Length; i++)
				{
					yield return fields[i];
				}
			}

			// Only return the properties if properties were found.
			if (properties != null && properties.Length != 0)
			{
				// Loop through the properties and return each one if they have the right type.
				for (int i = 0; i < properties.Length; i++)
				{
					yield return properties[i];
				}
			}
		}

		/// <summary>
		/// Gets all the fields in obj of type T.
		/// </summary>
		/// <typeparam name="T">Type of the fields allowed.</typeparam>
		/// <param name="obj">Object to find the fields in.</param>
		/// <param name="bindingFlags">Filters of the fields allowed.</param>
		/// <returns>The fields found.</returns>
		public static IEnumerable<T> GetAllFields<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			FieldInfo[] fields = obj.GetType().GetFields(bindingFlags);

			// If there are no properties, break.
			if (fields == null || fields.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < fields.Length; i++)
			{
				object currentValue = fields[i].GetValue(obj);

				if (currentValue.GetType() == typeof(T))
				{
					yield return (T)currentValue;
				}
			}
		}

		/// <summary>
		/// Gets all the properties in obj of type T.
		/// </summary>
		/// <typeparam name="T">Type of the properties allowed.</typeparam>
		/// <param name="obj">Object to find the properties in.</param>
		/// <param name="bindingFlags">Filters of the properties allowed.</param>
		/// <returns>The properties found.</returns>
		public static IEnumerable<T> GetAllProperties<T>(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			// Get all the properties.
			PropertyInfo[] properties = obj.GetType().GetProperties(bindingFlags);

			// If there are no properties, break.
			if (properties == null || properties.Length == 0)
			{
				yield break;
			}

			// If there are properties in the array, return each element.
			for (int i = 0; i < properties.Length; i++)
			{
				object currentValue = properties[i].GetValue(obj, null);

				if (currentValue.GetType() == typeof(T))
				{
					yield return (T)currentValue;
				}
			}
		}

		/// <summary>
		/// Gets all the properties and fields in object
		/// </summary>
		/// <param name="obj">Object to find the fields/properties in.</param>
		/// <param name="bindingFlags">Filters for the types of fields/properties that can be found.</param>
		/// <returns>The fields/properties found.</returns>
		public static IEnumerable<MemberReference> GetAllFieldsOrProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			return obj.GetAllFields(bindingFlags).Concat(obj.GetAllProperties(bindingFlags));
		}

		/// <summary>
		/// Gets all the fields in the object
		/// </summary>
		/// <param name="obj">Object to find the fields in.</param>
		/// <param name="bindingFlags">Filters of the fields allowed.</param>
		/// <returns>The fields found.</returns>
		public static IEnumerable<MemberReference> GetAllFields(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			return obj.GetType()
				.GetFields(bindingFlags)
				.Select(f => new MemberReference(f, obj));
		}

		/// <summary>
		/// Gets all the properties in obj.
		/// </summary>
		/// <param name="obj">Object to find the properties in.</param>
		/// <param name="bindingFlags">Filters of the properties allowed.</param>
		/// <returns>The properties found.</returns>
		public static IEnumerable<MemberReference> GetAllProperties(this object obj, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			return obj.GetType()
				.GetProperties(bindingFlags)
				.Select(p => new MemberReference(p, obj));
		}

		/// <summary>
		/// Returns all fields that are being serialized by Unity
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static FieldInfo[] GetSerializedFields(this Type type)
		{
			return type.GetFields();
		}

		/// <summary>
		/// Finds an attribute of the specified type inside the class
		/// </summary>
		/// <typeparam name="AttributeType">The attribute class which was used in the class declaration</typeparam>
		/// <param name="type">The type of the class that was declared with the attribute</param>
		/// <returns></returns>
		public static Dictionary<Type, Attribute> MapAttributes(MemberInfo memberInfo)
		{
			Attribute[] attributes = (Attribute[])memberInfo.GetCustomAttributes(typeof(Attribute), true);
			Dictionary<Type, Attribute> attributeMap = new Dictionary<Type, Attribute>();
			if (attributes.Length > 0)
			{
				attributeMap.AddRangeUnique((Attribute attr) => attr.GetType(), attributes);
				return attributeMap;
			}
			return null;
		}
	}
}

