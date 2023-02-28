using System;
using System.Numerics;

namespace Stratus.Data
{
	/// <summary>
	/// A pair between a variant an a generic key
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class KeyVariantPair<TKey> where TKey : IComparable
	{
		#region Fields
		/// <summary>
		/// The key used for this variant pair
		/// </summary>
		public TKey key;
		/// <summary>
		/// The variant used by the pair
		/// </summary>
		public Variant value;
		#endregion

		#region Properties
		/// <summary>
		/// The current type for this variant pair
		/// </summary>
		public VariantType type => value.type;
		#endregion

		#region Constructors
		public KeyVariantPair(TKey key)
		{
			this.key= key;
		}
		public KeyVariantPair(TKey key, int value) 
			: this(key)
		{
			this.value = new Variant(value);
		}
		public KeyVariantPair(TKey key, float value) 
		{
			this.value = new Variant(value); 
		}
		public KeyVariantPair(TKey key, bool value) 
		{
			this.value = new Variant(value);
		}
		public KeyVariantPair(TKey key, string value)
		{
			this.value = new Variant(value);
		}
		public KeyVariantPair(TKey key, Vector3 value) 
		{
			this.value = new Variant(value); 
		}
		public KeyVariantPair(TKey key, Variant value) 
		{
			this.value = new Variant(value); 
		}
		public KeyVariantPair(KeyVariantPair<TKey> other)
			: this(other.key)
		{
			value = new Variant(other.value);
		}
		#endregion

		public override string ToString()
		{
			return $"{this.key} ({value})";
		}

		#region Interface
		public TValue Get<TValue>()
		{
			return value.Get<TValue>();
		}

		public object Get() => value.Get();

		public void Set<TValue>(TValue value)
		{
			this.value.Set(value);
		}

		public void Set(object value)
		{
			this.value.Set(value);
		}

		public bool Compare(KeyVariantPair<TKey> other)
		{
			// https://msdn.microsoft.com/en-us/library/system.icomparable(v=vs.110).aspx
			if (this.key.CompareTo(other.key) < 0)
			{
				return false;
			}

			return this.value.Compare(other.value);
		}

		public KeyVariantPair<TKey> Copy()
		{
			return new KeyVariantPair<TKey>(key, value);
		}
		#endregion
	}
}
