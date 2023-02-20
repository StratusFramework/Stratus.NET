using System.IO;

using Newtonsoft.Json;

namespace Stratus.Serialization
{
	/// <summary>
	/// A serializer using the JSON format
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class JsonObjectSerializer : ObjectSerializer
	{
		private JsonSerializerSettings settings = new JsonSerializerSettings();

		protected override Result<T> OnDeserialize<T>(string filePath)
		{
			var serialization = File.ReadAllText(filePath);
			return JsonConvert.DeserializeObject<T>(serialization, settings);
		}

		protected override Result OnSerialize<T>(T value, string filePath)
		{
			var serialization = JsonConvert.SerializeObject(value, settings);
			File.WriteAllText(filePath, serialization);
			return true;
		}
	}
}