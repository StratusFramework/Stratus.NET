using System;
using System.Collections.Generic;
using System.Text;

namespace Stratus.Extensions
{
	public static class StratusDictionaryExtensions
	{
		/// <summary>
		/// Adds the given key-value pair if the key if not already present.
		/// Returns false if element was present (and not added).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static bool AddUnique<T, U>(this Dictionary<T, U> dictionary, T key, U value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, value);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Adds the value to the dictionary if not present, updates it otherwise
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddOrUpdate<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, Value value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		/// <summary>
		/// Increments the value to the dictionary element if present, 
		/// adds it otherwise
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddOrIncrement<Key>(this Dictionary<Key, int> dictionary, Key key, int value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] += value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="keyFunction"></param>
		public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Value, Key> keyFunction, IEnumerable<Value> values)
		{
			foreach (Value element in values)
			{
				dictionary.Add(keyFunction(element), element);
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the value for each key
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="keyFunction"></param>
		public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Value, Key> keyFunction, params Value[] values)
		{
			dictionary.AddRange(keyFunction, (IEnumerable<Value>)values);
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="valueFunction"></param>
		public static void AddRange<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Key, Value> valueFunction, IEnumerable<Key> keys)
		{
			foreach (Key key in keys)
			{
				dictionary.Add(key, valueFunction(key));
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value. and predicate
		/// </summary>
		public static void AddRangeByKey<Key, Value>(this Dictionary<Key, Value> dictionary,
			Func<Key, Value> valueFunction,
			Predicate<Key> predicate,
			IEnumerable<Key> keys)
		{
			foreach (Key key in keys)
			{
				if (predicate(key))
				{
					Value value = valueFunction(key);
					dictionary.Add(key, value);
				}
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value. and predicate
		/// </summary>
		public static void AddRangeByValue<Key, Value>(this Dictionary<Key, Value> dictionary,
			Func<Value, Key> keyFunction,
			Predicate<Value> predicate,
			IEnumerable<Value> values)
		{
			foreach (Value element in values)
			{
				if (predicate(element))
				{
					Key key = keyFunction(element);
					dictionary.Add(key, element);
				}
			}
		}

		/// <summary>
		/// Adds the given list to the dictionary, provided a function that will extract the key for each value.
		/// This will not attempt to add elements with duplicate kaeys.
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="values"></param>
		/// <param name="keyFunction"></param>
		public static void AddRangeUnique<Key, Value>(this Dictionary<Key, Value> dictionary, Func<Value, Key> keyFunction, IEnumerable<Value> values)
		{
			foreach (Value element in values)
			{
				Key key = keyFunction(element);
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, element);
				}
			}
		}

		/// <summary>
		/// Adds the given key-value pair if the key is not present, also adding the necessary list
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void AddIfMissing<Key, Value>(this Dictionary<Key, List<Value>> dictionary, Key key, Value value)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new List<Value>());
			}
			dictionary[key].Add(value);
		}

		/// <summary>
		/// Invokes the given action on every element of the list if the key is present within the dictionary
		/// </summary>
		public static void TryInvoke<Key, Value>(this Dictionary<Key, List<Value>> dictionary,
			Key key,
			Action<Value> action)
		{
			if (dictionary.ContainsKey(key))
			{
				foreach (Value item in dictionary[key])
				{
					action(item);
				}
			}
		}

		/// <summary>
		/// Invokes the given action on the value within the dictionary if present
		/// </summary>
		public static void TryInvoke<Key, Value>(this Dictionary<Key, Value> dictionary,
			Key key,
			Action<Value> action)
		{
			if (dictionary.ContainsKey(key))
			{
				action(dictionary[key]);
			}
		}

		/// <summary>
		/// Invokes the given function on the value within the dictionary if present.
		/// If not, will return the default value.
		/// </summary>
		public static ReturnValue TryInvoke<Key, Value, ReturnValue>(this Dictionary<Key, Value> dictionary,
			Key key,
			Func<Value, ReturnValue> func)
		{
			if (dictionary.ContainsKey(key))
			{
				Value value = dictionary[key];
				return func(value);
			}
			return default;
		}

		/// <summary>
		/// Returns the value from the dictionary if present, otherwise the default
		/// </summary>
		public static Value GetValueOrDefault<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, Value defaultValue = default)
		{
			if (!dictionary.ContainsKey(key))
			{
				return defaultValue;
			}
			return dictionary[key];
		}

		/// <summary>
		/// Returns the value from the dictionary if present, otherwise generates it (from a value function)
		/// </summary>
		public static Value GetValueOrGenerate<Key, Value>(this Dictionary<Key, Value> dictionary, Key key, Func<Key, Value> valueFunction)
		{
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, valueFunction(key));
			}
			return dictionary[key];
		}

		/// <summary>
		/// Returns a string of stringified Key-Value pair lines
		/// </summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static string ToString<Key, Value>(this Dictionary<Key, Value> dictionary, string separator,
			Func<Key, string> keyToStringFunction = null,
			Func<Value, string> valueToStringFunction = null)
		{
			StringBuilder sb = new StringBuilder();
			bool hasKeyFunction = keyToStringFunction != null;
			bool hasValueFunction = valueToStringFunction != null;
			foreach (var kp in dictionary)
			{
				sb.AppendLine($"[{(hasKeyFunction ? keyToStringFunction(kp.Key) : kp.Key.ToString())}{separator}{(hasValueFunction ? valueToStringFunction(kp.Value) : kp.Value.ToString())}]");
			}
			return sb.ToString();
		}

		public static Dictionary<Key, List<Value>> ToDictionaryOfList<Key, Value>(this IEnumerable<Value> values, Func<Value, Key> selector)
		{
			Dictionary<Key, List<Value>> result = new Dictionary<Key, List<Value>>();
			foreach (var value in values)
			{
				Key key = selector(value);
				if (!result.ContainsKey(key))
				{
					result.Add(key, new List<Value>());
				}
				result[key].Add(value);
			}
			return result;
		}

		public static Dictionary<K2, List<K1>> Bucket<K1, V1, K2>(this Dictionary<K1, V1> source, Func<KeyValuePair<K1, V1>, K2> mapper)
		{
			Dictionary<K2, List<K1>> result = new Dictionary<K2, List<K1>>();
			foreach (var kvp in source)
			{
				K2 key = mapper(kvp);
				if (!result.ContainsKey(key))
				{
					result.Add(key, new List<K1>());
				}
				result[key].Add(kvp.Key);
			}
			return result;
		}

		public static Dictionary<TKey, TValue2> TransformValues<TKey, TValue1, TValue2>(this Dictionary<TKey, TValue1> source, Func<TValue1, TValue2> convert)
		{
			Dictionary<TKey, TValue2> result = new Dictionary<TKey, TValue2>();
			foreach (var kvp in source)
			{
				result.Add(kvp.Key, convert(kvp.Value));
			}
			return result;
		}

		public static void AddFrom<TKey, TValue, TList>(this Dictionary<TKey, List<TValue>> self,
			Dictionary<TKey, TList> other)
			where TList : IList<TValue>
		{
			foreach (var kvp in other)
			{
				if (!self.ContainsKey(kvp.Key))
				{
					self.Add(kvp.Key, new List<TValue>());
				}
				else
				{
					self[kvp.Key].AddRange(kvp.Value);
				}
			}
		}
	}
}
