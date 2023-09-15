using Stratus.Events;

namespace Stratus.Models.States
{
	public interface IGameState
	{
	}

	public abstract class GameState
	{
	}

	public class MainMenuEvent : Event
	{
	}

	public class NewGameEvent : Event
	{
	}

	public class ContinueGameEvent : Event { }

	public class EndGameEvent : Event
	{
	}
}
