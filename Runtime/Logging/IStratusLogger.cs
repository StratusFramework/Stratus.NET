using Stratus.Utilities;

using System;

namespace Stratus
{
	public interface IStratusLogger
	{
	}

	public static class StratusLoggerExtensions
	{
		public static void Info(this IStratusLogger logger, string message)
		{

		}
	}

	public abstract class StratusLogger
	{
		public static Lazy<Type[]> types = new Lazy<Type[]>(() => StratusTypeUtility.SubclassesOf<StratusLogger>());

		public abstract void LogInfo(string message);
		public abstract void LogWarning(string message);
		public abstract void LogError(string message);

	}

	public static class StratusLog
	{
		private static Lazy<StratusLogger> instance
			= new Lazy<StratusLogger>(() =>
			{
				if (StratusLogger.types.Value.Length > 0)
				{

				}
				return null;
			});

		public static void Info(string message)
		{
			instance.Value?.LogInfo(message);
		}

		public static void Warning(string message)
		{
			instance.Value?.LogWarning(message);
		}

		public static void Error(string message)
		{
			instance.Value?.LogError(message);
		}
	}
}