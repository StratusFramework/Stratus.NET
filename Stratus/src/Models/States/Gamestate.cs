using Stratus.Events;
using Stratus.Extensions;
using Stratus.Types;

using System;
using System.Collections.Generic;

namespace Stratus.Models.States
{
	public interface IGameState
	{
		string name { get; }
		event Action<StateTransition> onTransition;
	}

	public enum StateTransition
	{
		Push,
		Pop,

		Enabled,
		Disabled
	}

	/// <summary>
	/// The game states usually dictate the input and scenes we are on
	/// </summary>
	public abstract class GameState : IGameState
	{
		#region Instance
		public virtual string name
		{
			get => GetType().Name.Remove("State");
		}

		public event Action<StateTransition> onTransition;

		protected GameState()
		{
		}
		#endregion

		#region Static Interface
		private static TypeInstancer<GameState> instancer
			= new TypeInstancer<GameState>();

		private static Stack<GameState> states = new Stack<GameState>();
		public static GameState? current => states.PeekOrDefault();

		public static event Action<GameState, StateTransition> onChange;

		public static TState Push<TState>()
			where TState : GameState, new()
			=> (TState)Push(instancer.Get(typeof(TState)));

		private static GameState Push(GameState next)
		{
			if (current == next)
			{
				return current;
			}

			NotifyForCurrent(StateTransition.Disabled);
			states.Push(next);
			NotifyForCurrent(StateTransition.Enabled);
			return current;
		}

		public static void Pop()
		{
			NotifyForCurrent(StateTransition.Disabled);
			states.Pop();
			NotifyForCurrent(StateTransition.Enabled);
		}

		private static void NotifyForCurrent(StateTransition transition)
		{
			if (current != null)
			{
				current.onTransition?.Invoke(transition);
				onChange?.Invoke(current, transition);
			}
		}

		public static void Return<TGameState>()
			where TGameState : GameState
		{
			while (states.Count > 0
				&& !(current is TGameState))
			{
				Pop();
			}
		}

		public static void Enabled<TState>(Action action) where TState : GameState
			=> When<TState>(StateTransition.Enabled, action);

		public static void Disabled<TState>(Action action) where TState : GameState
			=> When<TState>(StateTransition.Disabled, action);

		public static void When<TState>(StateTransition transition, Action action)
			where TState : GameState
		{
			onChange += (s, t) =>
			{
				if (s is TState &&
					t.Equals(transition))
				{
					action();
				}
			};
		}
		#endregion
	}

	/// <summary>
	/// Usually the default state of the game (after a splash screen)
	/// </summary>
	public class MainMenuState : GameState
	{
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
	public class NewGameEvent : Event
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
