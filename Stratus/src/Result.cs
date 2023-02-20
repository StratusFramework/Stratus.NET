using Stratus.Extensions;

using System;

namespace Stratus
{
	/// <summary>
	/// The result of an operation or procedure
	/// </summary>
	public class Result
	{
		public bool valid { get; protected set; }
		public string message { get; protected set; }

		public Result(bool valid, string message)
			: this(valid)
		{
			this.message = message;
		}

		public Result(bool valid)
		{
			this.valid = valid;
		}

		public Result(Exception exception)
		{
			this.valid = false;
			this.message = exception.Message;
		}

		public override string ToString()
		{
			return message != null ? $"{valid} ({message})" : $"{valid}";
		}

		public Result WithMessage(string message)
		{
			if (message.IsValid())
			{
				this.message = message;
			}
			return this;
		}

		public static implicit operator bool(Result result) => result.valid;
		public static implicit operator Result(bool valid) => new Result(valid, null);
	}

	/// <summary>
	/// The result of an operation which returns a value
	/// </summary>
	public class Result<T> : Result
	{
		public T result { get; private set; }

		public Result(bool valid, T value) : base(valid)
		{
			this.result = value;
		}

		public Result(bool valid, T value, string message) : base(valid, message)
		{
			this.result = value;
		}

		public Result(Exception exception) : base(exception)
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

		public static implicit operator T(Result<T> result) => result.result;
		public static implicit operator Result<T>(T value) => new Result<T>(true, value);
		public static implicit operator Result<T>(bool valid) => new Result<T>(valid, default, null);
	}
}