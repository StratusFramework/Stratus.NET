using System;
using System.Collections.Generic;

namespace Stratus.Collections
{
	/// <summary>
	/// A list of filtered items
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FilteredList<T>
	{
		#region Declarations
		public struct Entry
		{
			public int index;
			public Item item;
		}

		public class Item
		{
			public string name;
			public T value;
		}

		protected struct EntriesHook
		{
			public List<T> values;
			public Action onFinished;
		}
		#endregion

		#region Properties
		public string filter
		{
			get => this._filter;
			private set => this.UpdateFilter(value);
		}
		private string _filter;

		public List<Entry> currentEntries { get; private set; }
		public Item[] entries { get; private set; }
		public string[] displayOptions { get; private set; }
		public int currentEntryCount => this.currentEntries.Count;
		private Func<T, string> nameFunction { get; set; }
		public int maxIndex => this.currentEntries.Count - 1;
		public bool hasFilter => !string.IsNullOrEmpty(this.filter);
		#endregion

		#region Constructors
		public FilteredList(T[] items, Func<T, string> nameFunction)
		{
			this.entries = new Item[items.Length];
			for (int i = 0; i < items.Length; ++i)
			{
				T value = items[i];
				this.entries[i] = new Item() { name = nameFunction(value), value = value };
			}
			this.nameFunction = nameFunction;
			this.currentEntries = new List<Entry>();
			this.UpdateFilter(string.Empty);
		}
		#endregion

		#region Interface
		public bool UpdateFilter(string filter)
		{
			// Set the filter
			if (filter == this._filter)
			{
				return false;
			}
			this._filter = filter;
			string filterLowercase = this.filter.ToLower();

			// Update the entries
			this.currentEntries.Clear();

			// Optional
			List<string> displayOptions = new List<string>();

			for (int i = 0; i < this.entries.Length; ++i)
			{
				string name = this.entries[i].name.ToLower();
				if (string.IsNullOrEmpty(this.filter) || name.Contains(filterLowercase))
				{
					Entry entry = new Entry
					{
						index = i,
						item = this.entries[i]
					};

					if (string.Equals(name, filter, StringComparison.CurrentCultureIgnoreCase))
					{
						this.currentEntries.Insert(0, entry);
						displayOptions.Insert(0, this.entries[i].name);
					}
					else
					{
						this.currentEntries.Add(entry);
						displayOptions.Add(this.entries[i].name);
					}
				}
			}

			this.displayOptions = displayOptions.ToArray();
			return true;
		} 
		#endregion
	}

	public class StratusFilteredStringList : FilteredList<string>
	{
		private static Func<string, string> stringNameFunction { get; } = (value) => value;

		public StratusFilteredStringList(string[] items) : base(items, stringNameFunction)
		{
		}

	}

}