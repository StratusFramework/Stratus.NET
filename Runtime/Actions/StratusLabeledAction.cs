using Stratus.IO;

using System;

namespace Stratus
{
	public class StratusLabeledAction
	{
		public string label { get; private set; }
		public Action action { get; private set; }
		/// <summary>
		/// Can be used to associate this action with a given key
		/// </summary>
		public object data { get; set; }

		public StratusLabeledAction(string label, Action action)
		{
			this.label = label;
			this.action = action;
		}

		public override string ToString()
		{
			return label;
		}

		public void Invoke() => action();
		public bool TryInvoke()
		{
			if (action != null)
			{
				action();
				return true;
			}
			return false;
		}

		public T Data<T>() => (T)data;

		public static implicit operator Action(StratusLabeledAction action) => action.action;


	}

	public struct StratusLabeledContextAction<T> where T : class
	{
		public string label;
		public Action action;
		public T context;

		public StratusLabeledContextAction(string label, Action action, T context)
		{
			this.label = label;
			this.action = action;
			this.context = context;
		}
	}
}