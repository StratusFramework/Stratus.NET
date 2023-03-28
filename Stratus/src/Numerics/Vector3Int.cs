using System.Numerics;

namespace Stratus.Numerics
{
	public struct Vector3Int
	{
		public int x { get; set; }
		public int y { get; set; }
		public int z { get; set; }

		public Vector3Int(Vector3 value)
		{
			x = (int)value.X;
			y = (int)value.Y;
			z = (int)value.Z;
		}

		public Vector3Int(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3Int(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.z = 0;
		}

		public override string ToString()
		{
			return $"({x},{y},{z})";
		}

		public static implicit operator Vector3Int(Vector3 value)
		{
			return new Vector3Int(value);
		}

		public static implicit operator Vector3(Vector3Int value)
		{
			return new Vector3(value.x, value.y, value.z);
		}

		public static float Distance(Vector3Int a, Vector3Int b)
			=> Vector3.Distance(a, b);
	}
	
	public struct Vector2Int
	{
		public int x { get; set; }
		public int y { get; set; }

		public Vector2Int(Vector2 value)
		{
			x = (int)value.X;
			y = (int)value.Y;
		}

		public Vector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return $"({x},{y})";
		}

		public static implicit operator Vector2Int(Vector2 value)
		{
			return new Vector2Int(value);
		}

		public static implicit operator Vector2(Vector2Int value)
		{
			return new Vector2(value.x, value.y);
		}

		public static float Distance(Vector2Int a, Vector2Int b)
			=> Vector2.Distance(a, b);
	}

	public static class VectorIntExtensions
	{
		public static Vector3Int ToVector3Int(this Vector2Int value)
			=> new Vector3Int(value.x, value.y);
	}

}
