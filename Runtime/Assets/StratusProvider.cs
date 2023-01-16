using System;

namespace Stratus
{
	public enum StratusProviderSource
	{
		Invalid,
		Reference,
		Value
	}

	public class StratusProvider<TValue>
	{
		public StratusProviderSource source { get; private set; }
		public TValue value
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

		private TValue _value;
		private Func<TValue> _getter;

		public bool valid => source != StratusProviderSource.Invalid;

		public StratusProvider(Func<TValue> getter)
		{
			if (getter == null)
			{
				this.source = StratusProviderSource.Invalid;
				return;
			}
			this._getter = getter;
			this.source = StratusProviderSource.Reference;
		}

		public StratusProvider(TValue value)
		{
			if (value == null)
			{
				this.source = StratusProviderSource.Invalid;
				return;
			}
			this._value = value;
			this.source = StratusProviderSource.Value;
		}

		public static implicit operator StratusProvider<TValue>(TValue value) => new StratusProvider<TValue>(value);
	}

}