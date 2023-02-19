using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Stratus.Models
{
	public struct StratusVector3Int
	{
		public int x { get; set; }
		public int y { get; set; }
		public int z { get; set; }

		public StratusVector3Int(Vector3 value)
		{
			x = (int)value.X;
			y = (int)value.Y;
			z = (int)value.Z;
		}

		public StratusVector3Int(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public StratusVector3Int(int x, int y)
		{
			this.x = x;
			this.y = y;
			this.z = 0;
		}

		public static implicit operator StratusVector3Int(Vector3 value)
		{
			return new StratusVector3Int(value);
		}

		public static implicit operator Vector3(StratusVector3Int value)
		{
			return new Vector3(value.x, value.y, value.z);
		}

		public static float Distance(StratusVector3Int a, StratusVector3Int b)
			=> Vector3.Distance(a, b);
	}

	public struct StratusVector3
	{
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }

		public StratusVector3(Vector3 value)
		{
			x = value.X;
			y = value.Y;
			z = value.Z;
		}

		public static implicit operator Vector3(StratusVector3 value)
		{
			return new Vector3(value.x, value.y, value.z);
		}
	}
}
