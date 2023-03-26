using Stratus.Events;
using Stratus.Models;
using Stratus.Numerics;

using System.Numerics;

namespace Stratus.Models.Maps
{
    public interface IActor2D
    {
        string name { get; }
        /// <summary>
        /// How much the actor can change its position.
        /// This could be its movement speed, etc...
        /// </summary>
        int range { get; }
        Vector2Int cellPosition { get; }
    }

    public class MovementRangeEvent : RequestResponseEvent<IActor2D, GridRange>
    {
    }
}
