using Stratus.Events;
using Stratus.Types;
using System.Collections.Generic;
using System;
using Stratus.Extensions;

namespace Stratus.Models.States
{
	/// <summary>
	/// A state machine that manages the state in a stack
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	public interface IStateStack<TState> : IStateMachine<TState>
		where TState : class, IState
	{
		/// <summary>
		/// Exit current states until reaching <typeparamref name="UState"/>
		/// </summary>
		/// <typeparam name="UState"></typeparam>
		void Return<UState>() where UState : TState;
	}

	// TODO: Unit test
	/// <summary>
	/// Provides static access to the given state of type <typeparamref name="TState"/>
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	public class StateStack<TState> : IStateStack<TState>
		where TState : class, IState
	{
		private TypeInstancer<TState> instancer = new TypeInstancer<TState>();
		private Stack<TState> states = new Stack<TState>();
		public TState? current => states.PeekOrDefault();

		// Onetime callbacks [State > ]
		private Dictionary<Type, List<DelegateBinding>> enteredCallbacks = new();
		private Dictionary<Type, List<DelegateBinding>> exitedCallbacks = new();
		private event Action<TState, StateTransition> onTransition;

		public UState Get<UState>() where UState : TState => instancer.Get<UState>();

		public void Enter<UState>(Action<UState> configure = null) where UState : TState
		{
			var next = Get<UState>();
			configure?.Invoke(next);
			Enter(next);
		}

		private TState Enter(TState next, Action<TState> configure = null)
		{
			if (current == next)
			{
				return current;
			}

			states.Push(next);
			NotifyForCurrent(StateTransition.Enter);
			return current;
		}

		public void Exit()
		{
			NotifyForCurrent(StateTransition.Exit);
			states.Pop();
			NotifyForCurrent(StateTransition.Enter);
		}

		public void Return<UState>() where UState : TState
		{
			while (states.Count > 0 && !(current is UState))
			{
				NotifyForCurrent(StateTransition.Exit);
				states.Pop();
			}

			if (current is UState)
			{
				NotifyForCurrent(StateTransition.Enter);
			}
		}

		private void NotifyForCurrent(StateTransition transition)
		{
			void notify(List<DelegateBinding> list)
			{
				list.RemoveAll(b => b.subscriber == null
				|| b.action.Target == null || b.action.Method == null);

				list.ForEach(b =>
				{
					b.action.DynamicInvoke();
				});
			}

			if (current != null)
			{
				switch (transition)
				{
					case StateTransition.Enter:
						current.Enter();
						if (enteredCallbacks.TryGetValue(current.GetType(), out var entered))
						{
							notify(entered);
						}
						break;
					case StateTransition.Exit:
						current.Exit();
						if (exitedCallbacks.TryGetValue(current.GetType(), out var exited))
						{
							notify(exited);
						}
						break;
				}
				onTransition?.Invoke(current, transition);
			}
		}

		public void Entered<UState>(object subscriber, Action action)
			where UState : TState
		{
			Type type = typeof(UState);
			Subscribe(enteredCallbacks, subscriber, action, type);
		}

		public void Exited<UState>(object subscriber, Action action)
			where UState : TState
		{
			Type type = typeof(UState);
			Subscribe(exitedCallbacks, subscriber, action, type);
		}

		private void Subscribe(Dictionary<Type, List<DelegateBinding>> dictionary, object subscriber, Action action, Type type)
		{
			if (!dictionary.ContainsKey(type))
			{
				dictionary.Add(type, new());
			}
			dictionary[type].Add(new DelegateBinding(subscriber, action));
		}

		public void Changed(Action<TState, StateTransition> callback)
		{
			onTransition += callback;
		}
	}
}
