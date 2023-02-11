using System;
using System.Collections.Generic;
using System.Numerics;

namespace Stratus.Models.Maps
{
	public abstract class StratusGridUtility
	{
		public class GridSearch : StratusSearch<StratusVector3Int>
		{
			private GridSearch()
			{
			}
		}

		public static readonly HexagonalOddRowDirection[] oddRowDirections = (HexagonalOddRowDirection[])Enum.GetValues(typeof(HexagonalOddRowDirection));


		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		public static Vector3 CubeLerp(StratusVector3Int a, StratusVector3Int b, float t)
		{
			return new Vector3(Lerp(a.x, b.x, t),
							   Lerp(a.y, b.y, t),
							   Lerp(a.z, b.z, t));
		}

		#region Conversions
		public static StratusVector3Int CubeRound(StratusVector3Int cube)
		{
			int rx = (int)MathF.Round(cube.x);
			int ry = (int)MathF.Round(cube.y);
			int rz = (int)MathF.Round(cube.z);

			var x_diff = MathF.Abs(rx - cube.x);
			var y_diff = MathF.Abs(ry - cube.y);
			var z_diff = MathF.Abs(rz - cube.z);

			if (x_diff > y_diff && x_diff > z_diff)
			{
				rx = -ry - rz;
			}
			else if (y_diff > z_diff)
			{
				ry = -rx - rz;
			}
			else
			{
				rz = -rx - ry;
			}

			return new StratusVector3Int(rx, ry, rz);
		}

		public static StratusVector3Int OffsetRound(StratusVector3Int offset)
		{
			var cube = OffsetToCube(offset);
			return CubeToOffset(CubeRound(cube));
		}

		public static StratusVector3Int OffsetToAxialXY(StratusVector3Int value)
		{
			value.y = value.y - (int)MathF.Floor(value.x / 2f);
			return value;
		}

		public static StratusVector3Int AxialToOffsetCoordinatesXY(StratusVector3Int value)
		{
			value.y = value.y + (int)MathF.Floor(value.x / 2f);
			return value;
		}

		public static StratusVector3Int OffsetToCube(StratusVector3Int value)
		{
			return OffsetOddRowToCube(value);
		}

		public static StratusVector3Int OffsetOddRowToCube(StratusVector3Int value)
		{
			int x = value.x - (value.y - (value.y & 1)) / 2;
			int z = value.y;
			int y = -x - z;
			return new StratusVector3Int(x, y, z);
		}

		public static StratusVector3Int CubeToOffset(StratusVector3Int value)
		{
			return CubeToOffsetOddRow(value);
		}

		public static StratusVector3Int CubeToOffsetOddRow(StratusVector3Int value)
		{
			int col = value.x + (value.z - (value.z & 1)) / 2;
			int row = value.z;
			return new StratusVector3Int(col, row, 0);
		}

		public static StratusVector3Int CubeToAxial(StratusVector3Int cube)
		{
			var q = cube.x;
			var r = cube.z;
			return new StratusVector3Int(q, r, 0);
		}

		public static StratusVector3Int AxialToCube(StratusVector3Int axial)
		{
			var x = axial.x;
			var z = axial.y;
			var y = -x - z;
			return new StratusVector3Int(x, y, z);
		}
		#endregion

		#region Distances
		public static float HexCubeDistance(StratusVector3Int a, StratusVector3Int b)
		{
			// return (abs(a.x - b.x) + abs(a.y - b.y) + abs(a.z - b.z)) / 2
			float distance = (MathF.Abs(a.x - b.x) + MathF.Abs(a.y - b.y) + MathF.Abs(a.z - b.z)) / 2f;
			return distance;
		}

		public static float HexOffsetDistance(StratusVector3Int a, StratusVector3Int b)
		{
			var ac = OffsetToCube(a);
			var bc = OffsetToCube(b);
			return HexCubeDistance(ac, bc);
		}

		public static float RectangleDistance(StratusVector3Int a, StratusVector3Int b, bool diagonal)
		{
			return diagonal ? EuclideanDistance(a, b) : ManhattanDistance(a, b);
		}

		/// <summary>
		/// The square root of the sum of the squares of the differences of the coordinates.
		/// </summary>
		public static float EuclideanDistance(StratusVector3Int a, StratusVector3Int b)
		{
			return Vector3.Distance(a, b);
		}

		/// <summary>
		/// The sum of the absolute values of the differences of the coordinates.
		/// That is, the number of cells you must travel vertically, plus the number of cells you 
		/// must travel horizontally, much like a taxi driving through a grid of city streets.
		/// </summary>
		public static float ManhattanDistance(StratusVector3Int a, StratusVector3Int b)
		{
			return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
		}
		#endregion

		#region Neighbors
		private static StratusVector3Int[][] oddRowDirectionValues = new StratusVector3Int[][]
		{
			new StratusVector3Int[]
			{
				new StratusVector3Int(1, 0, 0), new StratusVector3Int(0, -1, 0), new StratusVector3Int(-1, -1, 0),
				new StratusVector3Int(-1, 0, 0), new StratusVector3Int(-1, 1, 0), new StratusVector3Int(0, 1, 0)
			},
			new StratusVector3Int[]
			{
				new StratusVector3Int(1, 0, 0), new StratusVector3Int(1, -1, 0), new StratusVector3Int(0, -1, 0),
				new StratusVector3Int(-1, 0, 0), new StratusVector3Int(0, 1, 0), new StratusVector3Int(1, 1, 0)
			}
		};

		/// <summary>
		/// Where x = col, y = row
		/// </summary>
		/// <param name="hex"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static StratusVector3Int FindNeighboringCellsOddRow(StratusVector3Int hex,
			HexagonalOddRowDirection direction)
		{
			int parity = hex.y & 1;
			StratusVector3Int offset = oddRowDirectionValues[parity][(int)direction];
			StratusVector3Int neighbor = new StratusVector3Int(hex.x + offset.x, hex.y + offset.y, 0);
			return neighbor;
		}

		public static StratusVector3Int[] FindNeighboringCellsHexOffset(StratusVector3Int hex)
		{
			List<StratusVector3Int> result = new List<StratusVector3Int>();
			for (int i = 0; i < oddRowDirections.Length; ++i)
			{
				result.Add(FindNeighboringCellsOddRow(hex, oddRowDirections[i]));
			}
			return result.ToArray();
		}

		public static StratusVector3Int[] FindNeighboringCellsRectangle(StratusVector3Int element)
		{
			List<StratusVector3Int> result = new List<StratusVector3Int>();
			result.Add(new StratusVector3Int(element.x + 1, element.y, element.z));
			result.Add(new StratusVector3Int(element.x - 1, element.y, element.z));
			result.Add(new StratusVector3Int(element.x, element.y + 1, element.z));
			result.Add(new StratusVector3Int(element.x, element.y - 1, element.z));
			return result.ToArray();
		}

		public static StratusVector3Int[] FindNeighboringCells(StratusVector3Int element, CellLayout layout)
		{
			StratusVector3Int[] result = null;
			switch (layout)
			{
				case CellLayout.Rectangle:
					result = FindNeighboringCellsRectangle(element);
					break;
				case CellLayout.Hexagon:
					result = FindNeighboringCellsHexOffset(element);
					break;
			}
			return result;
		}
		#endregion

		#region Ranges
		/// <summary>
		/// Returns the cell range given an origin
		/// </summary>
		public static StratusGridRange GetRange(StratusVector3Int origin,
			StratusGridSearchRangeArguments range,
			CellLayout layout)
		{
			StratusGridRange result = null;
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
		public static StratusGridRange GetRangeRectangle(StratusVector3Int origin, StratusGridSearchRangeArguments args)
		{
			GridSearch.RangeSearch search
				= new GridSearch.RangeSearch()
				{
					debug = false,
					distanceFunction = ManhattanDistance,
					traversalCostFunction = args.traversalCostFunction,
					neighborFunction = FindNeighboringCellsRectangle,
					traversableFunction = args.traversableFunction,
					range = args.maximum,
					startElement = origin
				};

			return new StratusGridRange(search.SearchWithCosts());
		}

		/// <summary>
		/// Returns the cell range for a hexagon
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="n"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public static StratusGridRange GetRangeHexOffset(StratusVector3Int origin, StratusGridSearchRangeArguments args,
			StratusTraversalPredicate<StratusVector3Int> predicate = null)
		{
			GridSearch.RangeSearch search
			= new GridSearch.RangeSearch()
			{
				debug = false,
				distanceFunction = HexOffsetDistance,
				neighborFunction = FindNeighboringCellsHexOffset,
				traversableFunction = predicate,
				range = args.maximum,
				startElement = origin
			};
			return new StratusGridRange(search.SearchWithCosts());
		}
		#endregion

		#region Path
		public static StratusVector3Int[] FindPath(StratusVector3Int origin, StratusVector3Int target, CellLayout layout,
			StratusTraversalPredicate<StratusVector3Int> traversablePredicate = null)
		{
			StratusVector3Int[] result = null;
			switch (layout)
			{
				case CellLayout.Rectangle:
					result = FindRectanglePath(origin, target, traversablePredicate);
					break;
				case CellLayout.Hexagon:
					result = FindHexOffsetPath(origin, target, traversablePredicate);
					break;
					break;
			}
			return result;
		}

		public static StratusVector3Int[] FindRectanglePath(StratusVector3Int origin, StratusVector3Int target,
			StratusTraversalPredicate<StratusVector3Int> traversablePredicate = null)
		{
			var pathSearch = new GridSearch.PathSearch()
			{
				startElement = origin,
				targetElement = target,
				distanceFunction = ManhattanDistance,
				neighborFunction = FindNeighboringCellsRectangle,
				traversableFunction = traversablePredicate
			};
			return pathSearch.Search();
		}

		public static StratusVector3Int[] FindHexOffsetPath(StratusVector3Int origin, StratusVector3Int target,
			StratusTraversalPredicate<StratusVector3Int> traversablePredicate = null)
		{
			var pathSearch = new GridSearch.PathSearch()
			{
				startElement = origin,
				targetElement = target,
				distanceFunction = HexOffsetDistance,
				neighborFunction = FindNeighboringCellsHexOffset,
				traversableFunction = traversablePredicate
			};
			return pathSearch.Search();
		}
		#endregion
	}
}
