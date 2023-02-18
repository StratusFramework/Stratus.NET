using Stratus.Extensions;

using System;

namespace Stratus.Serialization
{
	/// <summary>
	/// Base class for serializes without type constraints
	/// </summary>
	public abstract class StratusSerializer
	{
		protected abstract void OnSerialize(object value, string filePath);
		protected abstract object OnDeserialize(string filePath);

		public void Serialize(object data, string filePath)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(filePath));
			}

			OnSerialize(data, filePath);
		}

		public object Deserialize(string filePath)
		{
			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}
			return OnDeserialize(filePath);
		}

		public StratusOperationResult TrySerialize(object data, string filePath)
		{
			try
			{
				Serialize(data, filePath);
			}
			catch (Exception ex)
			{
				return new StratusOperationResult(ex);
			}
			return true;
		}

		public StratusOperationResult TryDeserialize(string filePath, out object data)
		{
			try
			{
				data = Deserialize(filePath);
			}
			catch (Exception ex)
			{
				data = null;
				return new StratusOperationResult(ex);
			}
			return true;
		}
	}

	public interface IStratusSerializer
	{
		void Serialize(object data, string filePath);
		object Deserialize(string filePath);
	}

	/// <summary>
	/// Base class for object serializers with enforced type constraint
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusSerializer<T>
		where T : class, new()
	{
		public Type type { get; } = typeof(T);

		protected abstract void OnSerialize(T value, string filePath);
		protected abstract T OnDeserialize(string filePath);

		public void Serialize(T data, string filePath)
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

		public T Deserialize(string filePath)
		{
			if (filePath.IsNullOrEmpty())
			{
				throw new ArgumentNullException("No file path given");
			}
			return OnDeserialize(filePath);
		}

		public bool TrySerialize(T data, string filePath)
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

		public bool TryDeserialize(string filePath, out T data)
		{
			try
			{
				data = Deserialize(filePath);
			}
			catch (Exception e)
			{
				data = null;
				return false;
			}
			return true;
		}
	}
}