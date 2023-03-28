using Stratus.Numerics;
using Stratus.Search;

using System;

namespace Stratus.Models.Maps
{
	public class GridSearch : StratusSearch<Vector2Int>
	{
		private GridSearch()
		{
		}

		#region Ranges
		/// <summary>
		/// Returns the cell range given an origin
		/// </summary>
		public static GridRange GetRange(Vector2Int origin,
			GridSearchRangeArguments range,
			CellLayout layout)
		{
			GridRange result = null;
			switch (layout)
			{
				case CellLayout.Rectangle:
					result = GetRangeRectangle(origin, range);
					break;
				case CellLayout.Hexagon:
					result = GetRangeHexOffset(origin, range);
					break;
			}
			return result;
		}

		/// <summary>
		/// Returns the cell range given an origin and a range
		/// </summary>
		/// <param name="origin">The origin, from which to search from</param>
		/// <param name="n">The search range from the origin</param>
		/// <param name="predicate">A predicate that validates whether a given cell is traversible</param>
		/// <returns>A dictionary of all the elements in range along with the cost to traverse to them </returns>
		public static GridRange GetRangeRectangle(Vector2Int origin, GridSearchRangeArguments args)
		{
			RangeSearch search = new RangeSearch()
			{
				debug = false,
				distanceFunction = GridUtility.ManhattanDistance,
				traversalCostFunction = args.traversalCostFunction,
				neighborFunction = GridUtility.FindNeighboringCellsRectangle,
				traversableFunction = args.traversableFunction,
				range = args.maximum,
				startElement = origin
			};

			return new GridRange(search.SearchWithCosts());
		}

		/// <summary>
		/// Returns the cell range for a hexagon
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="n"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static GridRange GetRangeHexOffset(Vector2Int origin, GridSearchRangeArguments args,
			StratusTraversalPredicate<Vector2Int> predicate = null)
		{
			RangeSearch search = new RangeSearch()
			{
				debug = false,
				distanceFunction = GridUtility.HexOffsetDistance,
				neighborFunction = GridUtility.FindNeighboringCellsHexOffset,
				traversableFunction = predicate,
				range = args.maximum,
				startElement = origin
			};
			return new GridRange(search.SearchWithCosts());
		}
		#endregion

		#region Path
		public static Vector2Int[] FindPath(Vector2Int origin, Vector2Int target, CellLayout layout,
			StratusTraversalPredicate<Vector2Int> traversablePredicate = null)
		{
			Vector2Int[] result = null;
			switch (layout)
			{
				case CellLayout.Rectangle:
					result = FindRectanglePath(origin, target, traversablePredicate);
					break;
				case CellLayout.Hexagon:
					result = FindHexOffsetPath(origin, target, traversablePredicate);
					break;
			}
			return result;
		}

		public static Vector2Int[] FindRectanglePath(Vector2Int origin, Vector2Int target,
			StratusTraversalPredicate<Vector2Int> traversablePredicate = null)
		{
			var pathSearch = new GridSearch.PathSearch()
			{
				startElement = origin,
				targetElement = target,
				distanceFunction = GridUtility.ManhattanDistance,
				neighborFunction = GridUtility.FindNeighboringCellsRectangle,
				traversableFunction = traversablePredicate
			};
			return pathSearch.Search();
		}

		public static Vector2Int[] FindHexOffsetPath(Vector2Int origin, Vector2Int target,
			StratusTraversalPredicate<Vector2Int> traversablePredicate = null)
		{
			var pathSearch = new GridSearch.PathSearch()
			{
				startElement = origin,
				targetElement = target,
				distanceFunction = GridUtility.HexOffsetDistance,
				neighborFunction = GridUtility.FindNeighboringCellsHexOffset,
				traversableFunction = traversablePredicate
			};
			return pathSearch.Search();
		}
		#endregion
	}

	/// <summary>
	/// Arguments to perform a range search
	/// </summary>
	public class GridSearchRangeArguments
	{
		public GridSearchRangeArguments(int minimum, int maximum)
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		public GridSearchRangeArguments(int maximum)
		{
			this.minimum = 0;
			this.maximum = maximum;
		}

		public int minimum { get; }
		public int maximum { get; }
		public Func<Vector2Int, float> traversalCostFunction { get; set; }
		public StratusTraversalPredicate<Vector2Int> traversableFunction { get; set; }
	}
}
