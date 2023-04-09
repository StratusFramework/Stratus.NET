using Stratus.Extensions;
using Stratus.Numerics;
using Stratus.Search;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Stratus.Models
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

	/// <summary>
	/// The cells and bounds of a constructed 2D grid
	/// </summary>
	public class Bounds2D
	{
		public int xMax { get; private set; }
		public int yMax { get; private set; }
		public int xMin { get; private set; }
		public int yMin { get; private set; }
		public Vector2Int[] cells => _cells.Value;

		private Lazy<Vector2Int[]> _cells;

		public Bounds2D(Vector2Int size)
		{
			WithSize(size);
		}

		public Bounds2D WithSize(int size)
		{
			return WithSize(new Vector2Int(size, size));
		}

		public Bounds2D WithSize(Vector2Int size)
		{
			xMin = yMin = 0;
			yMax = size.y - 1;
			xMax = size.x - 1;
			UpdateCells();
			return this;
		}

		private void UpdateCells()
		{
			_cells = new Lazy<Vector2Int[]>(() =>
			{
				List<Vector2Int> result = new();
				for (int x = xMin; x <= xMax; x++)
				{
					for (int y = yMin; y <= yMax; y++)
					{
						result.Add(new Vector2Int(x, y));
					}
				}
				return result.ToArray();
			});
		}

		public bool Contains(Vector2Int position) => Contains(position.x, position.y);

		public bool Contains(int x, int y)
		{
			return (x >= xMin && x <= xMax)
				&& (y >= yMin && y <= yMax);
		}
	}
}
