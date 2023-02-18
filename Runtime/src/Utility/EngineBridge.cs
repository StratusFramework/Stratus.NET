using System;

namespace Stratus.Utilities
{
	/// <summary>
	/// A class which the current game engine can connect delegates to
	/// </summary>
	public static class EngineBridge
	{
		public static Func<bool> isPlaying;
	}
}
