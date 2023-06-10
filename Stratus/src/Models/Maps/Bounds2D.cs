using Stratus.Numerics;

using System;
using System.Collections.Generic;

namespace Stratus.Models.Maps
{
	public interface IBounds2D
	{
		Vector2Int[] cells { get; }
		int xMax { get; }
		int yMax { get; }
		int xMin { get; }
		int yMin { get; }
	}

	/// <summary>
	/// The cells and bounds of a constructed 2D grid
	/// </summary>
	public class Bounds2D : IBounds2D
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
			return x >= xMin && x <= xMax
				&& y >= yMin && y <= yMax;
		}
	}
}
