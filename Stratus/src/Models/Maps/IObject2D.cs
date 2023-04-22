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
		Enumerated layer { get; }
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
		public Enumerated layer { get; }
		public bool blocking => _blocking != null ? _blocking.value : false;

		public Object2D(string name, Enumerated layer, ValueProvider<Vector2Int> cellPosition)
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
	}	
}