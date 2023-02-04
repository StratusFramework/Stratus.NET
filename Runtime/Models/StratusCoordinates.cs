using Stratus.Utilities;

using System;
using System.Numerics;

namespace Stratus
{
	/// <summary>
	/// Provides common functions related to mapping coordinate systems
	/// </summary>
	public static class StratusCoordinates
	{
		/// <summary>
		/// The eight cardinal directions or cardinal points are the directions north, east, south, and west and the directions halfway between each of these points.
		/// </summary>
		public enum CardinalDirection
		{
			North,
			South,
			West,
			East,
			NorthWest,
			NorthEast,
			SouthWest,
			SouthEast
		}

		/// <summary>
		/// The axis in 3D space
		/// </summary>
		[Flags]
		public enum Axis
		{
			X = 1 << 0,
			Y = 1 << 1,
			Z = 1 << 2
		}

		public static Axis UnsetAll(this Axis axis)
		{
			axis &= ~Axis.X;
			axis &= ~Axis.Y;
			axis &= ~Axis.Z;
			return axis;
		}

		/// <summary>
		/// Returns a random cardinal direction
		/// </summary>
		/// <returns></returns>
		public static CardinalDirection randomCardinalDirection => (CardinalDirection)StratusRandom.Range(0, 7);

		/// <summary>
		/// 2D arrays use the row/column scheme, where rows are descending.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public static Vector2 RowColumnToCartesian(int row, int col) => new Vector2(col, -row);

		public static T GetNeighbor<T>(this CardinalDirection direction, T[,] grid, int row, int col, bool wrap = false) => GetNeighbor(grid, row, col, direction, wrap);

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
						neighborRow = ((row - 1) + n) % (n);
						neighborCol = ((col - 1) + n) % (n);
						//neighbor = neighbors[r - 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.NorthEast:
						neighborRow = ((row - 1) + n) % (n);
						neighborCol = (col + 1) % (n);
						//neighbor = neighbors[r - 1 % n - 1, c + 1 % n - 1];
						break;
					case CardinalDirection.North:
						neighborRow = ((row - 1) + n) % (n);
						neighborCol = col;
						//neighbor = neighbors[r - 1 % n - 1, c];
						break;
					case CardinalDirection.West:
						neighborRow = row;
						neighborCol = ((col - 1) + n) % (n);
						//neighbor = neighbors[r, c - 1 % n - 1];
						break;
					case CardinalDirection.East:
						neighborRow = row;
						neighborCol = (col + 1) % (n);
						//neighbor = neighbors[r, c + 1 % n - 1];
						break;
					case CardinalDirection.South:
						neighborRow = (row + 1) % (n);
						neighborCol = col;
						//neighbor = neighbors[r + 1 % n - 1, c];
						break;
					case CardinalDirection.SouthWest:
						neighborRow = (row + 1) % (n);
						neighborCol = ((col - 1) + n) % (n);
						//neighbor = neighbors[r + 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.SouthEast:
						neighborRow = (row + 1) % (n);
						neighborCol = (col + 1) % (n);
						//neighbor = neighbors[r + 1 % n - 1, c + 1 % n - 1];
						break;
				}
			}
			else
			{
				switch (direction)
				{
					case CardinalDirection.NorthWest:
						neighborRow = (row - 1) % (n);
						if (neighborRow < 0) neighborRow = 0;
						neighborCol = (col - 1) % (n);
						if (neighborCol < 0) neighborCol = 0;
						//neighbor = neighbors[r - 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.NorthEast:
						neighborRow = (row - 1) % (n);
						if (neighborRow < 0) neighborRow = 0;
						neighborCol = (col + 1) % (n);
						//neighbor = neighbors[r - 1 % n - 1, c + 1 % n - 1];
						break;
					case CardinalDirection.North:
						neighborRow = (row - 1) % (n);
						if (neighborRow < 0) neighborRow = 0;
						neighborCol = col;
						//neighbor = neighbors[r - 1 % n - 1, c];
						break;
					case CardinalDirection.West:
						neighborRow = row;
						neighborCol = (col - 1) % (n);
						if (neighborCol < 0) neighborCol = 0;
						//neighbor = neighbors[r, c - 1 % n - 1];
						break;
					case CardinalDirection.East:
						neighborRow = row;
						neighborCol = (col + 1) % (n);
						//neighbor = neighbors[r, c + 1 % n - 1];
						break;
					case CardinalDirection.South:
						neighborRow = (row + 1) % (n);
						neighborCol = col;
						//neighbor = neighbors[r + 1 % n - 1, c];
						break;
					case CardinalDirection.SouthWest:
						neighborRow = (row + 1) % (n);
						neighborCol = (col - 1) % (n);
						if (neighborCol < 0) neighborCol = 0;
						//neighbor = neighbors[r + 1 % n - 1, c - 1 % n - 1];
						break;
					case CardinalDirection.SouthEast:
						neighborRow = (row + 1) % (n);
						neighborCol = (col + 1) % (n);
						break;
				}
			}



			//Trace.Script($"(From) = {row},{col} to ({direction}) = {neighborRow},{neighborCol}");
			return neighbors[neighborRow, neighborCol];

		}
	}
}