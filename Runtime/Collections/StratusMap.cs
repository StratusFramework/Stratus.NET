using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using Stratus.Extensions;
using Stratus.Serialization;

namespace Stratus.Collections
{
	public interface IReadOnlyStratusMap<TKey, TValue> : IEnumerable<TValue>
	{
		int Count { get; }
		bool Contains(TKey key);
		bool Contains(TValue value);
		TValue Get(TKey key);
	}

	/// <summary>
	/// A collection that serializes the <see cref="List{TValue}"/>, generates the <see cref="Dictionary{TKey, TValue}"/> during runtime
	/// </summary>
	/// <typeparam name="TKey">The key type used for the dictionary</typeparam>
	/// <typeparam name="TValue">The value type</typeparam>
	public abstract class StratusMap<TKey, TValue> : IEnumerable<TValue>, IReadOnlyStratusMap<TKey, TValue>
	{
		[SerializeField]
		private List<TValue> _list = new List<TValue>();
		[NonSerialized]
		private Dictionary<TKey, TValue> _dictionary;

		#region Properties
		private Dictionary<TKey, TValue> lookup
		{
			get
			{
				if (_dictionary == null)
				{
					GenerateLookup();
				}
				return _dictionary;
			}
		}
		public int Count => _list.Count;
		public IReadOnlyList<TValue> Values => _list;
		public IReadOnlyCollection<TKey> Keys => lookup.Keys;
		#endregion

		public bool Add(TValue value)
		{
			TKey key = GetKey(value);
			if (Contains(key))
			{
				return false;
			}

			_list.Add(value);
			lookup.Add(key, value);
			return true;
		}

		public bool Remove(TValue value)
		{
			TKey key = GetKey(value);
			if (!Contains(key))
			{
				return false;
			}

			_list.Remove(value);
			lookup.Remove(key);
			return true;
		}

		public bool Remove(TKey key)
		{
			if (!Contains(key))
			{
				return false;
			}

			TValue value = lookup[key];
			_list.Remove(value);
			lookup.Remove(key);
			return true;
		}

		public TValue this[TKey key] => lookup[key];

		public bool AddRange(IEnumerable<TValue> collection)
		{
			if (collection.Any(x => Contains(x)))
			{
				return false;
			}

			_list.AddRange(collection);
			lookup.AddRange(GetKey, collection);
			return true;
		}

		public bool Contains(TValue value)
		{
			return Contains(GetKey(value));
		}

		public bool Contains(TKey key)
		{
			return lookup.ContainsKey(key);
		}

		public TValue Get(TKey key)
		{
			return lookup.GetValueOrDefault(key);
		}

		public void Clear()
		{
			_list?.Clear();
			lookup?.Clear();
		}

		private void GenerateLookup()
		{
			_dictionary = new Dictionary<TKey, TValue>();
		}

		protected abstract TKey GetKey(TValue value);

		public IEnumerator<TValue> GetEnumerator()
		{
			return ((IEnumerable<TValue>)this._list).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this._list).GetEnumerator();
		}
	}

	/// <summary>
	/// Non-serialized version
	/// </summary>
	public class StratusDefaultMap<TKey, TValue> : StratusMap<TKey, TValue>
	{
		private Func<TValue, TKey> keyFunction;

		public StratusDefaultMap(Func<TValue, TKey> keyFunction)
		{
			this.keyFunction = keyFunction;
		}

		protected override TKey GetKey(TValue value) => keyFunction(value);
	}

}