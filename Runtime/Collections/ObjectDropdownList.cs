using Stratus.Extensions;

using System.Collections.Generic;

namespace Stratus
{
	/// <summary>
	/// A generic dropdown list that refers to a list of any given Object type
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ObjectDropdownList<T> where T : class
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public string[] displayedOptions { get; private set; }
		public int selectedIndex { get; set; }
		public T selected => isList ? list[selectedIndex] : array[selectedIndex];

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		private List<T> list;
		private T[] array;
		private bool isList;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public ObjectDropdownList(List<T> list, T initial = null)
		{
			this.list = list;
			isList = true;
			displayedOptions = list.ToStringArray();

			if (initial != null)
			{
				SetIndex(initial);
			}
		}

		public ObjectDropdownList(List<T> list, int index = 0)
		{
			this.list = list;
			isList = true;
			displayedOptions = list.ToStringArray();
			selectedIndex = 0;
		}

		public ObjectDropdownList(T[] array, T initial = null)
		{
			this.array = array;
			isList = false;
			displayedOptions = array.ToStringArray();

			if (initial != null)
			{
				SetIndex(initial);
			}
		}

		public ObjectDropdownList(T[] array, int index = 0)
		{
			this.array = array;
			isList = false;
			displayedOptions = array.ToStringArray();
			selectedIndex = 0;
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

	}
}