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
		//--------------------------------------------------------------------/
		// Constructors
		//--------------------------------------------------------------------/
		public Symbol(string key, int value) : base(key, value) { }
		public Symbol(string key, float value) : base(key, value) { }
		public Symbol(string key, bool value) : base(key, value) { }
		public Symbol(string key, string value) : base(key, value) { }
		public Symbol(string key, Variant value) : base(key, value) { }
		public Symbol(Symbol other) : base(other) { }

		//--------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------/
		/// <summary>
		/// Constructs a symbol with the given key and value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Symbol Construct<T>(string key, T value)
		{
			return new Symbol(key, Variant.Make(value));
		}

		//--------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------/
		/// <summary>
		/// Constructs a reference to this symbol
		/// </summary>
		/// <returns></returns>
		public Reference reference => new Reference(key, type);

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
	}
}
