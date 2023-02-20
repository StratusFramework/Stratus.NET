using System;

using Newtonsoft.Json;

namespace Stratus.Serialization
{
	public static class JsonSerializationUtility
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