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

	/// <summary>
	/// An action from an item in the menu
	/// </summary>
	/// <returns>True if the menu can be closed</returns>
	public delegate bool MenuAction();
}
