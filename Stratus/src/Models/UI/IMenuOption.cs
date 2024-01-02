using Stratus.Data;
using Stratus.Reflection;

using System;
using System.Runtime.CompilerServices;

namespace Stratus.Models.UI
{
	/// <summary>
	/// A menu item that modifies a value
	/// </summary>
	public interface IMenuOption : IMenuEntry
	{
		ObjectReference reference { get; }
	}

	public class MenuOption : MenuItemBase, IMenuOption
	{
		public ObjectReference reference { get; }
		public override bool valid => reference != null;

		public MenuOption(string name, ObjectReference reference)
			: base(name)
		{
			this.reference = reference;
		}
	}

	public static class MenuOptionExtensions
	{
		public static Menu Option(this Menu menu, string name, ObjectReference reference)
		{
			return menu.Entry(new MenuOption(name, reference));
		}
	}
}
