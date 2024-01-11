using Stratus.Extensions;
using Stratus.Inputs;
using Stratus.Models.States;
using Stratus.Utilities;

using System;
using System.Collections.Generic;

namespace Stratus.Models.Gameflow
{
    /// <summary>
    /// The game states usually dictate the input and scenes we are on
    /// </summary>
    public abstract class State : IState
    {
        #region Instance
        public virtual string name => GetType().Name.Remove("State");

        protected State()
        {
        }
        #endregion

        #region Virtual
        public virtual void Enter() { }
        public virtual void Exit() { }
        #endregion
    }

	/// <summary>
	/// A gamestate that uses input
	/// </summary>
	/// <typeparam name="TInputLayer"></typeparam>
	public abstract class InputState<TInputLayer> : State
		where TInputLayer : InputLayer, new()
	{
		protected TInputLayer inputLayer { get; set; } = new();
        private bool initialized { get; set; }

        public void Initialize(Action<TInputLayer> configure)
        {
            configure(inputLayer);
            initialized = true;
        }
	}

	/// <summary>
	/// The default state machine singleton provided by the framework.
	/// In this model the state is managed by a stack.
	/// </summary>
	/// <remarks>Implements the interface directly through the instance</remarks>
	public class GameState : StratusSingleton<StateStack<State>>
    {
        public State? current => instance.current;

        public static UState Get<UState>() where UState : State => instance.Get<UState>();
        public static void Enter<UState>(Action<UState> configure = null)
            where UState : State => instance.Enter(configure);
        public static void Exit() => instance.Exit();

        public static void Entered<UState>(object subscriber, Action action) where UState : State
             => instance.Entered<UState>(subscriber, action);
        public static void Exited<UState>(object subscriber, Action action) where UState : State
            => instance.Exited<UState>(subscriber, action);

        public static void Return<UState>() where UState : State
            => instance.Return<UState>();

        public static void Changed(Action<State, StateTransition> callback)
            => instance.Changed(callback);

        protected override void OnInitialize()
        {
        }
    }
}
