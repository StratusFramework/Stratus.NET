using System;

namespace Stratus.Utilities
{
	/// <summary>
	/// A class which the current game engine can connect delegates to
	/// </summary>
	public static class EngineBridge
	{
		public static bool isPlaying => _isPlaying != null ? _isPlaying() : false;
		private static Func<bool> _isPlaying;

		public static void SetPlayingCallback(Func<bool> callback)
		{
			_isPlaying = callback;
		}
	}
}
