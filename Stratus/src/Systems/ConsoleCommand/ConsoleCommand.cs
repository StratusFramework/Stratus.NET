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
		Action<string> invocation { get; set; }
	}

	public abstract class ConsoleCommand : IConsoleCommand
	{
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

		public enum EntryType
		{
			Submit,
			Result,
			Warning,
			Error
		}

		#region Declarations
		[Serializable]
		public class History
		{
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

		private class Instance
		{
			public IConsoleCommand commamd { get; }
			public StratusConsoleCommandParameterInformation[] parameters { get; internal set; }
			public Action<string> invocation { get; internal set; }

			public Instance(IConsoleCommand commamd, StratusConsoleCommandParameterInformation[] parameters, Action<string> invocation)
			{
				this.commamd = commamd;
				this.parameters = parameters;
				this.invocation = invocation;
			}
		}

		public delegate void EntryEvent(Entry e);
		public delegate void SubmitEvent(string command);
		#endregion

		#region Constants
		public const char delimiter = ' ';
		public const string delimiterStr = " ";
		private static readonly BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		#endregion

		#region Properties
		string IConsoleCommand.name { get; set; }
		string IConsoleCommand.description { get; set; }
		string IConsoleCommand.usage { get; set; }
		bool IConsoleCommand.hidden { get; }
		StratusConsoleCommandParameterInformation[] IConsoleCommand.parameters { get; set; }
		public Action<string> invocation { get; set; }
		#endregion

		#region Static Properties
		public static IReadOnlyList<Entry> entries => history.entries;
		public static Lazy<Type[]> handlerTypes = new Lazy<Type[]>(() => TypeUtility.GetInterfaces(typeof(IConsoleCommandProvider)));
		public static Lazy<Dictionary<string, Type>> handlerTypesByName = new Lazy<Dictionary<string, Type>>(() => handlerTypes.Value.ToDictionary(t => t.Name));
		public static Lazy<IEnumerable<IConsoleCommand>> commands = new Lazy<IEnumerable<IConsoleCommand>>(Generate);
		public static Lazy<Dictionary<string, IConsoleCommand>> commandsByName
			= new Lazy<Dictionary<string, IConsoleCommand>>(() => commands.Value.ToDictionary(c => c.name));
		private static Lazy<Dictionary<string, Action<string>>> commandActions =
			new Lazy<Dictionary<string, Action<string>>>(() => commands.Value.ToDictionary(c => (c.name, c.invocation)));

		public static History history { get; private set; } = new History();
		public static string lastCommand => history.commands.LastOrDefault();
		public static string latestResult => history.results.LastOrDefault();

		#endregion

		#region Events
		public static event EntryEvent onEntry;
		public static event SubmitEvent onSubmit;
		#endregion

		#region Interface
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
				commandAction = commandActions.Value.GetValueOrDefault(commandName);
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
					RecordEntry(new Entry(msg, EntryType.Error));
				}
				return true;
			}
			else
			{
				RecordEntry(new Entry($"No command matching '{command}' could be found!", EntryType.Warning));
			}

			return false;
		}

		[ConsoleCommand("clear")]
		public static void ClearHistory()
		{
			history.Clear();
		}
		#endregion

		#region History
		private static void RecordCommand(string command)
		{
			history.commands.Add(command);
			onSubmit?.Invoke(command);
			RecordEntry(new Entry(command, EntryType.Submit));
		}

		private static void RecordResult(string text, object result)
		{
			//RecordCommand(text);
			history.results.Add(result.ToString());
			RecordEntry(new Entry(result.ToString(), EntryType.Result));
		}

		private static void RecordEntry(Entry e)
		{
			history.entries.Add(e);
			switch (e.type)
			{
				case EntryType.Submit:
					StratusLog.Info(e.text);
					break;
				case EntryType.Result:
					StratusLog.Info(e.text);
					break;
				case EntryType.Warning:
					StratusLog.Warning(e.text);
					break;
				case EntryType.Error:
					StratusLog.Error(e.text);
					break;
			}
			onEntry?.Invoke(e);
		}
		#endregion

		#region Generation
		private static IEnumerable<IConsoleCommand> Generate()
		{
			foreach (Type handler in handlerTypes.Value)
			{
				// Methods
				foreach (MethodInfo method in handler.GetMethods(flags))
				{
					var command = GetCommand(method);
					if (command != null)
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceMethodParameters(method);
						if (command.usage.IsNullOrEmpty())
						{
							command.usage = $"({method.GetParameterNames()})";
						}

						command.invocation = (args) =>
						{
							object[] parsedArgs = Parse(command, args);
							object returnValue = method.Invoke(null, parsedArgs);
							if (returnValue != null)
							{
								RecordResult($"{command.name}({args}) = {returnValue}", $"{returnValue}");
							}
						};

						yield return command;
					}
				}

				// Fields
				foreach (FieldInfo field in handler.GetFields(flags))
				{
					var command = GetCommand(field);
					if (command != null)
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceParameters(field);
						StratusConsoleCommandParameterInformation parameter = command.parameters[0];
						if (command.usage.IsNullOrEmpty())
						{
							command.usage = $"{parameter.type}";
						}

						command.invocation = (args) =>
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
						};

						yield return command;
					}
				}

				// Properties
				foreach (PropertyInfo property in handler.GetProperties(flags))
				{
					var command = GetCommand(property);
					if (command != null)
					{
						command.parameters = ConsoleCommandParameterExtensions.DeduceParameters(property);
						StratusConsoleCommandParameterInformation parameter = command.parameters[0];

						if (command.usage.IsNullOrEmpty())
						{
							command.usage = $"{parameter.type}";
						}

						bool hasSetter = property.GetSetMethod(true) != null;
						if (hasSetter)
						{
							command.invocation = (args) =>
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
							};
						}
						else
						{
							command.invocation = (args) =>
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
							};
						}

						yield return command;
					}
				}
			}
		}

		private static IConsoleCommand GetCommand(MemberInfo member)
		{
			IConsoleCommand command = member.GetAttribute<ConsoleCommandAttribute>();
			if (command != null)
			{
				if (command.name.IsNullOrEmpty())
				{
					command.name = member.Name;
				}
			}
			return command;
		} 
		#endregion

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