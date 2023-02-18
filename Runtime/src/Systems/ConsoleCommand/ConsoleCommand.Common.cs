namespace Stratus.Systems
{
	public class CommonConsoleCommands : IConsoleCommandProvider
	{
		[ConsoleCommand("log")]
		public static string Log(string message)
		{
			return message;
		}
	}
}