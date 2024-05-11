using Stratus.Events;

namespace Stratus.Inputs
{
	public record InputActionPrompt(string action, string message)
	{
	}

	public record ToggleInputPromptEvent(InputActionPrompt prompt, bool toggle) : Event
	{
	}
}
