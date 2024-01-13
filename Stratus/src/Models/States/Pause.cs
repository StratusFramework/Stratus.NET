using Stratus.Events;

using System;

namespace Stratus.Models.States
{
	/// <summary>
	/// Pause the game
	/// </summary>
	public record PauseEvent : Event
	{
	}

	/// <summary>
	/// Resume the game
	/// </summary>
	public record ResumeEvent : Event
	{
	}

	[Serializable]
	public class PauseOptions
	{
		public bool timeScale = true;
	}
}
