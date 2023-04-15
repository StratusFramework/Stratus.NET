using System;

namespace Stratus.Data
{
	public enum StratusProviderSource
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
		public StratusProviderSource source { get; private set; }
		public T value
		{
			get
			{
				switch (source)
				{
					case StratusProviderSource.Reference:
						return _getter();
					case StratusProviderSource.Value:
						return _value;
				}
				throw new Exception("No value source was set");
			}
		}

		private T _value;
		private Func<T> _getter;

		public bool valid => source != StratusProviderSource.Invalid;

		public ValueProvider(Func<T> getter)
		{
			if (getter == null)
			{
				source = StratusProviderSource.Invalid;
				return;
			}
			_getter = getter;
			source = StratusProviderSource.Reference;
		}

		public ValueProvider(T value)
		{
			if (value == null)
			{
				source = StratusProviderSource.Invalid;
				return;
			}
			_value = value;
			source = StratusProviderSource.Value;
		}

		public static implicit operator ValueProvider<T>(T value) => new ValueProvider<T>(value);
		public static implicit operator ValueProvider<T>(Func<T> getValue) => new ValueProvider<T>(getValue);
	}

	public class PropertyReference<T>
	{
		public T value
		{
			get =>  get();
			set => set(value);
		}

		private Func<T> get;
		private Action<T> set;

		public PropertyReference(Func<T> get, Action<T> set)
		{
			this.get = get;
			this.set = set;
		}
	}
}