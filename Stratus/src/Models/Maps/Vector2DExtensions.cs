using Stratus.Numerics;

using System;

namespace Stratus.Models.Maps
{
	public static class Vector2DExtensions
	{
		public static CardinalDirection DirectionTo(this Vector2Int source, Vector2Int target)
		{
			var offset = target - source;
			if (offset.x > 0)
			{
				return CardinalDirection.East;
			}
			else if (offset.x < 0)
			{
				return CardinalDirection.West;
			}
			else if (offset.y > 0)
			{
				return CardinalDirection.North;
			}
			else if (offset.y < 0)
			{
				return CardinalDirection.South;
			}

			throw new ArgumentException("No valid direction");
		}
	}
}