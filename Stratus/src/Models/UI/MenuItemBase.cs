using Stratus.Extensions;

using System;
using System.Collections.Generic;

namespace Stratus.Models.UI
{
	public abstract class MenuItemBase : IMenuEntry
	{
		private Func<MenuVisibility> _visibility;
		public string name { get; }
		public MenuVisibility visibility => _visibility.GetValueOrDefault(MenuVisibility.Visible);

		public abstract bool valid { get; }

		protected MenuItemBase(string name)
		{
			this.name = name;
		}

		public MenuItemBase When(Func<MenuVisibility> predicate)
		{
			_visibility = predicate;
			return this;
		}

		public override string ToString()
		{
			return name;
		}
	}

	public class MenuItem : MenuItemBase
	{
		/// <summary>
		/// An action that returns true if the menu should be closed
		/// </summary>
		public MenuAction action { get; private set; }
		public override bool valid => action != null;

		public MenuItem(string name, MenuAction action) : base(name)
		{
			this.action = action;
		}

		public MenuItem(string name, Action action, bool close) : base(name)
		{
			this.action = () =>
			{
				action?.Invoke();
				return close;
			};
		}
	}
}
