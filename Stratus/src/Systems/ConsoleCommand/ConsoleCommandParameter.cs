using Stratus.Numerics;
using Stratus.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Stratus.Systems
{

	public abstract class ConsoleCommandParameterHandler
	{
		public abstract object Parse(string arg);
	}

	public abstract class ConsoleCommandParameterHandler<T> : ConsoleCommandParameterHandler
	{
		public readonly Type type = typeof(T);
		public bool Matches(Type type) => this.type == type;

		public override object Parse(string arg)
		{
			return OnParse(arg);
		}

		protected abstract T OnParse(string arg);
	}

	#region Default Handler Implementations 
	public class IntegerConsoleCommandParameterHandler : ConsoleCommandParameterHandler<int>
	{
		protected override int OnParse(string arg)
		{
			return int.Parse(arg);
		}
	}

	public class BooleanConsoleCommandParameterHandler : ConsoleCommandParameterHandler<bool>
	{
		public const string booleanTrueAlternative = "on";
		public const string booleanFalseAlternative = "off";

		protected override bool OnParse(string arg)
		{
			bool value = false;
			string lowercaseArg = arg.ToLower();
			if (lowercaseArg.Equals(booleanTrueAlternative))
			{
				value = true;
			}
			else if (lowercaseArg.Equals(booleanFalseAlternative))
			{
				value = false;
			}
			else
			{
				value = bool.Parse(arg);
			}
			return value;
		}
	}

	public class FloatConsoleCommandParameterHandler : ConsoleCommandParameterHandler<float>
	{
		protected override float OnParse(string arg)
		{
			return float.Parse(arg);
		}
	}

	public class Vector3ConsoleCommandParameterHandler : ConsoleCommandParameterHandler<Vector3>
	{
		protected override Vector3 OnParse(string arg)
		{
			return VectorUtility.ParseVector3(arg);
		}
	}
	#endregion

	public static class ConsoleCommandParameterExtensions
	{
		public static ImplementationTypeInstancer<ConsoleCommandParameterHandler> handlers
			= new ImplementationTypeInstancer<ConsoleCommandParameterHandler>(typeof(ConsoleCommandParameterHandler<>));


		public const char delimiter = ConsoleCommand.delimiter;


		/// <summary>
		/// Converts a given a string arg to the supported parameter type
		/// </summary>
		/// <param name="arg"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public static object Parse(string arg, StratusConsoleCommandParameterInformation info)
		{
			object value = null;
			if (info.valid)
			{
				value = info.handler.Parse(arg);
			}
			else if (info.type == typeof(string))
			{
				value = arg;
			}
			else if (info.type.IsEnum)
			{
				value = Enum.Parse(info.type, arg);
			}
			return value;
		}

		public static object[] Parse(IConsoleCommand command, string args)
		{
			return Parse(command, args.Split(ConsoleCommand.delimiter));
		}

		public static object[] Parse(IConsoleCommand command, string[] args)
		{
			if (command.parameters.Length == 0)
			{
				return null;
			}

			int parameterCount = command.parameters.Length;
			if (args.Length < parameterCount)
			{
				throw new ArgumentException("Not enough arguments passed!");
			}
			else if (args.Length > parameterCount && command.parameters.Last().type != typeof(string))
			{
				throw new ArgumentException("Too many arguments passed!");
			}

			object[] parse = new object[parameterCount];
			for (int i = 0; i < parameterCount; ++i)
			{
				string param = args[i];
				parse[i] = Parse(param, command.parameters[i]);
			}

			// If the last parameter is a string, add the rest of the args to it
			if (command.parameters.Last().type == typeof(string)
				&& args.Length != parameterCount)
			{
				int lastIndex = parameterCount - 1;
				parse[lastIndex] = string.Join(delimiter.ToString(), args.Skip(lastIndex));
			}

			return parse;
		}

		public static StratusConsoleCommandParameterInformation[] DeduceMethodParameters(MethodInfo method)
		{
			List<StratusConsoleCommandParameterInformation> parameters = new List<StratusConsoleCommandParameterInformation>();
			foreach (ParameterInfo parameter in method.GetParameters())
			{
				// = ConsoleCommandParameter.Unsupported;
				if (TryDeduceParameter(parameter.ParameterType, out ConsoleCommandParameterHandler handler))
				{
					parameters.Add(new StratusConsoleCommandParameterInformation(parameter.ParameterType, handler));
				}
				else
				{
					throw new ArgumentOutOfRangeException($"Unsupported parameter {parameter.ParameterType} in method {method.Name}");
				}
			}
			return parameters.ToArray();
		}

		public static StratusConsoleCommandParameterInformation[] DeduceParameters(FieldInfo field)
		{
			if (TryDeduceParameter(field.FieldType, out ConsoleCommandParameterHandler handler))
			{
				return new StratusConsoleCommandParameterInformation[] { new StratusConsoleCommandParameterInformation(field.FieldType, handler) };
			}
			throw new ArgumentOutOfRangeException($"Unsupported parameter type for field {field.FieldType}");
		}

		public static StratusConsoleCommandParameterInformation[] DeduceParameters(PropertyInfo property)
		{
			if (TryDeduceParameter(property.PropertyType, out ConsoleCommandParameterHandler handler))
			{
				return new StratusConsoleCommandParameterInformation[] { new StratusConsoleCommandParameterInformation(property.PropertyType, handler) };
			}
			throw new ArgumentOutOfRangeException($"Unsupported parameter type for field {property.PropertyType}");
		}

		private static bool TryDeduceParameter(Type type, out ConsoleCommandParameterHandler handler)
		{
			return handlers.TryResolve(type, out handler);
		}
	}

	public class StratusConsoleCommandParameterInformation
	{
		/// <summary>
		/// The qualified type of the parameter
		/// </summary>
		public Type type;
		public ConsoleCommandParameterHandler handler;
		public bool valid => handler != null;

		public StratusConsoleCommandParameterInformation(Type type, ConsoleCommandParameterHandler handler)
		{
			this.type = type;
			this.handler = handler;
		}
	}
}