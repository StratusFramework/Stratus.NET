using Stratus.Numerics;
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

		public FloatRange? numericRange { get; set; }

		public MenuOption(string name, ObjectReference reference)
			: base(name)
		{
			this.reference = reference;
		}

		public MenuOption(string name, Func<float> get, Action<float> set, float minimum, float maximum)
			: base(name)
		{
			this.reference = ObjectReference.Float(get, set);
			this.numericRange = new FloatRange(minimum, maximum);
		}
	}

	public static class MenuOptionExtensions
	{
		public static Menu Option(this Menu menu, string name, ObjectReference reference)
		{
			return menu.Entry(new MenuOption(name, reference));
		}

		public static Menu Option(this Menu menu, MenuOption option)
		{
			return menu.Entry(option);
		}
	}
}
