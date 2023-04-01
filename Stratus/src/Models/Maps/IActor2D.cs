using Stratus.Events;
using Stratus.Models;
using Stratus.Numerics;

using System;
using System.Numerics;

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
	}

	public class MovementRangeEvent : RequestResponseEvent<IActor2D, GridRange>
	{
	}
}
