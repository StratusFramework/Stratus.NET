﻿using Stratus.Logging;
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
}

namespace Stratus.Logging
{
	public static class IStratusLoggerExtensions
	{
		/// <summary>
		/// Prints the given message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void Log(this IStratusLogger logger, object value) => StratusLog.Info(value);

		/// <summary>
		/// Prints the given warning message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void LogWarning(this IStratusLogger logger, object value) => StratusLog.Warning(value);

		/// <summary>
		/// Prints the given error message to the console
		/// </summary>
		/// <param name="value"></param>
		public static void LogError(this IStratusLogger logger, object value) => StratusLog.Error(value);

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
}

namespace Stratus.Logging
{
	public abstract class StratusLogger
	{
		public static Lazy<Type[]> types = new Lazy<Type[]>(() => TypeUtility.SubclassesOf<StratusLogger>());

		public abstract void LogInfo(string message);
		public abstract void LogWarning(string message);
		public abstract void LogError(string message);
		public abstract void LogException(Exception ex);

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

		public static void Info(string message)
		{
			instance.Value?.LogInfo(message);
		}

		public static void Info(object value) => Info(value.ToString());

		public static void Warning(string message)
		{
			instance.Value?.LogWarning(message);
		}

		public static void Warning(object value) => Warning(value.ToString());

		public static void Error(string message)
		{
			instance.Value?.LogError(message);
		}

		public static void Error(object value) => Error(value.ToString());

		public static void Exception(Exception ex)
		{
			instance.Value.LogException(ex);
		}

		public static void AssertNotNull(object value, string message)
		{
			if (value == null)
			{
				throw new NullReferenceException(message);
			}
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
	}
}