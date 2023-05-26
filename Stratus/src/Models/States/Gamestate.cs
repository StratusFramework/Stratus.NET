using Stratus.Events;

namespace Stratus.Models.States
{
	public abstract class Gamestate
	{

	}

	public class MainMenuEvent : Event
	{
	}

	public class NewGameEvent : Event
	{
	}

	public class ContinueGameEvent : Event { }
}
