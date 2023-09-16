using Stratus.Events;

namespace Stratus.Models.States
{
	public interface IGameState
	{
	}

	/// <summary>
	/// Manages a game
	/// </summary>
	public abstract class GameState
	{
	}

	/// <summary>
	/// Go back to the main menu
	/// </summary>
	public class MainMenuEvent : Event
	{
	}

	/// <summary>
	/// Start a new game
	/// </summary>
	public class NewGameEvent : Event
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
