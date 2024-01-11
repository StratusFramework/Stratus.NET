using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Stratus.Models.UI
{
	public class Menu : MenuItemBase, IEnumerable<IMenuEntry>
	{
		public Menu? parent { get; }

		public IReadOnlyList<IMenuEntry> items => _items;
		private List<IMenuEntry> _items = new List<IMenuEntry>();

		public override bool valid => _items.IsValid();
		public IMenuEntry? this[string name] => _items.Find(e => e.name == name);
		public IMenuEntry this[int index] => _items[index];

		/// <summary>
		/// Whether this menu can be closed through an input
		/// </summary>
		public bool exitOnCancel { get; set; } = true;

		public Menu(Enumerated name, Menu parent = null) : base(name)
		{
			this.parent = parent;
		}

		public Menu Entry(IMenuEntry item)
		{
			_items.Add(item);
			return this;
		}

		public Menu Child(Enumerated name, Action<Menu> onMenu)
		{
			var menu = new Menu(name, this);
			onMenu?.Invoke(menu);
			return Entry(menu);
		}

		public Menu Action(Enumerated name, MenuAction action)
		{
			var menu = new MenuItem(name, action);
			return Entry(menu);
		}

		public Menu Action(Enumerated name, Action action, bool close = true)
		{
			var menu = new MenuItem(name, action, close);
			return Entry(menu);
		}

		public Menu Action(LabeledAction action, bool close = true)
		{
			var menu = new MenuItem(action.label, action.action, close);
			return Entry(menu);
		}

		public Menu Actions(IEnumerable<LabeledAction> actions)
		{
			foreach(var action in actions)
			{
				Action(action);
			}
			return this;
		}

		/// <summary>
		/// An entry which will close this menu
		/// </summary>
		public Menu Close(string name = "Close")
		{
			return Action(name, null, true);
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
