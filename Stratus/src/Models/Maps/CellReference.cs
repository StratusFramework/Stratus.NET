using Stratus.Numerics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Models.Maps
{
	public interface ICellReference : IObject2D
	{
	}

	/// <summary>
	/// A reference to a cell that can either be static (a tile painted from a tileset)
	/// or dynamic (an object instantiated on top of the tilemap, such as an actor)
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <typeparam name="TData"></typeparam>
	public class CellReference<TObject, TData> : ICellReference, IEquatable<CellReference<TObject, TData>>
		where TObject : class, IObject2D
		where TData : class
	{
		/// <summary>
		/// For active objects on a tile
		/// </summary>
		public TObject? actor { get; }
		/// <summary>
		/// For static tiles (painted from a tileset)
		/// </summary>
		public TData? data { get; }
		/// <summary>
		/// Used in conjuntion with <see cref="data"/> to differentiate
		/// since data is shared among all tiles of a given type
		/// </summary>
		public Vector2Int? position { get; }

		string IObject2D.name => actor.name ?? data.ToString();
		Vector2Int IObject2D.cellPosition
		{
			get
			{
				if (actor != null)
				{
					return actor.cellPosition;
				}
				return position.Value;
			}
		}

		public CellReference(TData data, Vector2Int position)
		{
			this.data = data;
			this.position = position;
		}

		public CellReference(TObject actor)
		{
			this.actor = actor;
		}

		public bool Equals(CellReference<TObject, TData>? other)
		{
			return actor == other.actor && data == other.data
				&& position == other.position;
		}

		public override bool Equals(object? obj) => Equals(obj as CellReference<TObject, TData>);

		public override int GetHashCode()
		{
			return actor?.GetHashCode() ?? data.GetHashCode();
		}

		public override string ToString()
		{
			return actor != null ? actor.ToString() : data.ToString();
		}
	}
}
