using Stratus.Events;
using Stratus.Extensions;
using Stratus.Inputs;
using Stratus.Models.UI;
using Stratus.Utilities;

using System;
using System.Collections.Generic;

namespace Stratus.Models.States
{
	/// <summary>
	/// The game states usually dictate the input and scenes we are on
	/// </summary>
	public abstract class GameState : IState
	{
		#region Instance
		public virtual string name => GetType().Name.Remove("State");

		protected GameState()
		{
		}
		#endregion

		#region Virtual
		public virtual void Enter() { }
		public virtual void Exit() { }
		#endregion
	}

	/// <summary>
	/// The default state machine singleton provided by the framework.
	/// In this model the state is managed by a stack.
	/// </summary>
	/// <remarks>Implements the interface directly through the instance</remarks>
	public class StateStack : StratusSingleton<StateStack<GameState>>
	{
		public GameState? current => instance.current;

		public static UState Get<UState>() where UState : GameState => instance.Get<UState>();
		public static void Enter<UState>(Action<UState> configure = null) 
			where UState : GameState => instance.Enter(configure);
		public static void Exit() => instance.Exit();

		public static void Entered<UState>(object subscriber, Action action) where UState : GameState
			 => instance.Entered<UState>(subscriber, action);
		public static void Exited<UState>(object subscriber, Action action) where UState : GameState
			=> instance.Exited<UState>(subscriber, action);

		public static void Return<UState>() where UState : GameState
			=> instance.Return<UState>();

		public static void Changed(Action<GameState, StateTransition> callback)
			=> instance.Changed(callback);

		protected override void OnInitialize()
		{
		}
	}

	/// <summary>
	/// A gamestate that uses input
	/// </summary>
	/// <typeparam name="TInputLayer"></typeparam>
	public abstract class InputGameState<TInputLayer> : GameState
		where TInputLayer : InputLayer, new()
	{
		public TInputLayer inputLayer { get; private set; } = new();
	}

	/// <summary>
	/// Usually the default state of the game (after a splash screen)
	/// </summary>
	public class MainMenuState : GameState
	{
	}

	/// <summary>
	/// The main menu for managing the game's options
	/// </summary>
	/// <remarks>This can be configured at runtime before it is entered, in order to add new menus and items</remarks>
	public class OptionsMenuState : GameState
	{
		public Menu menu { get; } = new Menu("Options");
	}

	/// <summary>
	/// The default pause state of the simulation
	/// </summary>
	public class PauseState : GameState
	{
	}

	/// <summary>
	/// Start a new game
	/// </summary>
	public class StartGameEvent : Event
	{
	}

	/// <summary>
	/// Continue an existing game
	/// </summary>
	public class ContinueGameEvent : Event
	{
	}

	/// <summary>
	/// End the current session
	/// </summary>
	public class EndGameEvent : Event
	{
	}
}
