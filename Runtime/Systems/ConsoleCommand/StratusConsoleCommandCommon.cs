namespace Stratus
{
	public class CommonConsoleCommands : IStratusConsoleCommandProvider
	{
		[StratusConsoleCommand("log")]
		public static string Log(string message)
		{
			return message;
		}	
	}
}