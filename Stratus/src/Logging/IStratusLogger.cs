using Stratus.Logging;
using Stratus.Types;

using System;
using System.Linq;
using System.Net;

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
		public static void LogException(this IStratusLogger logger, Exception e) => StratusLog.Exception(e);

		/// <summary>
		/// Prints the given operation result to the console
		/// </summary>
		/// <param name="value"></param>
		public static void Log(this IStratusLogger logger, Result result) => StratusLog.Result(result);
	}

	/// <summary>
	/// A common logger
	/// </summary>
	/// <remarks>Implement each application</remarks>
	public abstract class StratusLogger
	{
		#region Static Properties
		private static Lazy<StratusLogger> _instance = new Lazy<StratusLogger>(() =>
		{
			if (types.Value.Length > 0)
			{
				var implementations = types.Value;
				return (StratusLogger)ObjectUtility.Instantiate(implementations.First());
			}
			return null;
		});

		public static StratusLogger instance => _instance.Value;
		public static Lazy<Type[]> types = new Lazy<Type[]>(() => TypeUtility.SubclassesOf<StratusLogger>()); 
		#endregion

		#region Interface
		public abstract void LogInfo(string message);
		public abstract void LogWarning(string message);
		public abstract void LogError(string message);
		public abstract void LogException(Exception ex);

		public virtual string Format(string message, object context)
		{
			return context == null ? message : $"[{context}] {message}";
		} 
		#endregion
	}
}

namespace Stratus
{
	public static class StratusLog
	{
		public static void Log(LogType type, string message, object context)
		{
			switch (type)
			{
				case LogType.Info:
					Info(message, context);
					break;
				case LogType.Warning:
					Warning(message, context);
					break;
				case LogType.Error:
					Error(message, context);
					break;
				default:
					break;
			}
		}

		public static void Log(LogType type, string message) => Log(type, message);

		private static string Format(string message, object? context) => StratusLogger.instance.Format(message, context);

		public static void Info(string message, object? context = null)
		{
			StratusLogger.instance.LogInfo(Format(message, context));
		}

		public static void Info(object value, object? context = null) => Info(value.ToString(), context);

		public static void Warning(string message, object? context = null)
		{
			StratusLogger.instance.LogWarning(Format(message, context));
		}
		public static void Warning(object value, object? context = null) => Warning(value.ToString(), context);

		public static void Error(string message, object? context = null)
		{
			StratusLogger.instance.LogError(Format(message, context));
		}
		public static void Error(object value, object? context = null) => Error(value.ToString(), context);

		public static void Exception(Exception ex)
		{
			StratusLogger.instance.LogException(ex);
		}

		public static void Result(Result result)
		{
			if (result)
			{
				Info(result.message);
			}
			else
			{
				Error(result.message);
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