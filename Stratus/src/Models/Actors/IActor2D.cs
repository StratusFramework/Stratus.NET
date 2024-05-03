using Stratus.Extensions;
using Stratus.Models.Actors;
using Stratus.Numerics;

using System;
using System.Linq;

namespace Stratus.Models.Maps
{
	/// <summary>
	/// An actor in 2D
	/// </summary>
	public interface IActor2D : IObject2D
	{
		/// <summary>
		/// How much the actor can change its position.
		/// This could be its movement speed, etc...
		/// </summary>
		int range { get; }
		/// <summary>
		/// Invoked whenever the actor's position changes
		/// </summary>
		event Action<Vector2Int> onMoved;
		/// <summary>
		/// An offset from the actor's current position
		/// </summary>
		Vector2Int[] Offset(int range, CellLayout layout)
		{
			switch (layout)
			{
				case CellLayout.Rectangle:
					return GridUtility.SquareOffset(range, cellPosition).ToArray();

				case CellLayout.Hexagon:
					throw new NotImplementedException("Offset not implemented for hexagon layout");
			}
			return new Vector2Int[0];
		}

	}

	public abstract class Actor2DCommand : Command
	{
		public IActor2D actor { get; }

		public Actor2DCommand(IActor2D actor)
		{
			this.actor = actor;
		}

		public TActor As<TActor>() where TActor : IActor2D => (TActor)actor;
	}
}