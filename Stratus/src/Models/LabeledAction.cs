using Stratus.IO;

using System;

namespace Stratus.Models
{
	public class LabeledAction
	{
		public string label { get; private set; }
		public Action action { get; private set; }
		/// <summary>
		/// Can be used to associate this action with some data
		/// </summary>
		public object data { get; set; }

		public LabeledAction(string label, Action action)
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

		public static implicit operator Action(LabeledAction action) => action.action;
	}
}