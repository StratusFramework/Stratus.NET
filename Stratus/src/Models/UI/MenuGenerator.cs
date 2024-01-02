using Stratus.Logging;

using System;

namespace Stratus.Models.UI
{
	public abstract class MenuGenerator : IStratusLogger
	{
		public Menu current
		{
			get => _current;
			set
			{
				_current = value;
				onMenuChanged?.Invoke(value);
			}
		}
		private Menu _current;

		public event Action<Menu> onMenuChanged;

		public abstract void Open(Menu menu);
		public abstract void Close();
	}
}
