//using Stratus.OdinSerializer;

using System;
using System.IO;

//using UnityEngine;
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
			//byte[] serialization = File.ReadAllBytes(filePath);			
			//return SerializationUtility.DeserializeValue<T>(serialization, DataFormat.JSON);
		}

		protected override void OnSerialize(T value, string filePath)
		{
			var serialization = JsonConvert.SerializeObject(value, settings);
			File.WriteAllText(filePath, serialization);	

			//var serialization = serializer.Serialize(value);
			//byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.JSON);
			//File.WriteAllBytes(filePath, serialization);
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

			//byte[] serialization = File.ReadAllBytes(filePath);
			//return SerializationUtility.DeserializeValueWeak(serialization, DataFormat.JSON);
		}

		protected override void OnSerialize(object value, string filePath)
		{
			var serialization = JsonConvert.SerializeObject(value, settings);
			File.WriteAllText(filePath, serialization);

			//byte[] serialization = SerializationUtility.SerializeValue(value, DataFormat.JSON);
			//File.WriteAllBytes(filePath, serialization);
		}
	}

	public static class StratusJSONSerializerUtility
	{
		public static string Serialize(object value)
		{
			return JsonConvert.SerializeObject(value);
			//return JsonUtility.ToJson(value);
		}

		public static object Deserialize(string serialization, Type type)
		{
			return JsonConvert.DeserializeObject(serialization, type);
		}

		public static T Deserialize<T>(string serialization)
		{
			return JsonConvert.DeserializeObject<T>(serialization);
			//return JsonUtility.FromJson<T>(serialization);
		}

	}
}