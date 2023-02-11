using Stratus.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models
{
	/// <summary>
	/// Utlity class for working on a list of serializable TreeElements where the order
	/// and depth of each tree element define the tree structure. 
	/// The TreeModel itself is not serializable but the input list is.
	/// The tree representation (parent and children references) are built internally using 
	/// an utility function to convert the list to tree using the depth values of the elements.
	/// The first element of the input list is required to have a depth of -1 (the hidden root),
	/// and the rest a depth of >= 0.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StratusTreeModel<T> where T : StratusTreeElement
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/ 
		private StratusProvider<IList<T>> provider;
		private IList<T> elements => provider.value;
		private int maxID;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/     
		public T root
		{
			get
			{
				if (_root == null)
				{
					BuildRoot();
				}
				return _root;
			}
		}
		private T _root;

		#region Events
		public event Action onModelChanged;
		#endregion

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/ 
		public StratusTreeModel(StratusProvider<IList<T>> data)
		{
			this.SetData(data);
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/ 
		/// <summary>
		/// Sets the data for this tree model
		/// </summary>
		/// <param name="provider"></param>
		public void SetData(StratusProvider<IList<T>> provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("No input data given!");
			}
			this.provider = provider;
		}

		/// <summary>
		/// Generates the root element of the model
		/// </summary>
		public void BuildRoot()
		{
			if (this.elements.Count > 0)
			{
				this._root = StratusTreeElement.ListToTree(this.elements);
				this.maxID = this.elements.Max(d => d.id);
			}
		}

		/// <summary>
		/// Finds the given element by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public T Find(int id) => this.elements.FirstOrDefault(element => element.id == id);

		/// <summary>
		/// Generates an unique id for a tree element
		/// </summary>
		/// <returns></returns>
		public int GenerateUniqueID() => ++this.maxID;

		/// <summary>
		/// Gets the ids of all ancestors to the element of given id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IList<int> GetAncestors(int id)
		{
			var parents = new List<int>();
			StratusTreeElement T = Find(id);
			if (T != null)
			{
				while (T.parent != null)
				{
					parents.Add(T.parent.id);
					T = T.parent;
				}
			}
			return parents;
		}

		/// <summary>
		/// Gets the ids of all descendants to the element of given id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public IList<int> GetDescendantsThatHaveChildren(int id)
		{
			T searchFromThis = Find(id);
			if (searchFromThis != null)
			{
				return GetParentsBelowStackBased(searchFromThis);
			}
			return new List<int>();
		}

		/// <summary>
		/// Adds the given elements to the tree model
		/// </summary>
		/// <param name="elements"></param>
		/// <param name="parent"></param>
		/// <param name="insertPosition"></param>
		public void AddElements(IList<T> elements, StratusTreeElement parent, int insertPosition)
		{
			if (elements == null)
				throw new ArgumentNullException("elements", "elements is null");
			if (elements.Count == 0)
				throw new ArgumentNullException("elements", "elements Count is 0: nothing to add");
			if (parent == null)
				throw new ArgumentNullException("parent", "parent is null");

			if (parent.children == null)
				parent.children = new List<StratusTreeElement>();

			parent.children.InsertRange(insertPosition, elements.Cast<StratusTreeElement>());
			foreach (var element in elements)
			{
				element.parent = parent;
				element.depth = parent.depth + 1;
				StratusTreeElement.UpdateDepthValues(element);
			}

			StratusTreeElement.TreeToList(this.root, this.elements);

			OnChanged();
		}

		/// <summary>
		/// Adds an element onto the tree
		/// </summary>
		/// <param name="element"></param>
		/// <param name="parent"></param>
		/// <param name="insertPosition"></param>
		public T AddElement(T element, StratusTreeElement parent, int insertPosition)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element", "element is null");
			}
			if (parent == null)
			{
				throw new ArgumentNullException("parent", "parent is null");
			}

			if (parent.children == null)
			{
				parent.children = new List<StratusTreeElement>();
			}

			parent.children.Insert(insertPosition, element);
			element.parent = parent;

			StratusTreeElement.UpdateDepthValues(parent);
			int parentIndex = elements.IndexOf((T)parent);
			elements.Insert(parentIndex + 1 + insertPosition, element);
			//StratusTreeElement.TreeToList(this.root, this.elements);

			OnChanged();
			return element;
		}

		/// <summary>
		/// Adds the root element to this model
		/// </summary>
		/// <param name="root"></param>
		public void AddRoot(T root)
		{
			if (root == null)
				throw new ArgumentNullException("root", "root is null");

			if (this.elements == null)
				throw new InvalidOperationException("Internal Error: data list is null");

			if (this.elements.Count != 0)
				throw new InvalidOperationException("AddRoot is only allowed on empty data list");

			root.id = GenerateUniqueID();
			root.depth = -1;
			this.elements.Add(root);
		}

		/// <summary>
		/// Removes all elements with the given id
		/// </summary>
		/// <param name="elementIDs"></param>
		public void RemoveElements(IList<int> elementIDs)
		{
			IList<T> elements = this.elements.Where(element => elementIDs.Contains(element.id)).ToArray();
			RemoveElements(elements);
		}

		/// <summary>
		/// Removes the given elements 
		/// </summary>
		/// <param name="elements"></param>
		public void RemoveElements(IList<T> elements)
		{
			foreach (var element in elements)
				if (element == this.root)
					throw new ArgumentException("It is not allowed to remove the root element");

			var commonAncestors = StratusTreeElement.FindCommonAncestorsWithinList(elements);

			foreach (var element in commonAncestors)
			{
				element.parent.children.Remove(element);
				element.parent = null;
			}

			StratusTreeElement.TreeToList(this.root, this.elements);

			OnChanged();
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(StratusTreeElement parentElement, int insertionIndex, List<StratusTreeElement> elements)
		{
			MoveElements(parentElement, insertionIndex, elements.ToArray());
		}

		/// <summary>
		/// Reparents the given elements
		/// </summary>
		/// <param name="parentElement"></param>
		/// <param name="insertionIndex"></param>
		/// <param name="elements"></param>
		public void MoveElements(StratusTreeElement parentElement, int insertionIndex, params StratusTreeElement[] elements)
		{
			if (insertionIndex < 0)
				throw new ArgumentException("Invalid input: insertionIndex is -1, client needs to decide what index elements should be reparented at");

			// Invalid reparenting input
			if (parentElement == null)
				return;

			// We are moving items so we adjust the insertion index to accomodate that any items above the insertion index is removed before inserting
			if (insertionIndex > 0)
				insertionIndex -= parentElement.children.GetRange(0, insertionIndex).Count(elements.Contains);

			// Remove draggedItems from their parents
			foreach (var draggedItem in elements)
			{
				draggedItem.parent.children.Remove(draggedItem);  // remove from old parent
				draggedItem.parent = parentElement;         // set new parent
			}

			if (parentElement.children == null)
				parentElement.children = new List<StratusTreeElement>();

			// Insert dragged items under new parent
			parentElement.children.InsertRange(insertionIndex, elements);

			StratusTreeElement.UpdateDepthValues(root);
			StratusTreeElement.TreeToList(this.root, this.elements);

			OnChanged();
		}

		/// <summary>
		/// Invoked when the model is changed
		/// </summary>
		private void OnChanged()
		{
			if (this.onModelChanged != null)
				onModelChanged();
		}

		//------------------------------------------------------------------------/
		// Methods: Private
		//------------------------------------------------------------------------/ 
		private IList<int> GetParentsBelowStackBased(StratusTreeElement searchFromThis)
		{
			Stack<StratusTreeElement> stack = new Stack<StratusTreeElement>();
			stack.Push(searchFromThis);

			var parentsBelow = new List<int>();
			while (stack.Count > 0)
			{
				StratusTreeElement current = stack.Pop();
				if (current.hasChildren)
				{
					parentsBelow.Add(current.id);
					foreach (var T in current.children)
					{
						stack.Push(T);
					}
				}
			}

			return parentsBelow;
		}

	}
}