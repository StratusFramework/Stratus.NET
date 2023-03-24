using System;

namespace Stratus
{
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