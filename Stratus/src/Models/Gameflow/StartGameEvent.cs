using Stratus.Events;

namespace Stratus.Models.Gameflow
{
	/// <summary>
	/// Start a new game
	/// </summary>
	public record StartGameEvent : Event
	{
	}

	/// <summary>
	/// Continue an existing game
	/// </summary>
	public record ContinueGameEvent : Event
	{
	}

	/// <summary>
	/// End the current session
	/// </summary>
	public record EndGameEvent : Event
	{
	}
}
