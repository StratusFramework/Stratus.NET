using Stratus.Numerics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Models.Maps
{
	/// <summary>
	/// A reference to a cell that can either be static (a tile painted from a tileset)
	/// or dynamic (an object instantiated on top of the tilemap, such as an actor)
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <typeparam name="TData"></typeparam>
	public class CellReference<TObject, TData> : IEquatable<CellReference<TObject, TData>>
		where TObject : class
		where TData : class
	{
		/// <summary>
		/// For active objects on a tile
		/// </summary>
		public TObject? node { get; }
		/// <summary>
		/// For static tiles (painted from a tileset)
		/// </summary>
		public TData? data { get; }
		/// <summary>
		/// Used in conjuntion with <see cref="data"/> to differentiate
		/// since data is shared among all tiles of a given type
		/// </summary>
		public Vector2Int? position { get; }

		public CellReference(TData data, Vector2Int position)
		{
			this.data = data;
			this.position = position;
		}

		public CellReference(TObject node)
		{
			this.node = node;
		}

		public bool Equals(CellReference<TObject, TData>? other)
		{
			return node == other.node && data == other.data
				&& position == other.position;
		}

		public override bool Equals(object? obj) => Equals(obj as CellReference<TObject, TData>);

		public override int GetHashCode()
		{
			return node?.GetHashCode() ?? data.GetHashCode();
		}

		public override string ToString()
		{
			return node != null ? node.ToString() : data.ToString();
		}
	}
}
