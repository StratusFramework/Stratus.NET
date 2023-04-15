using Stratus.Data;
using Stratus.Numerics;

using System;

namespace Stratus.Models.Maps
{
	public interface IObject2D
	{
		/// <summary>
		/// The name of the object
		/// </summary>
		string name { get; }
		/// <summary>
		/// The layer the object is in
		/// </summary>
		Layer layer { get; }
		/// <summary>
		/// The current position on the grid
		/// </summary>
		Vector2Int cellPosition { get; }
		/// <summary>
		/// Casts this object onto a derived one
		/// </summary>
		/// <typeparam name="TObject">An object type</typeparam>
		/// <returns></returns>
		TObject As<TObject>() where TObject : IObject2D => (TObject)this;
	}

	public class Object2D : IObject2D, IEquatable<Object2D>
	{
		private ValueProvider<bool> _blocking;
		private ValueProvider<Vector2Int> _cellPosition;

		public string name { get; }
		public Vector2Int cellPosition => _cellPosition.value;
		public Layer layer { get; }
		public bool blocking => _blocking != null ? _blocking.value : false;

		public Object2D(string name, Layer layer, ValueProvider<Vector2Int> cellPosition)
		{
			this.name = name;
			this.layer = layer;
			this._cellPosition = cellPosition;
		}

		public override string ToString()
		{
			return name;
		}

		public bool Equals(Object2D? other)
		{
			return name == other.name && cellPosition == other.cellPosition;
		}

		public void WithBlocking(ValueProvider<bool> provider)
		{

		}
	}

	/// <summary>
	/// A portal could be a door, gate, etc...
	/// </summary>
	public interface IPortal2D : IObject2D
	{
		bool open { get; set; }
	}

	public enum PortalState
	{
		Open,
		Closed
	}

	public class Portal2D : Object2D, IPortal2D
	{
		public bool open
		{
			get => _open.value;
			set => _open.value = value;
		}
		private PropertyReference<bool> _open;
		public PortalState state => open ? PortalState.Open : PortalState.Closed;

		public Portal2D(string name, Layer layer, Vector2Int cellPosition,
			PropertyReference<bool> open)
			: base(name, layer, cellPosition)
		{
			_open = open;
		}

	}
}