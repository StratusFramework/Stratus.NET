using Stratus.Logging;
using Stratus.Types;

using System;
using System.Linq;

namespace Stratus.Logging
{
	public interface IStratusLogger
	{
	}

	public enum LogType
	{
		Info,
		Warning,
		Error
	}

	public static class IStratusLoggerExtensions
	{
		/// <summary>
		/// Prints the given message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void Log(this IStratusLogger logger, object value) => StratusLog.Info(value, logger);

		/// <summary>
		/// Prints the given warning message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void LogWarning(this IStratusLogger logger, object value) => StratusLog.Warning(value, logger);

		/// <summary>
		/// Prints the given error message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void LogError(this IStratusLogger logger, object value) => StratusLog.Error(value, logger);

		/// <summary>
		/// Prints the given exception to the console
		/// </summary>
		/// <param name="value"></param>
		public static void LogException(this IStratusLogger logger, Exception e) => StratusLog.Exception(e, logger);

		/// <summary>
		/// Prints the given operation result to the console
		/// </summary>
		/// <param name="value"></param>
		public static void Log(this IStratusLogger logger, Result result) => StratusLog.Result(result, logger);
	}

	public abstract class StratusLogger
	{
		public static Lazy<Type[]> types = new Lazy<Type[]>(() => TypeUtility.SubclassesOf<StratusLogger>());

		public abstract void LogInfo(string message, IStratusLogger logger = null);
		public abstract void LogWarning(string message, IStratusLogger logger = null);
		public abstract void LogError(string message, IStratusLogger logger = null);
		public abstract void LogException(Exception ex, IStratusLogger logger = null);
	}
}

namespace Stratus
{
	public static class StratusLog
	{
		private static Lazy<StratusLogger> instance
			= new Lazy<StratusLogger>(() =>
			{
				if (StratusLogger.types.Value.Length > 0)
				{
					var implementations = StratusLogger.types.Value;
					return (StratusLogger)ObjectUtility.Instantiate(implementations.First());
				}
				return null;
			});

		public static void Log(LogType type, string message, IStratusLogger logger = null)
		{
			switch (type)
			{
				case LogType.Info:
					Info(message, logger);
					break;
				case LogType.Warning:
					Warning(message, logger);
					break;
				case LogType.Error:
					Error(message, logger);
					break;
				default:
					break;
			}
		}

		public static void Info(string message, IStratusLogger logger = null)
		{
			instance.Value?.LogInfo(message, logger);
		}

		public static void Info(object value, IStratusLogger logger = null)
			=> Info(value.ToString(), logger);

		public static void Warning(string message, IStratusLogger logger = null)
		{
			instance.Value?.LogWarning(message, logger);
		}

		public static void Warning(object value, IStratusLogger logger = null)
			=> Warning(value.ToString(), logger);

		public static void Error(string message, IStratusLogger logger = null)
		{
			instance.Value?.LogError(message, logger);
		}

		public static void Error(object value, IStratusLogger logger = null)
			=> Error(value.ToString(), logger);

		public static void Exception(Exception ex, IStratusLogger logger = null)
		{
			instance.Value.LogException(ex, logger);
		}

		public static void Result(Result result, IStratusLogger logger = null)
		{
			if (result)
			{
				Info(result.message, logger);
			}
			else
			{
				Error(result.message, logger);
			}
		}

		public static void AssertNotNull(object value, string message)
		{
			if (value == null)
			{
				throw new NullReferenceException(message);
			}
		}
	}
}