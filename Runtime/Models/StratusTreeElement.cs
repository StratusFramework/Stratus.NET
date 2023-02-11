using System;
using System.Collections.Generic;

namespace Stratus.Models
{
	/// <summary>
	/// A serialized element of a Tree Model
	/// </summary>
	[Serializable]
	public partial class StratusTreeElement
	{
		#region Fields
		public int id;
		public string name;
		public int depth;

		[NonSerialized] public StratusTreeElement parent;
		[NonSerialized] public List<StratusTreeElement> children = new List<StratusTreeElement>();
		#endregion

		#region Properties
		/// <summary>
		/// Whether this tree element has children
		/// </summary>
		public bool hasChildren => children != null && children.Count > 0;
		/// <summary>
		/// The root node must have a depth of -1
		/// </summary>
		public bool isRoot => depth == -1;
		/// <summary>
		/// Howw many children this element has
		/// </summary>
		public int childrenCount => children != null ? children.Count : 0;
		/// <summary>
		/// How many children in total this element has (including subchildren)
		/// </summary>
		public int totalChildrenCount => GetTotalChildrenCount(this);
		/// <summary>
		/// How many children in total this element has (including subchildren)
		/// </summary>
		public StratusTreeElement[] allChildren => GetAllChildren(this);
		#endregion

		#region Constructors
		public StratusTreeElement()
		{
		}

		public StratusTreeElement(string name, int depth, int id)
		{
			this.name = name;
			this.depth = depth;
			this.id = id;
		}
		#endregion

		#region Messages
		public override string ToString()
		{
			return $"{name} id({id}) depth({depth})";
		}

		#endregion

	}

	/// <summary>
	/// Generic class for a tree element with one primary data member
	/// </summary>
	/// <typeparam name="DataType"></typeparam>
	public abstract class StratusTreeElement<DataType> : StratusTreeElement
	  where DataType : class, IStratusNamed
	{
		#region Fields
		public DataType data;
		public string dataTypeName;
		#endregion

		#region Properties
		public bool hasData => data != null;
		public Type dataType => data.GetType();
		public DataType[] childrenData { get; protected set; }
		#endregion

		#region Constructors
		public StratusTreeElement(DataType data)
		{
			name = data.name;
			Set(data);
		}

		public StratusTreeElement()
		{
		}

		public StratusTreeElement(string name, int depth, int id) : base(name, depth, id)
		{
		}
		#endregion



		#region Methods
		public void Set(DataType data)
		{
			this.data = data;
			this.dataTypeName = data.GetType().Name;
			this.UpdateName();
		}

		public void UpdateName()
		{
			this.name = this.GetName();
		}

		protected virtual string GetName()
		{
			return data.name;
		}

		public DataType[] GetChildrenData()
		{
			if (!this.hasChildren)
				return null;

			List<DataType> children = new List<DataType>();
			foreach (var child in this.children)
			{
				children.Add(((StratusTreeElement<DataType>)child).data);
			}
			return children.ToArray();
		}

		public StratusTreeElement<DataType> GetChild(int index) => (StratusTreeElement<DataType>)children[index];
		public T GetParent<T>() where T : StratusTreeElement => (T)parent;
		#endregion
	}
}