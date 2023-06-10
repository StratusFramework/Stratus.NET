using Stratus.Extensions;
using Stratus.Numerics;
using Stratus.Search;

using System.Collections;
using System.Collections.Generic;

namespace Stratus.Models.Maps
{
	public class GridRange : SearchRange<Vector2Int, float>
	{
		public GridRange()
		{
		}

		public GridRange(IDictionary<Vector2Int, float> dictionary) : base(dictionary)
		{
		}

		public GridRange(IEqualityComparer<Vector2Int> comparer) : base(comparer)
		{
		}

		public GridRange(IEnumerable<KeyValuePair<Vector2Int, float>> collection) : base(collection)
		{
		}

		public GridRange(IDictionary<Vector2Int, float> dictionary, IEqualityComparer<Vector2Int> comparer) : base(dictionary, comparer)
		{
		}

		public GridRange(IEnumerable<KeyValuePair<Vector2Int, float>> collection, IEqualityComparer<Vector2Int> comparer) : base(collection, comparer)
		{
		}
	}

	public class GridPath : IEnumerable<Vector2Int>
	{
		public Vector2Int[] cells { get; }

		public GridPath(Vector2Int[] cells)
		{
			this.cells = cells;
		}

		public override string ToString()
		{
			return cells.ToStringJoin(",").Enclose(StratusStringEnclosure.SquareBracket);
		}

		public IEnumerator<Vector2Int> GetEnumerator()
		{
			return ((IEnumerable<Vector2Int>)this.cells).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.cells.GetEnumerator();
		}
	}


}
