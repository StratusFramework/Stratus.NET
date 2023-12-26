using Stratus.Data;
using Stratus.Events;
using Stratus.Extensions;
using Stratus.Logging;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Stratus.Models.UI
{
	public interface IMenuEntry
	{
		string name { get; }
		MenuVisibility visibility { get; }
		bool valid { get; }
	}

	public enum MenuVisibility
	{
		Visible,
		Disabled,
		Hidden,
	}

	public class MenuStateEvent<T>
	{
		public class OpenEvent : Event
		{
		}

		public class CloseEvent : Event 
		{
		}
	}

	/// <summary>
	/// An action from an item in the menu
	/// </summary>
	/// <returns>True if the menu can be closed</returns>
	public delegate bool MenuAction();

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
				action();
				return close;
			};
		}
	}

	public class Menu : MenuItemBase, IEnumerable<IMenuEntry>
	{
		public Menu? parent { get; }
		private List<IMenuEntry> _items = new List<IMenuEntry>();
		public override bool valid => _items.IsValid();
		public IReadOnlyList<IMenuEntry> items => _items;

		/// <summary>
		/// Whether this menu can be closed through an input
		/// </summary>
		public bool exitOnCancel { get; set; } = true;

		public Menu(string name, Menu parent = null) : base(name)
		{
			this.parent = parent;
		}

		public Menu Add(IMenuEntry item)
		{
			_items.Add(item);
			return this;
		}

		public Menu SubMenu(string name, Action<Menu> onMenu)
		{
			var menu = new Menu(name, this);
			onMenu?.Invoke(menu);
			return Add(menu);
		}

		public Menu Item(string name, MenuAction action)
		{
			var menu = new MenuItem(name, action);
			return Add(menu);
		}

		public Menu Item(string name, Action action, bool close = true)
		{
			var menu = new MenuItem(name, action, close);
			return Add(menu);
		}

		public Menu Item(LabeledAction action, bool close = true)
		{
			var menu = new MenuItem(action.label, action.action, close);
			return Add(menu);
		}

		public Menu Items(IEnumerable<LabeledAction> actions)
		{
			foreach(var action in actions)
			{
				Item(action);
			}
			return this;
		}

		public Menu NotClosable()
		{
			exitOnCancel = false;
			return this;
		}

		public IEnumerator<IMenuEntry> GetEnumerator()
		{
			return ((IEnumerable<IMenuEntry>)this._items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this._items).GetEnumerator();
		}
	}

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
