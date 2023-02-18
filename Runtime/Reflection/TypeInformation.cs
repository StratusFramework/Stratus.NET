using Stratus.Extensions;
using Stratus.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stratus.Reflection
{
	/// <summary>
	/// Stores reflection information about a given type
	/// </summary>
	public class TypeInformation
	{
		public Type type { get; }
		public Lazy<string[]> subclassNames { get; }
		public Lazy<Type[]> subclasses { get; }
		public FieldInfo[] fields => _fields.Value;
		public Lazy<FieldInfo[]> _fields { get; }
		public Lazy<PropertyInfo[]> properties { get; }
		public Lazy<MethodInfo[]> methods { get; }
		public Dictionary<string, FieldInfo> fieldsByName => _fieldsByName.Value;
		public Lazy<Dictionary<string, FieldInfo>> _fieldsByName { get; }
		public Lazy<Dictionary<string, PropertyInfo>> propertiesByName { get; }
		public Lazy<Dictionary<string, MethodInfo>> methodsByName { get; }

		public const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

		public TypeInformation(Type type)
		{
			this.type = type;
			this._fields = new Lazy<FieldInfo[]>(() => type.GetFields(flags));
			this._fieldsByName = new Lazy<Dictionary<string, FieldInfo>>(() => fields.ToDictionary((x) => x.Name, false));
			this.methods = new Lazy<MethodInfo[]>(() => type.GetMethods(flags));
			this.methodsByName = new Lazy<Dictionary<string, MethodInfo>>(() => methods.Value.ToDictionary((x) => x.Name, false));
			this.properties = new Lazy<PropertyInfo[]>(() => type.GetProperties(flags));
			this.propertiesByName = new Lazy<Dictionary<string, PropertyInfo>>(() => properties.Value.ToDictionary((x) => x.Name, false));
			this.subclasses = new Lazy<Type[]>(() => TypeUtility.SubclassesOf(type, true));
			this.subclassNames = new Lazy<string[]>(() => subclasses.Value.Select(t => t.Name).ToArray());

		}

		public static TypeInformation From(object obj) => new TypeInformation(obj.GetType());
	}

}