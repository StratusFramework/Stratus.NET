using Stratus.Extensions;

using System;

namespace Stratus.Serialization
{
	public interface IObjectSerializer
	{
		Result Serialize<T>(T value, string filePath);
		Result<T> Deserialize<T>(string filePath);
	}

	/// <summary>
	/// Base class for object serializers with enforced type constraint
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ObjectSerializer : IObjectSerializer
	{
		protected abstract Result OnSerialize<T>(T value, string filePath);
		protected abstract Result<T> OnDeserialize<T>(string filePath);

		public Result Serialize<T>(T data, string filePath)
		{
			if (data == null)
			{
				return new Result(false, "No data to serialize");
			}

			if (filePath.IsNullOrEmpty())
			{
				return new Result(false, "No file path given");
			}

			return OnSerialize(data, filePath);
		}

		public Result<T> Deserialize<T>(string filePath)
		{
			if (filePath.IsNullOrEmpty())
			{
				return new Result<T>(false, default, "No file path given");
			}
			return OnDeserialize<T>(filePath);
		}

		public Result TrySerialize<T>(T data, string filePath)
		{
			try
			{
				return Serialize(data, filePath);
			}
			catch (Exception ex)
			{
				return new Result(ex);
			};
		}

		public Result TryDeserialize<T>(string filePath, out T data)
		{
			try
			{
				data = Deserialize<T>(filePath);
				return true;
			}
			catch (Exception ex)
			{
				data = default;
				return new Result(ex);
			}
		}
	}
}