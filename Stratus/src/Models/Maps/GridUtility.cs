using Stratus.Models.Math;
using Stratus.Numerics;

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Stratus.Models.Maps
{
	public abstract class GridUtility
	{
		public static readonly HexagonalOddRowDirection[] oddRowDirections = (HexagonalOddRowDirection[])Enum.GetValues(typeof(HexagonalOddRowDirection));

		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		public static Vector3 CubeLerp(Vector3Int a, Vector3Int b, float t)
		{
			return new Vector3(Lerp(a.x, b.x, t),
							   Lerp(a.y, b.y, t),
							   Lerp(a.z, b.z, t));
		}

		#region Conversions
		public static Vector3Int CubeRound(Vector3Int cube)
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

			return new Vector3Int(rx, ry, rz);
		}

		public static Vector3Int OffsetRound(Vector3Int offset)
		{
			var cube = OffsetToCube(offset);
			return CubeToOffset(CubeRound(cube));
		}

		public static Vector3Int OffsetToAxialXY(Vector3Int value)
		{
			value.y = value.y - (int)MathF.Floor(value.x / 2f);
			return value;
		}

		public static Vector3Int AxialToOffsetCoordinatesXY(Vector3Int value)
		{
			value.y = value.y + (int)MathF.Floor(value.x / 2f);
			return value;
		}

		public static Vector3Int OffsetToCube(Vector3Int value)
		{
			return OffsetOddRowToCube(value);
		}

		public static Vector3Int OffsetOddRowToCube(Vector3Int value)
		{
			int x = value.x - (value.y - (value.y & 1)) / 2;
			int z = value.y;
			int y = -x - z;
			return new Vector3Int(x, y, z);
		}

		public static Vector3Int CubeToOffset(Vector3Int value)
		{
			return CubeToOffsetOddRow(value);
		}

		public static Vector3Int CubeToOffsetOddRow(Vector3Int value)
		{
			int col = value.x + (value.z - (value.z & 1)) / 2;
			int row = value.z;
			return new Vector3Int(col, row, 0);
		}

		public static Vector3Int CubeToAxial(Vector3Int cube)
		{
			var q = cube.x;
			var r = cube.z;
			return new Vector3Int(q, r, 0);
		}

		public static Vector3Int AxialToCube(Vector3Int axial)
		{
			var x = axial.x;
			var z = axial.y;
			var y = -x - z;
			return new Vector3Int(x, y, z);
		}
		#endregion

		#region Distances
		public static float HexCubeDistance(Vector3Int a, Vector3Int b)
		{
			// return (abs(a.x - b.x) + abs(a.y - b.y) + abs(a.z - b.z)) / 2
			float distance = (MathF.Abs(a.x - b.x) + MathF.Abs(a.y - b.y) + MathF.Abs(a.z - b.z)) / 2f;
			return distance;
		}

		public static float HexOffsetDistance(Vector2Int a, Vector2Int b)
		{
			var ac = OffsetToCube(a.ToVector3Int());
			var bc = OffsetToCube(b.ToVector3Int());
			return HexCubeDistance(ac, bc);
		}

		public static float RectangleDistance(Vector2Int a, Vector2Int b, bool diagonal)
		{
			return diagonal ? EuclideanDistance(a, b) : ManhattanDistance(a, b);
		}

		/// <summary>
		/// The square root of the sum of the squares of the differences of the coordinates.
		/// </summary>
		public static float EuclideanDistance(Vector2Int a, Vector2Int b)
		{
			return Vector2.Distance(a, b);
		}

		/// <summary>
		/// The sum of the absolute values of the differences of the coordinates.
		/// That is, the number of cells you must travel vertically, plus the number of cells you 
		/// must travel horizontally, much like a taxi driving through a grid of city streets.
		/// </summary>
		public static float ManhattanDistance(Vector2Int a, Vector2Int b)
		{
			return System.Math.Abs(a.x - b.x) + System.Math.Abs(a.y - b.y);
		}
		#endregion

		#region Neighbors
		private static Vector2Int[][] oddRowDirectionValues = new Vector2Int[][]
		{
			new Vector2Int[]
			{
				new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1),
				new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, 1)
			},
			new Vector2Int[]
			{
				new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
				new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1)
			}
		};

		/// <summary>
		/// Where x = col, y = row
		/// </summary>
		/// <param name="hex"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public static Vector2Int FindNeighboringCellsOddRow(Vector2Int hex,
			HexagonalOddRowDirection direction)
		{
			int parity = hex.y & 1;
			Vector2Int offset = oddRowDirectionValues[parity][(int)direction];
			Vector2Int neighbor = new Vector2Int(hex.x + offset.x, hex.y + offset.y);
			return neighbor;
		}

		public static Vector2Int[] FindNeighboringCellsHexOffset(Vector2Int hex)
		{
			List<Vector2Int> result = new List<Vector2Int>();
			for (int i = 0; i < oddRowDirections.Length; ++i)
			{
				result.Add(FindNeighboringCellsOddRow(hex, oddRowDirections[i]));
			}
			return result.ToArray();
		}

		public static Vector2Int[] FindNeighboringCellsRectangle(Vector2Int element)
		{
			List<Vector2Int> result = new();
			result.Add(new Vector2Int(element.x + 1, element.y));
			result.Add(new Vector2Int(element.x - 1, element.y));
			result.Add(new Vector2Int(element.x, element.y + 1));
			result.Add(new Vector2Int(element.x, element.y - 1));
			return result.ToArray();
		}

		public static Vector2Int[] FindNeighboringCells(Vector2Int element, CellLayout layout)
		{
			Vector2Int[] result = null;
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

		public static T GetNeighbor<T>(T[,] neighbors, int row, int col, CardinalDirection direction, bool wrap = false)
		{
			// (x + N - 1 % N, y + N - 1 % N)(x + N - 1 % N, y) (x + N - 1 % N, y + 1 % N)
			// (x, y + N - 1 % N)                               (x, y + 1 % N)
			// (x + 1 % N, y + N - 1 % N)(x + 1, y)             (x + 1 % N, y + 1 % N)

			//T neighbor = default(T);

			// 0-index
			int n = (int)MathF.Sqrt(neighbors.Length);
			int neighborRow = 0, neighborCol = 0;

			if (wrap)
			{
				switch (direction)
				{
					case CardinalDirection.NorthWest:
						neighborRow = (row - 1 + n) % n;
						neighborCol = (col - 1 + n) % n;
						//neighbor = neighbors[r - 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.NorthEast:
						neighborRow = (row - 1 + n) % n;
						neighborCol = (col + 1) % n;
						//neighbor = neighbors[r - 1 % n - 1, c + 1 % n - 1];
						break;
					case CardinalDirection.North:
						neighborRow = (row - 1 + n) % n;
						neighborCol = col;
						//neighbor = neighbors[r - 1 % n - 1, c];
						break;
					case CardinalDirection.West:
						neighborRow = row;
						neighborCol = (col - 1 + n) % n;
						//neighbor = neighbors[r, c - 1 % n - 1];
						break;
					case CardinalDirection.East:
						neighborRow = row;
						neighborCol = (col + 1) % n;
						//neighbor = neighbors[r, c + 1 % n - 1];
						break;
					case CardinalDirection.South:
						neighborRow = (row + 1) % n;
						neighborCol = col;
						//neighbor = neighbors[r + 1 % n - 1, c];
						break;
					case CardinalDirection.SouthWest:
						neighborRow = (row + 1) % n;
						neighborCol = (col - 1 + n) % n;
						//neighbor = neighbors[r + 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.SouthEast:
						neighborRow = (row + 1) % n;
						neighborCol = (col + 1) % n;
						//neighbor = neighbors[r + 1 % n - 1, c + 1 % n - 1];
						break;
				}
			}
			else
			{
				switch (direction)
				{
					case CardinalDirection.NorthWest:
						neighborRow = (row - 1) % n;
						if (neighborRow < 0) neighborRow = 0;
						neighborCol = (col - 1) % n;
						if (neighborCol < 0) neighborCol = 0;
						//neighbor = neighbors[r - 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.NorthEast:
						neighborRow = (row - 1) % n;
						if (neighborRow < 0) neighborRow = 0;
						neighborCol = (col + 1) % n;
						//neighbor = neighbors[r - 1 % n - 1, c + 1 % n - 1];
						break;
					case CardinalDirection.North:
						neighborRow = (row - 1) % n;
						if (neighborRow < 0) neighborRow = 0;
						neighborCol = col;
						//neighbor = neighbors[r - 1 % n - 1, c];
						break;
					case CardinalDirection.West:
						neighborRow = row;
						neighborCol = (col - 1) % n;
						if (neighborCol < 0) neighborCol = 0;
						//neighbor = neighbors[r, c - 1 % n - 1];
						break;
					case CardinalDirection.East:
						neighborRow = row;
						neighborCol = (col + 1) % n;
						//neighbor = neighbors[r, c + 1 % n - 1];
						break;
					case CardinalDirection.South:
						neighborRow = (row + 1) % n;
						neighborCol = col;
						//neighbor = neighbors[r + 1 % n - 1, c];
						break;
					case CardinalDirection.SouthWest:
						neighborRow = (row + 1) % n;
						neighborCol = (col - 1) % n;
						if (neighborCol < 0) neighborCol = 0;
						//neighbor = neighbors[r + 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.SouthEast:
						neighborRow = (row + 1) % n;
						neighborCol = (col + 1) % n;
						break;
				}
			}

			return neighbors[neighborRow, neighborCol];
		}

		/// <summary>
		/// Returns a random cardinal direction
		/// </summary>
		/// <returns></returns>
		public static CardinalDirection randomCardinalDirection => (CardinalDirection)RandomUtility.Range(0, 7);

		/// <summary>
		/// 2D arrays use the row/column scheme, where rows are descending.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public static Vector2 RowColumnToCartesian(int row, int col) => new Vector2(col, -row);
		#endregion
	}
}
