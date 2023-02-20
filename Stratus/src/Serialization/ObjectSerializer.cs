using Stratus.Extensions;

using System;

namespace Stratus.Serialization
{
	public interface IObjectSerializer
	{
		void Serialize<T>(T value, string filePath);
		T Deserialize<T>(string filePath);
	}

	/// <summary>
	/// Base class for object serializers with enforced type constraint
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ObjectSerializer : IObjectSerializer
	{
		protected abstract void OnSerialize<T>(T value, string filePath);
		protected abstract T OnDeserialize<T>(string filePath);

		public void Serialize<T>(T data, string filePath)
		{
			if (data == null)
			{
				throw new ArgumentNullException("No data to serialize");
			}

			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}

			OnSerialize(data, filePath);
		}

		public T Deserialize<T>(string filePath)
		{
			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}
			return OnDeserialize<T>(filePath);
		}

		public bool TrySerialize<T>(T data, string filePath)
		{
			try
			{
				Serialize(data, filePath);
			}
			catch (Exception e)
			{
				return false;
			}
			return true;
		}

		public bool TryDeserialize<T>(string filePath, out T data)
		{
			try
			{
				data = Deserialize<T>(filePath);
			}
			catch (Exception e)
			{
				data = default;
				return false;
			}
			return true;
		}
	}
}