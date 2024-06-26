using Stratus.Extensions;

using System;
using System.Collections.Generic;
using Stratus.Models.Math;
using System.Numerics;
using static Stratus.Collections.ArrayNavigator;

namespace Stratus.Collections
{
	/// <summary>
	/// Base class for the array navigator
	/// </summary>
	public abstract class ArrayNavigator
	{
		public enum Direction
		{
			Up,
			Down,
			Left,
			Right
		}
	}

	public interface IArrayNavigator<T>
	{
		int count { get; }
		bool valid { get; }
		T current { get; }

		T Next();
		T Previous();
		T Navigate(Direction dir);

	}

	/// <summary>
	/// Provides a generic way to navigate a 1D array using directional axis.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ArrayNavigator<T> : ArrayNavigator, IArrayNavigator<T>
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		/// <summary>
		/// Retrieves the last element in the array.
		/// </summary>
		/// <returns></returns>
		public T current => values[currentIndex];

		/// <summary>
		/// Retrieves the last element in the array.
		/// </summary>
		/// <returns></returns>
		public T first => values[0];

		/// <summary>
		/// Retrieves the last element in the array.
		/// </summary>
		/// <returns></returns>
		public T previous => values[previousIndex];

		/// <summary>
		/// Retrieves the last element in the array.
		/// </summary>
		/// <returns></returns>
		public T last => values[indexSize];

		/// <summary>
		/// Retrieves a random element in the array.
		/// </summary>
		/// <returns></returns>
		public T random
		{
			get
			{
				var randomIndex = RandomUtility.Range(0, indexSize);
				return values[randomIndex];
			}
		}

		/// <summary>
		/// Whether on navigating to the end of the array, we loop around
		/// </summary>
		public bool loop { get; set; }

		/// <summary>
		/// The amount of 0-indexed elements in the array
		/// </summary>
		private int indexSize { get { return values.Count - 1; } }

		/// <summary>
		/// The length of the array
		/// </summary>
		public int count => values.CountOrDefault();

		/// <summary>
		/// Whether its in a valid sate
		/// </summary>
		public bool valid => count > 0;

		/// <summary>
		/// Whether the underlying array is empty
		/// </summary>
		public bool empty => count == 0;

		/// <summary>
		/// The current index
		/// </summary>
		public int currentIndex { get; private set; }

		/// <summary>
		/// The current index
		/// </summary>
		public int previousIndex { get; private set; }

		/// <summary>
		/// The last index in the array
		/// </summary>
		public int firstIndex => 0;

		/// <summary>
		/// The last index in the array
		/// </summary>
		public int lastIndex => indexSize;

		/// <summary>
		/// Whether navigation is currently at the last index
		/// </summary>
		public bool atLastIndex => currentIndex == indexSize;

		/// <summary>
		/// Whether the index has recently changed
		/// </summary>
		public bool recentlyChanged { get; private set; }

		/// <summary>
		/// Returns the previous value, if not equal to current
		/// </summary>
		protected T previousIfNotCurrent => !previous.Equals(current) ? previous : default;

		/// <summary>
		/// Whether there's a value at the next possible index
		/// </summary>
		public bool hasNext => currentIndex != lastIndex;

		/// <summary>
		/// Whether there's a value at the previous possible index
		/// </summary>
		public bool hasPrevious => currentIndex - 1 > firstIndex;

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Function to invoke once the index of this array has changed
		/// </summary>
		public event Action<T, int> onIndexChanged;

		/// <summary>
		/// Function to invoke once the current element has changed
		/// (Current, Previous)
		/// </summary>
		public event Action<T, T> onChanged;

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		/// <summary>
		/// The array being used
		/// </summary>
		public IList<T> values { get; private set; }

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public ArrayNavigator()
		{
			currentIndex = 0;
		}

		public ArrayNavigator(IList<T> array, bool loop = false)
			: this(array, 0, loop)
		{
		}

		public ArrayNavigator(IList<T> array, int index, bool loop = false)
		{
			this.values = array;
			this.loop = loop;
			this.currentIndex = index;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Sets an updated array to use for navigation
		/// </summary>
		/// <param name="values"></param>
		public void Set(IList<T> values)
		{
			this.values = values;
			currentIndex = 0;
		}

		/// <summary>
		/// Updates the current index to point at the given element
		/// </summary>
		/// <param name="element"></param>
		public bool NavigateToElement(T element)
		{
			// Look for the element in the array
			var index = 0;
			foreach (var e in values)
			{
				// The element was found
				if (e.Equals(element))
				{
					NavigateToIndex(index);
					return true;
				}
				index++;
			}

			return false;
		}

		public T NavigateToIndex(int index)
		{
			if (!ContainsIndex(index))
			{
				return current;
			}

			if (currentIndex != index)
			{
				previousIndex = currentIndex;
				currentIndex = index;
				OnIndexChanged();
			}
			else
			{
				recentlyChanged = false;
			}

			return current;

		}

		public void Clear()
		{
			values = null;
			currentIndex = 0;
		}

		public bool ContainsIndex(int index) => values.ContainsIndex(index);

		/// <summary>
		/// Navigates along the array in a given direction
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public T Navigate(Direction dir)
		{
			if (dir == Direction.Right || dir == Direction.Up)
			{
				return this.Next();
			}
			else if (dir == Direction.Left || dir == Direction.Down)
			{
				return this.Previous();
			}
			recentlyChanged = false;
			return current;
		}

		/// <summary>
		/// Navigates to the element at the given index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T Navigate(int index)
		{
			if (!values.ContainsIndex(index))
			{
				return default;
			}

			if (currentIndex != index)
			{
				previousIndex = currentIndex;
				currentIndex = index;
				OnIndexChanged();
			}
			else
			{
				recentlyChanged = false;
			}

			return current;
		}

		public T Navigate(Vector2 dir)
		{
			if (dir.X > 0)
			{
				return Navigate(Direction.Right);
			}
			else if (dir.X < 0)
			{
				return Navigate(Direction.Left);
			}
			else if (dir.Y > 0)
			{
				return Navigate(Direction.Up);
			}
			else if (dir.Y < 0)
			{
				return Navigate(Direction.Down);
			}

			return current;
		}

		/// <summary>
		/// Navigates to the first element
		/// </summary>
		/// <returns></returns>
		public T NavigateToFirst()
		{
			return Navigate(0);
		}

		/// <summary>
		/// Navigates to the first element
		/// </summary>
		/// <returns></returns>
		public T NavigateToLast()
		{
			return Navigate(indexSize);
		}

		/// <summary>
		/// Navigates to the next element in the array
		/// </summary>
		/// <returns></returns>
		public T Next()
		{
			if (currentIndex < indexSize)
			{
				previousIndex = currentIndex;
				currentIndex++;
				OnIndexChanged();
			}
			else if (loop)
			{
				previousIndex = currentIndex;
				currentIndex = 0;
				OnIndexChanged();
			}
			else
			{
				recentlyChanged = false;
			}
			return this.current;
		}

		/// <summary>
		/// Navigates to the previous element in the array
		/// </summary>
		/// <returns></returns>
		public T Previous()
		{
			if (currentIndex != 0)
			{
				previousIndex = currentIndex;
				currentIndex--;
				OnIndexChanged();
			}
			else if (loop)
			{
				previousIndex = currentIndex;
				currentIndex = indexSize;
				OnIndexChanged();
			}
			else
			{
				recentlyChanged = false;
			}

			return this.current;
		}

		/// <summary>
		/// Navigates to the next element
		/// </summary>
		public void NavigateToNext() => Next();

		/// <summary>
		/// Navigates to the previous element
		/// </summary>
		public void NavigateToPrevious() => Previous();

		private void OnIndexChanged()
		{
			onIndexChanged?.Invoke(current, currentIndex);
			onChanged?.Invoke(current, previousIfNotCurrent);
			recentlyChanged = true;
		}

	}
}
