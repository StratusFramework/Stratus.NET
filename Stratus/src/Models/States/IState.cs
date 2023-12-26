using System;

namespace Stratus.Models.States
{
	public interface IState
	{
		string name { get; }

		void Enter();
		void Exit();
	}

	public enum StateTransition
	{
		Enter,
		Exit
	}

	public interface IStateMachine<TState>
	where TState : class, IState
	{
		/// <summary>
		/// The current state
		/// </summary>
		TState? current { get; }

		/// <summary>
		/// Enter the given state
		/// </summary>
		/// <typeparam name="UState"></typeparam>
		void Enter<UState>(Action<UState> configure = null) where UState : TState;
		/// <summary>
		/// Exit the current state
		/// </summary>
		void Exit();

		/// <summary>
		/// Set a callback to be invoked when the given state <typeparamref name="UState"/> is entered
		/// </summary>
		/// <typeparam name="UState"></typeparam>
		/// <param name="action"></param>
		void Entered<UState>(object subscriber, Action action) where UState : TState;
		/// <summary>
		/// Set a callback to be invoked when the given state <typeparamref name="UState"/> is exited
		/// </summary>
		/// <typeparam name="UState"></typeparam>
		/// <param name="action"></param>
		void Exited<UState>(object subscriber, Action action) where UState : TState;
		/// <summary>
		/// Set a callback to be invoked when any state changes
		/// </summary>
		/// <param name="callback"></param>
		void Changed(Action<TState, StateTransition> callback);
	}
}
