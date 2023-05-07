using Stratus.Events;

using System;

namespace Stratus.Models.States
{
	public static class Pause
	{
	}

	public class PauseEvent : Event
	{
	}

	public class ResumeEvent : Event
	{
	}

	[Serializable]
	public class PauseOptions
	{
		public bool timeScale = true;
	}
}
