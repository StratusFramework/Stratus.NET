using Stratus.Events;
using Stratus.Extensions;
using Stratus.Numerics;

using System;
using System.Collections.Generic;
using System.Linq;
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