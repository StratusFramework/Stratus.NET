using System;
using System.Collections.Generic;

namespace Stratus.Collections
{
	public class StratusDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		private Func<TValue, TKey> keyFunction;

		public StratusDictionary(Func<TValue, TKey> keyFunction,
								 int capacity = 0,
								 IEqualityComparer<TKey> comparer = null)
								 : base(capacity, comparer)
		{
			this.keyFunction = keyFunction;
		}

		public StratusDictionary(Func<TValue, TKey> keyFunction,
								IEnumerable<TValue> values,
								 int capacity = 0,
								 IEqualityComparer<TKey> comparer = null)
								 : this(keyFunction, capacity, comparer)
		{
			AddRange(values);
		}


		public bool Add(TValue value)
		{
			TKey key = keyFunction(value);
			if (ContainsKey(key))
			{
				//StratusDebug.LogError($"Value with key '{key}' already exists in this collection!");
				return false;
			}
			Add(key, value);
			return true;
		}

		public int AddRange(IEnumerable<TValue> values)
		{
			if (values == null)
			{
				return 0;
			}

			int failCount = 0;
			foreach (TValue value in values)
			{
				if (!Add(value))
				{
					failCount++;
				}
			}
			return failCount;
		}

		public bool Remove(TValue value)
		{
			TKey key = keyFunction(value);
			if (!ContainsKey(key))
			{
				return false;
			}
			Remove(key);
			return true;
		}

		public TValue GetValueOrDefault(TKey key)
		{
			return ContainsKey(key) ? this[key] : default;
		}
	}
}