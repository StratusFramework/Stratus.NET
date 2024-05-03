using Stratus.Events;
using Stratus.Models.Maps;

namespace Stratus.Models.Actors
{
    public interface IObject
    {
        /// <summary>
        /// The name of the object
        /// </summary>
        string name { get; }
    }

    /// <summary>
    /// An actor is an object with agency, such as a character
    /// </summary>
	public interface IActor : IObject
	{
	}

    /// <summary>
    /// An event to signal interaction with 
    /// </summary>
	public record InteractEvent(IActor actor) : Event
    {
    }
}