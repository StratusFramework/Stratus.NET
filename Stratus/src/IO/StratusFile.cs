using Stratus.Serialization;

using System.IO;

namespace Stratus
{
	public class StratusFile
	{
		public FileInfo file { get; protected set; }
		public string filePath => file.FullName;
		public bool valid => file != null;
		public bool exists => file.Exists;
		public static string temporaryDirectoryPath => Path.GetTempPath();
	}

	public class StratusFile<T> : StratusFile
		where T : class
	{
		public T data { get; private set; }
		public bool canSerialize => serializer != null;
		public ObjectSerializer serializer { get; private set; }

		#region Serialization
		public Result Serialize(T data)
		{
			this.data = data;
			return Serialize();
		}

		public Result Serialize()
		{
			if (!valid)
			{
				return new Result(false, "No file path has been set");
			}

			return serializer.TrySerialize(data, file.FullName);
		}

		public Result<T> Deserialize()
		{
			if (!valid)
			{
				return new Result<T>(false, null, "No file path has been set");
			}

			T _data;
			var deserialization = serializer.TryDeserialize(filePath, out _data);
			if (_data != null)
			{
				data = (T)_data;
			}
			return new Result<T>(data != null, data);
		}

		public StratusFile WithJson()
		{
			WithSerializer(new JsonObjectSerializer());
			return this;
		}

		public void WithSerializer(ObjectSerializer serializer)
		{
			this.serializer = serializer;
		}
		#endregion

		/// <summary>
		/// Sets the path for the file based on the temporary directory
		/// </summary>
		/// <param name="fileName">The name of the file. If none is set, will generate an unique one.</param>
		public StratusFile<T> AtTemporaryPath(string fileName = null)
		{
			var filePath = fileName != null
				? Path.Combine(temporaryDirectoryPath, fileName)
			: Path.GetTempFileName();
			return At(filePath);
		}

		/// <summary>
		/// Sets the path for the file
		/// </summary>
		/// <param name="filePath">An absolute file path</param>
		public StratusFile<T> At(string filePath)
		{
			file = new FileInfo(filePath);
			return this;
		}
	}
}