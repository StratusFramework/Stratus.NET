using System;

namespace Stratus.Data
{
	/// <summary>
	/// A symbol represents a key-value pair where the key is an identifying string
	/// and the value is a variant (which can represent multiple types)
	/// </summary>
	[Serializable]
	public class Symbol : KeyVariantPair<string>
	{
		#region Declarations
		/// <summary>
		/// A reference of a symbol
		/// </summary>
		[Serializable]
		public class Reference
		{
			public string key;
			public VariantType type;

			public Reference()
			{
			}

			public Reference(VariantType type)
			{
				this.type = type;
			}

			public Reference(string key, VariantType type)
			{
				this.key = key;
				this.type = type;
			}
		}
		#endregion

		#region Constructors
		public Symbol(string key) : base(key)
		{
		}
		public Symbol(string key, int value) : base(key, value)
		{
		}
		public Symbol(string key, float value) : base(key, value)
		{
		}
		public Symbol(string key, bool value) : base(key, value)
		{
		}
		public Symbol(string key, string value) : base(key, value)
		{
		}
		public Symbol(string key, Variant value) : base(key, value)
		{
		}
		public Symbol(Symbol other) : base(other)
		{
		}
		#endregion

		#region Interface
		public Reference ToReference()
		{
			return new Reference(key, type);
		}
		#endregion

		#region Static Constructors
		public static Symbol Construct<T>(string key, T value)
		{
			return new Symbol(key, Variant.Make(value));
		}
		#endregion
	}
}
