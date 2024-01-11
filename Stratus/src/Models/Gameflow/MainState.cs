using Stratus.Models.UI;

namespace Stratus.Models.Gameflow 
{
	/// <summary>
	/// The state of the game after it has been started from the <see cref="MainMenuState"/>
	/// </summary>
	public class MainState : State
	{
	}

	/// <summary>
	/// Usually the default state of the game (after a splash screen)
	/// </summary>
	public class MainMenuState : State
	{
	}

	/// <summary>
	/// The main menu for managing the game's options
	/// </summary>
	/// <remarks>This can be configured at runtime before it is entered, in order to add new menus and items</remarks>
	public class OptionsMenuState : State
	{
		/// <summary>
		/// The menu to be generated when this state is active
		/// </summary>
		/// <remarks>A simulation can add entries onto the menu during the start</remarks>
		public Menu menu { get; } = new Menu("Options");
	}

	/// <summary>
	/// The default pause state of the simulation
	/// </summary>
	public class PauseState : State
	{
		/// <summary>
		/// The menu to be generated when this state is active
		/// </summary>
		/// <remarks>A simulation can add entries onto the menu during the start</remarks>
		public Menu menu { get; } = new Menu("Pause");
	}
}
