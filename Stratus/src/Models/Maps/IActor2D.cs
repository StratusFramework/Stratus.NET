using Stratus.Events;
using Stratus.Models;
using Stratus.Numerics;

using System;
using System.Numerics;

namespace Stratus.Models.Maps
{
    public interface IActor2D
    {
        /// <summary>
        /// The name of the actor
        /// </summary>
        string name { get; }
        /// <summary>
        /// How much the actor can change its position.
        /// This could be its movement speed, etc...
        /// </summary>
        int range { get; }
        /// <summary>
        /// The actor's current position n the grid
        /// </summary>
        Vector2Int cellPosition { get; }
        /// <summary>
        /// Invoked whenever the actor's position changes
        /// </summary>
        event Action<Vector2Int> onMoved;
    }

    public class MovementRangeEvent : RequestResponseEvent<IActor2D, GridRange>
    {
    }
}
