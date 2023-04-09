using Stratus.Numerics;

using System;

namespace Stratus.Models.Maps
{
	public interface IObject2D
	{
		/// <summary>
		/// The name of the object
		/// </summary>
		string name { get; }
		/// <summary>
		/// The current position on the grid
		/// </summary>
		Vector2Int cellPosition { get; }
		/// <summary>
		/// Casts this object onto a derived one
		/// </summary>
		/// <typeparam name="TObject">An object type</typeparam>
		/// <returns></returns>
		TObject As<TObject>() where TObject : IObject2D => (TObject)this;
	}

	public class Object2D : IObject2D, IEquatable<Object2D>
	{
		public string name { get; }
		public Vector2Int cellPosition { get; }

		public Object2D(string name, Vector2Int cellPosition)
		{
			this.name = name;
			this.cellPosition = cellPosition;
		}

		public override string ToString()
		{
			return name;
		}

		public bool Equals(Object2D? other)
		{
			return name == other.name && cellPosition == other.cellPosition;
		}
	}

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