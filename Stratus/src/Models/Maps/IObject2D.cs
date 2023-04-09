using Stratus.Numerics;

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
	}

	public class Object2D : IObject2D
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
	}
}