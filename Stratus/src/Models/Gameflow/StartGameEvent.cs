using Stratus.Events;

namespace Stratus.Models.Gameflow
{
	/// <summary>
	/// Start a new game
	/// </summary>
	public class StartGameEvent : Event
	{
	}

	/// <summary>
	/// Continue an existing game
	/// </summary>
	public class ContinueGameEvent : Event
	{
	}

	/// <summary>
	/// End the current session
	/// </summary>
	public class EndGameEvent : Event
	{
	}
}
