using Stratus.Extensions;

using System;
using System.Collections.Generic;

namespace Stratus.Collections
{
	/// <summary>
	/// An utility class for generating content for a dropdown list
	/// </summary>
	public class StratusDropdownList
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public string[] displayedOptions { get; protected set; }
		public int selectedIndex { get; set; }
	}

	/// <summary>
	/// An utility class for generating content for a dropdown list
	/// </summary>
	public class StratusDropdownList<T> : StratusDropdownList where T : class
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public T selected => isList ? list[selectedIndex] : array[selectedIndex];

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private List<T> list;
		private T[] array;
		private bool isList;
		private Func<T, string> nameFunction;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusDropdownList(List<T> list, Func<T, string> nameFunction, T initial = null)
		{
			this.list = list;
			isList = true;
			this.nameFunction = nameFunction;
			displayedOptions = list.ToStringArray(nameFunction);

			if (initial != null)
				SetIndex(initial);
		}

		public StratusDropdownList(List<T> list, Func<T, string> namer, int index = 0)
		{
			this.list = list;
			isList = true;
			this.nameFunction = namer;
			displayedOptions = list.ToStringArray(namer);
			selectedIndex = index;
		}

		public StratusDropdownList(T[] array, Func<T, string> namer, T initial = null)
		{
			this.array = array;
			isList = false;
			this.nameFunction = namer;
			displayedOptions = array.ToStringArray(namer);

			if (initial != null)
				SetIndex(initial);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Sort()
		{
			Array.Sort(displayedOptions);
			if (isList)
				list.Sort();
			else
				Array.Sort(array, (left, right) => { return nameFunction(left).CompareTo(nameFunction(right)); });
		}

		/// <summary>
		/// Sets the current index of this list to that of the given element
		/// </summary>
		/// <param name="element"></param>
		public void SetIndex(T element)
		{
			if (isList)
				selectedIndex = list.FindIndex(x => x == element);
			else
				selectedIndex = array.FindIndex(x => x == element);
		}

		/// <summary>
		/// Retrieves the element at the given index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T AtIndex(int index) => isList ? list[index] : array[index];
	}
}