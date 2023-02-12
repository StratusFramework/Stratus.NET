using Stratus.Models.Math;

using System;
using System.Numerics;

namespace Stratus.Models.Maps
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

	public static class AxisExtensions
	{
		public static Axis UnsetAll(this Axis axis)
		{
			axis &= ~Axis.X;
			axis &= ~Axis.Y;
			axis &= ~Axis.Z;
			return axis;
		}

		public static T GetNeighbor<T>(this CardinalDirection direction, T[,] grid, int row, int col, bool wrap = false) => GridUtility.GetNeighbor(grid, row, col, direction, wrap);
	}
}