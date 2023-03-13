using Stratus.Extensions;
using Stratus.Reflection;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Inputs
{
	public interface IActionMapHandler
	{
		/// <summary>
		/// The name of the action map these inputs are for
		/// </summary>
		string name { get; }
		bool HandleInput(object input);
	}

	public interface ICustomActionMapHandler : IActionMapHandler
	{
		bool lowercase { get; }
		bool TryBind(string name, object deleg);
		void Bind(string action, Action onAction);
	}

	public abstract class ActionMapHandler : IActionMapHandler
	{
		public abstract string name { get; }
		public abstract bool HandleInput(object input);
	}

	public abstract class ActionMapHandlerBase<TInput> : ActionMapHandler
	{
		public abstract bool HandleInput(TInput input);
		public override bool HandleInput(object input)
		{
			return HandleInput((TInput)input);
		}
	}

	public abstract class ActionMapHandler<TInput> : ActionMapHandlerBase<TInput>, ICustomActionMapHandler
	{
		#region Fields
		protected Dictionary<string, Action<TInput>> _actions = new Dictionary<string, Action<TInput>>(StringComparer.InvariantCultureIgnoreCase);
		#endregion

		#region Properties
		public bool initialized { get; private set; }
		public IReadOnlyDictionary<string, Action<TInput>> actions
		{
			get
			{
				if (!initialized)
				{
					OnInitialize();
					initialized = true;
				}

				return _actions;
			}
		}
		public int count => actions.Count;
		#endregion

		#region Virtual
		public bool lowercase { get; protected set; }
		public abstract bool TryBind(string name, object deleg);
		protected virtual void OnInitialize()
		{
		}
		#endregion

		#region Interface
		public void Bind(string action, Action<TInput> onAction)
		{
			_actions.AddOrUpdate(lowercase ? action.ToLowerInvariant() : action, onAction);
		}

		public void Bind(string action, Action onAction)
		{
			Bind(action, a => onAction());
		}

		public bool Contains(string name) => actions.ContainsKey(name);

		public void Bind<TAction>(TAction action, Action onAction) where TAction : Enum
		{
			Bind(action.ToString(), onAction);
		}

		public Result TryBindAll<TAction>() where TAction : Enum
		{
			var enumeratedValuesByName = EnumUtility.Values<TAction>().ToDictionary(v => v.ToString(), 
				StringComparer.InvariantCultureIgnoreCase);

			var members = this.GetAllFieldsOrProperties()
				.Where(m => typeof(Delegate).IsAssignableFrom(m.type)).ToArray();

			if (members.IsNullOrEmpty())
			{
				return new Result(false, $"Found no action members in the map class {GetType().Name}");
			}

			int count = 0;
			foreach (var member in members)
			{
				var name = member.name.ToLowerInvariant();
				if (!enumeratedValuesByName.ContainsKey(name))
				{
					continue;
				}

				TAction action = enumeratedValuesByName[name];
				if (TryBind(action.ToString(), member.value))
				{
					count++;
				}
			}

			if (count == 0)
			{
				return new Result(false, $"Found no actions to bind to in {GetType().Name}");
			}

			return true;
		}
		#endregion
	}
}
