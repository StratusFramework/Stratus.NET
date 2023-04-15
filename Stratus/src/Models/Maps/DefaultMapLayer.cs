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
		/// Walls and other static obstacles that prevent default movement
		/// </summary>
		Wall,
		/// <summary>
		/// Dynamic objects that block such as doors or gates
		/// </summary>
		Portal,
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

	public class DefaultGrid : Grid2D<DefaultMapLayer>
	{
		protected DefaultGrid(Vector2Int size, CellLayout layout) : base(size, layout)
		{
		}
	}
}