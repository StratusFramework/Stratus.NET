using System;
using System.IO;

using Newtonsoft.Json;

namespace Stratus.Serialization
{
	/// <summary>
	/// A serializer using the JSON format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusJSONSerializer<T> : StratusSerializer<T>
		where T : class, new()
	{
		private JsonSerializerSettings settings = new JsonSerializerSettings();

		protected override T OnDeserialize(string filePath)
		{
			var serialization = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject<T>(serialization, settings);
		}

		protected override void OnSerialize(T value, string filePath)
		{
			var serialization = JsonConvert.SerializeObject(value, settings);
			File.WriteAllText(filePath, serialization);
		}
	}

	/// <summary>
	/// A serializer using the JSON format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusJSONSerializer : StratusSerializer
	{
		private JsonSerializerSettings settings = new JsonSerializerSettings();

		protected override object OnDeserialize(string filePath)
		{
			var serialization = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject(serialization, settings);
		}

		protected override void OnSerialize(object value, string filePath)
		{
			var serialization = JsonConvert.SerializeObject(value, settings);
			File.WriteAllText(filePath, serialization);
		}
	}

	public static class StratusJSONSerializerUtility
	{
		public static string Serialize(object value)
		{
			return JsonConvert.SerializeObject(value);
		}

		public static object Deserialize(string serialization, Type type)
		{
			return JsonConvert.DeserializeObject(serialization, type);
		}

		public static T Deserialize<T>(string serialization)
		{
			return JsonConvert.DeserializeObject<T>(serialization);
		}

	}
}