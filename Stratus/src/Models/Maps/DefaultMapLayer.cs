using Stratus.Numerics;

namespace Stratus.Models.Maps
{
	public enum DefaultMapLayer
	{
		/// <summary>
		/// THe base layer (ground tile)
		/// </summary>
		Terrain,
		/// <summary>
		/// Walls and other obstacles that prevent default movement
		/// </summary>
		Wall,
		/// <summary>
		/// Static objects
		/// </summary>
		Object,
		/// <summary>
		/// Dynamic objects (such as characters in scene, etc)
		/// </summary>
		Actor,
		/// <summary>
		/// An event that is triggered when an actor steps into
		/// </summary>
		Event,
	}

	public class Grid2D<TObject> : Grid2D<TObject, DefaultMapLayer>
	where TObject : class
	{
		protected Grid2D(Vector2Int size, CellLayout layout) : base(size, layout)
		{
		}
	}
}