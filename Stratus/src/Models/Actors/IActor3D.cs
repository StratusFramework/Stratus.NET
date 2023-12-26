using Stratus.Events;

using System.Numerics;

namespace Stratus.Models.Actors
{
	public interface IActor3D
	{
		/// <summary>
		/// The current position of the actor
		/// </summary>
		Vector3 position { get; }
	}
}