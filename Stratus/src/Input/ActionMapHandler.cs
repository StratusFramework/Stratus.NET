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
		bool valid { get; }
	}

	public enum InputActionType
	{
		None,
		Button,
		Composite,
	}

	public interface ICustomActionMapHandler : IActionMapHandler
	{
		bool lowercase { get; }
		bool TryBind(string name, object deleg);
		void Bind(string action, Action onAction);
	}

	public abstract class ActionMapHandler : IActionMapHandler
	{
		public abstract bool valid { get; }
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
		#region Declarations
		public class Binding
		{
			public Binding(string name, InputActionType type, Action<TInput> action)
			{
				this.name = name;
				this.type = type;
				this.action = action;
			}

			public Action<TInput> action { get; }
			public string name { get; }
			public InputActionType type { get; }
		}
		#endregion

		#region Fields
		protected Dictionary<string, Binding> _actionsByName = new Dictionary<string, Binding>(StringComparer.InvariantCultureIgnoreCase);
		#endregion

		#region Properties
		public bool initialized { get; private set; }
		public override bool valid => actions.Count > 0;
		public IReadOnlyDictionary<string, Binding> actions
		{
			get
			{
				if (!initialized)
				{
					OnInitialize();
					initialized = true;
				}

				return _actionsByName;
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
		public virtual void Bind(string name, InputActionType type, Action<TInput> action)
		{
			name = lowercase ? name.ToLowerInvariant() : name;
			var binding = new Binding(name, type, action);
			_actionsByName.AddOrUpdate(name, binding);
		}

		public void Bind(string action, Action onAction)
		{
			Bind(action, InputActionType.Button, a => onAction());
		}

		public bool Contains(string name) => actions.ContainsKey(name);

		public void Bind<TAction>(TAction action, Action onAction) where TAction : Enum
		{
			Bind(action.ToString(), onAction);
		}

		public Result TryBindAll<TAction>(object target) where TAction : Enum
		{
			var enumeratedValuesByName = EnumUtility.Values<TAction>().ToDictionary(v => v.ToString(),
				StringComparer.InvariantCultureIgnoreCase);

			var members = target.GetAllFieldsOrProperties()
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
