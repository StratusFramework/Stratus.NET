using Stratus.Extensions;

using System;

namespace Stratus
{
	/// <summary>
	/// The result of an operation or procedure
	/// </summary>
	public class StratusOperationResult
	{
		public bool valid { get; protected set; }
		public string message { get; protected set; }

		public StratusOperationResult(bool valid, string message)
			: this(valid)
		{
			this.message = message;
		}

		public StratusOperationResult(bool valid)
		{
			this.valid = valid;
		}

		public StratusOperationResult(Exception exception)
		{
			this.valid = false;
			this.message = exception.Message;
		}

		public override string ToString()
		{
			return message != null ? $"{valid} ({message})" : $"{valid}";
		}

		public StratusOperationResult WithMessage(string message)
		{
			if (message.IsValid())
			{
				this.message = message;
			}
			return this;
		}

		public static implicit operator bool(StratusOperationResult result) => result.valid;
		public static implicit operator StratusOperationResult(bool valid) => new StratusOperationResult(valid, null);
	}

	/// <summary>
	/// The result of an operation which returns a value
	/// </summary>
	public class StratusOperationResult<T> : StratusOperationResult
	{
		public T result { get; private set; }

		public StratusOperationResult(bool valid, T value) : base(valid)
		{
			this.result = value;
		}

		public StratusOperationResult(bool valid, T value, string message) : base(valid, message)
		{
			this.result = value;
		}

		public StratusOperationResult(Exception exception) : base(exception)
		{
		}

		public override string ToString()
		{
			if (message.IsNullOrEmpty())
			{
				return $"{valid} ({result})";
			}
			return base.ToString();
		}

		public static implicit operator T(StratusOperationResult<T> result) => result.result;
		public static implicit operator StratusOperationResult<T>(T value) => new StratusOperationResult<T>(true, value);
		public static implicit operator StratusOperationResult<T>(bool valid) => new StratusOperationResult<T>(valid, default, null);
	}
}