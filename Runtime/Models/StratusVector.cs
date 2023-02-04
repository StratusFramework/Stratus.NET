using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Stratus
{
    public struct StratusVector3Int
    {
        public int x { get; }
        public int y { get; }
        public int z { get; }

        public StratusVector3Int(Vector3 value)
        {
            x = (int)value.X;
            y = (int)value.Y;
            z = (int)value.Z;
        }
	}
}
