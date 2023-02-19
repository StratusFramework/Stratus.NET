using System.Collections.Generic;
using System;

namespace Stratus.Collections
{
	public class StratusSortedList<KeyType, ValueType> : SortedList<KeyType, ValueType>
	{
		private Func<ValueType, KeyType> keyFunction;

		public StratusSortedList(Func<ValueType, KeyType> keyFunction,
			int capacity = 0,
			IComparer<KeyType> comparer = null)
			: base(capacity, comparer)
		{
			this.keyFunction = keyFunction;
		}

		public StratusSortedList(Func<ValueType, KeyType> keyFunction, IEnumerable<ValueType> values,
			int capacity = 0,
			IComparer<KeyType> comparer = null)
			: this(keyFunction, capacity, comparer)
		{
			AddRange(values);
		}

		public bool Add(ValueType value, bool overwrite = false)
		{
			KeyType key = keyFunction(value);
			if (ContainsKey(key))
			{
				if (overwrite)
				{
					this[key] = value;
					return true;
				}
				else
				{
					//StratusDebug.LogError($"Value with key '{key}' already exists in this collection!");
					return false;
				}
			}
			Add(key, value);
			return true;
		}

		public int AddRange(IEnumerable<ValueType> values, bool overwrite = false)
		{
			if (values == null)
			{
				return 0;
			}

			int failCount = 0;
			foreach (ValueType value in values)
			{
				if (!Add(value, overwrite))
				{
					failCount++;
				}
			}
			return failCount;
		}

		public bool Remove(ValueType value)
		{
			KeyType key = keyFunction(value);
			if (!ContainsKey(key))
			{
				return false;
			}
			Remove(key);
			return true;
		}

		public ValueType GetValueOrDefault(KeyType key)
		{
			return ContainsKey(key) ? this[key] : default;
		}
	}

}