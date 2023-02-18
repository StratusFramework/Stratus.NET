using Stratus.Extensions;
using Stratus.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stratus.Systems
{
	public interface IConsoleCommand
	{
		string name { get; set; }
		string description { get; set; }
		string usage { get; set; }
		bool hidden { get; }
		StratusConsoleCommandParameterInformation[] parameters { get; set; }
	}

	public abstract class ConsoleCommand : IConsoleCommand
	{
		//------------------------------------------------------------------------/
		// Declarations
		//------------------------------------------------------------------------/
		/// <summary>
		/// The current history of commands
		/// </summary>
		[Serializable]
		public class History
		{
			/// <summary>
			/// What type of console command entry was recorded
			/// </summary>
			public enum EntryType
			{
				Submit,
				Result,
				Warning,
				Error
			}

			/// <summary>
			/// What kind of entry was recorded
			/// </summary>
			public struct Entry
			{
				public string text;
				public EntryType type;
				public string timestamp;

				public Entry(string text, EntryType type)
				{
					this.text = text;
					this.type = type;
					this.timestamp = DateTime.Now.ToShortTimeString();
				}
			}

			public List<string> commands = new List<string>();
			public List<string> results = new List<string>();
			public List<Entry> entries = new List<Entry>();

			/// <summary>
			/// Clears the history
			/// </summary>
			public void Clear()
			{
				commands.Clear();
				results.Clear();
				entries.Clear();
			}
		}

		public delegate void EntryEvent(History.Entry e);
		public delegate void SubmitEvent(string command);

		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		public const char delimiter = ' ';
		public const string delimiterStr = " ";
		private static readonly BindingFlags flags =
			BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		private static Dictionary<string, Action<string>> commandActions;

		//------------------------------------------------------------------------/
		// Properties: Interface
		//------------------------------------------------------------------------/
		string IConsoleCommand.name { get; set; }
		string IConsoleCommand.description { get; set; }
		string IConsoleCommand.usage { get; set; }
		bool IConsoleCommand.hidden { get; }
		StratusConsoleCommandParameterInformation[] IConsoleCommand.parameters { get; set; }

		//------------------------------------------------------------------------/
		// Properties: Static
		//------------------------------------------------------------------------/
		public static Type[] handlerTypes { get; private set; }
		public static Dictionary<string, Type> handlerTypesByName { get; private set; }
		public static IConsoleCommand[] commands { get; private set; }
		public static Dictionary<string, IConsoleCommand> commandsByName { get; private set; }
		public static History history { get; private set; }
		public static string[] commandLabels { get; private set; }
		public static string[] variableNames { get; private set; }
		public static string lastCommand => history.commands.LastOrDefault();
		public static string latestResult => history.results.LastOrDefault();

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		public static event EntryEvent onEntry;
		public static event SubmitEvent onSubmit;

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		static ConsoleCommand()
		{
			RegisterCommands();
			history = new History();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Submits a command to be executed
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static bool Submit(string command)
		{
			RecordCommand(command);

			string[] commandSplit = command.Split(delimiter);
			int length = command.Length;

			if (length < 1)
			{
				return false;
			}

			string commandName = null;
			Action<string> commandAction = null;
			string args = null;

			// Compose the command by working backwards
			for (int i = length; i >= 0; i--)
			{
				commandName = commandSplit.Take(i).Join(delimiterStr);
				commandAction = commandActions.GetValueOrDefault(commandName);
				if (commandAction != null)
				{
					if (i > 0)
					{
						args = commandSplit.Skip(i).Join(delimiterStr);
					}
					break;
				}
			}

			if (commandAction != null)
			{
				try
				{
					commandAction.Invoke(args);
				}
				catch (Exception e)
				{
					string msg = $"Error executing the command '{commandName}':\n{e}";
					RecordEntry(new History.Entry(msg, History.EntryType.Error));
				}
				return true;
			}
			else
			{
				RecordEntry(new History.Entry($"No command matching '{command}' could be found!", History.EntryType.Warning));
			}

			return false;
		}

		[ConsoleCommand("clear")]
		public static void ClearHistory()
		{
			history.Clear();
		}

		//------------------------------------------------------------------------/
		// Recording
		//------------------------------------------------------------------------/
		private static void RecordCommand(string command)
		{
			history.commands.Add(command);
			onSubmit?.Invoke(command);
			RecordEntry(new History.Entry(command, History.EntryType.Submit));
		}

		private static void RecordResult(string text, object result)
		{
			RecordCommand(text);
			history.results.Add(result.ToString());
			RecordEntry(new History.Entry(result.ToString(), History.EntryType.Result));
		}

		private static void RecordEntry(History.Entry e)
		{
			history.entries.Add(e);
			switch (e.type)
			{
				case History.EntryType.Submit:
					StratusLog.Info(e.text);
					break;
				case History.EntryType.Result:
					StratusLog.Info(e.text);
					break;
				case History.EntryType.Warning:
					StratusLog.Warning(e.text);
					break;
				case History.EntryType.Error:
					StratusLog.Error(e.text);
					break;
			}
			onEntry?.Invoke(e);
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private static void RegisterCommands()
		{
			commandsByName = new Dictionary<string, IConsoleCommand>();
			commandActions = new Dictionary<string, Action<string>>();

			List<IConsoleCommand> commands = new List<IConsoleCommand>();
			List<string> variableNames = new List<string>();

			handlerTypes = TypeUtility.GetInterfaces(typeof(IConsoleCommandProvider));
			handlerTypesByName = new Dictionary<string, Type>();
			handlerTypesByName.AddRange(x => x.Name, handlerTypes);

			foreach (Type handler in handlerTypes)
			{
				// Methods
				foreach (MethodInfo method in handler.GetMethods(flags))
				{
					TryAddCommand(method, (command) =>
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceMethodParameters(method);
						if (command.usage.IsNullOrEmpty())
							command.usage = $"({method.GetParameterNames()})";

						commandActions.Add(command.name, (args) =>
						{
							object[] parsedArgs = Parse(command, args);
							object returnValue = method.Invoke(null, parsedArgs);
							if (returnValue != null)
							{
								RecordResult($"{command.name}({args}) = {returnValue}", $"{returnValue}");
							}
						});
					});
				}

				// Fields
				foreach (FieldInfo field in handler.GetFields(flags))
				{
					TryAddCommand(field, (command) =>
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceParameters(field);
						StratusConsoleCommandParameterInformation parameter = command.parameters[0];
						if (command.usage.IsNullOrEmpty())
							command.usage = $"{parameter.deducedType}";

						commandActions.Add(command.name, (args) =>
						{
							bool hasValue = args.IsValid();
							if (hasValue)
							{
								field.SetValue(null, Parse(parameter, args));
							}
							else
							{
								object value = field.GetValue(null);
								RecordResult($"{command.name} = {value}", value);
							}
						});

					});
				}

				// Properties
				foreach (PropertyInfo property in handler.GetProperties(flags))
				{
					TryAddCommand(property, (command) =>
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceParameters(property);
						StratusConsoleCommandParameterInformation parameter = command.parameters[0];

						if (command.usage.IsNullOrEmpty())
							command.usage = $"{parameter.deducedType}";

						bool hasSetter = property.GetSetMethod(true) != null;
						if (hasSetter)
						{
							commandActions.Add(command.name, (args) =>
							{
								bool hasValue = args.IsValid();
								if (hasValue)
								{
									property.SetValue(null, Parse(parameter, args));
								}
								else
								{
									object value = property.GetValue(null);
									RecordResult($"{command.name} = {value}", value);
								}
							});
						}
						else
						{
							commandActions.Add(command.name, (args) =>
							{
								bool hasValue = args.IsValid();
								if (hasValue)
								{
									RecordCommand($"{command.name} has no setters!");
								}
								else
								{
									object value = property.GetValue(null);
									RecordResult($"{command.name} = {value}", value);
								}
							});
						}

					});
				}
			}

			IConsoleCommand TryAddCommand(MemberInfo member, Action<IConsoleCommand> onCommandAdded)
			{
				IConsoleCommand command = member.GetAttribute<ConsoleCommandAttribute>();
				if (command != null)
				{
					if (command.name.IsNullOrEmpty())
						command.name = member.Name;
					onCommandAdded(command);
					commandsByName.Add(command.name, command);
					commands.Add(command);
				}
				return command;
			}

			ConsoleCommand.variableNames = variableNames.ToArray();
			ConsoleCommand.commands = commands.ToArray();
		}

		public static object Parse(StratusConsoleCommandParameterInformation parameter, string arg)
		{
			return ConsoleCommandParameterExtensions.Parse(arg, parameter);
		}

		public static object[] Parse(IConsoleCommand command, string args)
		{
			return ConsoleCommandParameterExtensions.Parse(command, args);
		}

		public static object[] Parse(IConsoleCommand command, string[] args)
		{
			return ConsoleCommandParameterExtensions.Parse(command, args);
		}
	}
}