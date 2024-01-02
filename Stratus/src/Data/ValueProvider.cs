using System;

namespace Stratus.Data
{
	public enum ProviderSource
	{
		Invalid,
		Reference,
		Value
	}

	public interface IValueProvider<T>
	{
		T value { get; }
	}

	public class ValueProvider<T> : IValueProvider<T>
	{
		public ProviderSource source { get; private set; }
		public T value
		{
			get
			{
				switch (source)
				{
					case ProviderSource.Reference:
						return _getter();
					case ProviderSource.Value:
						return _value;
				}
				throw new Exception("No value source was set");
			}
		}

		private T _value;
		private Func<T> _getter;

		public bool valid => source != ProviderSource.Invalid;

		public ValueProvider(Func<T> getter)
		{
			if (getter == null)
			{
				source = ProviderSource.Invalid;
				return;
			}
			_getter = getter;
			source = ProviderSource.Reference;
		}

		public ValueProvider(T value)
		{
			if (value == null)
			{
				source = ProviderSource.Invalid;
				return;
			}
			_value = value;
			source = ProviderSource.Value;
		}

		public static implicit operator ValueProvider<T>(T value) => new ValueProvider<T>(value);
		public static implicit operator ValueProvider<T>(Func<T> getValue) => new ValueProvider<T>(getValue);
	}
}