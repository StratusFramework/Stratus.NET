using System;
using System.Linq;

using Stratus.Extensions;
using Stratus.Serialization;
using Stratus.Types;

namespace Stratus
{
	public class StratusSerializedAsset
	{
		public static readonly Lazy<Type[]> types = new Lazy<Type[]>(() =>
			TypeUtility.TypesWithAttribute<StratusSerializedAssetAttribute>().ToArray());


		public static string ComposeFileName(Type type)
		{
			var attr = type.GetAttribute<StratusSerializedAssetAttribute>();
			return $"{attr.name ?? type.Name}.{attr.extension}";
		}

		public static StratusOperationResult<object> Create(Type type, string filePath, ObjectSerializer serializer)
		{
			var instance = ObjectUtility.Instantiate(type);
			serializer.Serialize(instance, filePath);
			return instance;
		}
	}

	/// <summary>
	/// Marks the class as one that can be serialized by Stratus
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class StratusSerializedAssetAttribute : Attribute
	{
		public string name { get; }
		public string extension { get; }

		public const string defaultExtension = "json";

		public StratusSerializedAssetAttribute()
		{
			extension = defaultExtension;
		}

		public StratusSerializedAssetAttribute(string name, string extension)
		{
			this.name = name;
			this.extension = extension;
		}
	}	
}