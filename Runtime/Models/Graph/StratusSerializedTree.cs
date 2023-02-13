using Stratus.Extensions;
using Stratus.Models;
using Stratus.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Graph
{
	public class StratusSerializedTree<TElement>
		where TElement : TreeElement, new()
	{
		#region Fields
		[SerializeField]
		protected List<TElement> _elements = new List<TElement>();
		[SerializeField]
		protected int idCounter = 0;
		[NonSerialized]
		private TElement _root;
		[SerializeField]
		protected int _maxDepth;
		#endregion

		#region Properties
		public TElement root
		{
			get
			{
				if (!this.valid)
				{
					this.BuildRootFromElements();
				}

				return this._root;
			}
		}
		/// <summary>
		/// The elements of the tree (including the root node at index 0)
		/// </summary>
		public TElement[] elements => _elements.ToArray();
		/// <summary>
		/// Whether the tree is currently valid
		/// </summary>
		private bool valid => this._root != null;
		/// <summary>
		/// Whether the tree has elements (not including the root node)
		/// </summary>
		public bool hasElements => Count > 0;
		/// <summary>
		/// The current max depth of the tree, that is the depth of its deepest node
		/// </summary>
		public int maxDepth => this._maxDepth;
		/// <summary>
		/// Whether the tree has a root element
		/// </summary>
		public bool hasRoot => this.hasElements && this._elements[0].depth == rootDepth;
		/// <summary>
		/// The number of elements in the tree, not including the root node.
		/// </summary>
		public int Count => _elements.Count - 1;
		#endregion

		#region Constants
		public const int rootDepth = -1;
		public const int defaultDepth = 0;
		#endregion

		#region Constructors
		public StratusSerializedTree(IEnumerable<TElement> elements)
		{
			this._elements.AddRange(elements);
			BuildRootFromElements();
		}

		public StratusSerializedTree()
		{
			this.AddRoot();
		}
		#endregion

		protected void BuildRootFromElements()
		{
			this._root = TreeElement.ListToTree(this._elements);
		}

		#region Internal
		private void AddRoot()
		{
			TElement root = new TElement
			{
				name = "Root",
				depth = -1,
				id = this.idCounter++
			};
			this._elements.Insert(0, root);
		}

		protected TElement GetElement(int index)
		{
			return this._elements[index];
		}

		protected int FindIndex(TElement element)
		{
			int index = this._elements.IndexOf(element);
			return index;
		}

		protected int FindLastChildIndex(TElement element)
		{
			int index = this.FindIndex(element);
			int lastIndex = index + element.totalChildrenCount;
			return lastIndex;
		}

		protected TElement[] FindChildren(TElement element)
		{
			int index = this.FindIndex(element);
			return this._elements.GetRange(index, element.totalChildrenCount).ToArray();
		}

		protected int GenerateID()
		{
			return this.idCounter++;
		}

		/// <summary>
		/// Creates and adds the element to the tree
		/// </summary>
		protected TElement CreateElement(int depth)
		{
			TElement element = new TElement();
			OnAddElement(element, depth, true);
			return element;
		}

		private void OnAddElement(TElement element, int depth, bool generateID)
		{
			element.depth = depth;
			if (generateID)
			{
				element.id = GenerateID();
			}
			if (depth > this.maxDepth)
			{
				this._maxDepth = element.depth;
			}
			_elements.Add(element);
		}
		#endregion

		#region Interface
		public void AddElement(TElement element, int depth = defaultDepth, bool generateID = true)
		{
			OnAddElement(element, depth, generateID);
		}

		public void RemoveElement(TElement element)
		{
			// Remove all children first
			if (element.hasChildren)
			{
				foreach (TreeElement child in element.allChildren)
				{
					this._elements.Remove((TElement)child);
				}
			}

			this._elements.Remove(element);
		}

		public void RemoveElementExcludeChildren(TElement element)
		{
			TreeElement parent = element.parent != null ? element.parent : this.root;

			if (element.hasChildren)
			{
				this.Reparent(parent, element.children);
			}

			this._elements.Remove(element);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(TElement parentElement, int insertionIndex, List<TElement> elements)
		{
			this.MoveElements(parentElement, insertionIndex, elements.ToArray());
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void Reparent(TreeElement parentElement, params TreeElement[] elements)
		{
			TreeElement.Parent(parentElement, elements);
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void Reparent(TreeElement parentElement, List<TreeElement> elements)
		{
			TreeElement.Parent(parentElement, elements.ToArray());
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(TElement parentElement, int insertionIndex, params TElement[] elements)
		{
			if (insertionIndex < 0)
			{
				throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");
			}

			// Invalid reparenting input
			if (parentElement == null)
			{
				return;
			}

			// We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
			if (insertionIndex > 0)
			{
				insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);
			}

			// Remove draggedItems from their parents
			foreach (TElement draggedItem in elements)
			{
				draggedItem.parent.children.Remove(draggedItem);  // remove from old parent
				draggedItem.parent = parentElement;         // set new parent
			}

			if (parentElement.children == null)
			{
				parentElement.children = new List<TreeElement>();
			}

			// Insert dragged items under new parent
			parentElement.children.InsertRange(insertionIndex, elements);

			TreeElement.UpdateDepthValues(this.root);
		}

		public void Iterate(Action<TElement> action)
		{
			if (!this.valid)
			{
				this.BuildRootFromElements();
			}

			foreach (TElement element in this._elements)
			{
				action(element);
			}
		}

		public void Assert()
		{
			TreeElement.Assert(this._elements);
		}

		public void Repair()
		{
			if (!this.hasRoot)
			{
				this.AddRoot();
			}
			TreeElement.UpdateDepthValues(this.root);
		}

		public Exception Validate()
		{
			Exception exception = TreeElement.Validate(this._elements);
			return exception;
		}

		public void Clear()
		{
			this._elements.Clear();
			this.idCounter = 0;
			this.AddRoot();
		}
		#endregion
	}

	/// <summary>
	/// A tree with an element type that encapsulates a data type
	/// </summary>
	/// <typeparam name="TElement"></typeparam>
	/// <typeparam name="TData"></typeparam>
	[Serializable]
	public class StratusSerializedTree<TElement, TData>
		: StratusSerializedTree<TElement>

	  where TElement : TreeElement<TData>, new()
	  where TData : class, IStratusNamed
	{
		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		public StratusSerializedTree(IEnumerable<TElement> elements)
			: base(elements)
		{
		}

		public StratusSerializedTree(IEnumerable<TData> values) : this()
		{
			AddElements(values, 0);
		}

		public StratusSerializedTree() : base()
		{
		}

		//------------------------------------------------------------------------/
		// Methods: Public
		//------------------------------------------------------------------------/
		public void AddElement(TData data)
		{
			this.CreateElement(data, defaultDepth);
		}

		public TElement AddChildElement(TData data, TElement parent)
		{
			// Insert element below the last child
			TElement element = this.CreateElement(data, parent.depth + 1);
			int insertionIndex = this.FindLastChildIndex(parent) + 1;
			this._elements.Insert(insertionIndex, element);
			return element;
		}

		public void AddParentElement(TData data, TElement element)
		{
			// Insert element below the last child
			TElement parentElement = this.CreateElement(data, element.depth);
			element.depth++;
			parentElement.parent = element.parent;

			int insertionIndex = this.FindIndex(element);

			foreach (TElement child in this.FindChildren(element))
			{
				child.depth++;
			}

			this._elements.Insert(insertionIndex, parentElement);
		}

		public void ReplaceElement(TElement originalElement, TData replacementData)
		{
			TElement replacementElement = this.AddChildElement(replacementData, (TElement)originalElement.parent);
			if (originalElement.hasChildren)
			{
				this.Reparent(replacementElement, originalElement.children);
			}

			this.RemoveElement(originalElement);
		}

		public void AddElements(IEnumerable<TData> elementsData, int depth)
		{
			foreach (TData data in elementsData)
			{
				this.CreateElement(data, depth);
			}
		}

		#region Internal
		/// <summary>
		/// Creates and adds the element to the tree
		/// </summary>
		private TElement CreateElement(TData data, int depth)
		{
			TElement element = CreateElement(depth);
			element.Set(data);
			return element;
		}
		#endregion
	}

}