using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Stratus.Models.UI
{
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

		public Menu Entry(IMenuEntry item)
		{
			_items.Add(item);
			return this;
		}

		public Menu SubMenu(string name, Action<Menu> onMenu)
		{
			var menu = new Menu(name, this);
			onMenu?.Invoke(menu);
			return Entry(menu);
		}

		public Menu Item(string name, MenuAction action)
		{
			var menu = new MenuItem(name, action);
			return Entry(menu);
		}

		public Menu Item(string name, Action action, bool close = true)
		{
			var menu = new MenuItem(name, action, close);
			return Entry(menu);
		}

		public Menu Item(LabeledAction action, bool close = true)
		{
			var menu = new MenuItem(action.label, action.action, close);
			return Entry(menu);
		}

		public Menu Items(IEnumerable<LabeledAction> actions)
		{
			foreach(var action in actions)
			{
				Item(action);
			}
			return this;
		}

		/// <summary>
		/// An entry which will close this menu
		/// </summary>
		public Menu Close(string name = "Close")
		{
			return Item(name, null, true);
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
}
