﻿using System.Collections.Generic;

namespace Stratus.Interpolation
{
	/// <summary>
	/// Manages the update of each <see cref="ActionBase"/> of the interpolation system.
	/// It must be manually updated by an external entity within the context of a game engine.
	/// </summary>
	public class ActionScheduler<T>
	{
		#region Declaration
		public record Instance
		{
			public T target;
			public ActionContainer container;

			public Instance(T target, ActionContainer owner)
			{
				this.target = target;
				this.container = owner;
			}
		}
		#endregion

		#region Properties
		private List<Instance> _actions { get; } = new List<Instance>();
		private Dictionary<T, Instance> actionInstanceMap { get; } = new Dictionary<T, Instance>();
		public bool empty => _actions.Count == 0;
		#endregion

		#region Events
		public event System.Action<T> onConnect;
		public event System.Action<T> onDisconnect;
		public event System.Action<Instance> onUpdate;
		#endregion

		#region Interface
		public void Update(double dt) => Update((float)dt);

		public void Update(float dt)
		{
			if (empty)
			{
				return;
			}

			Instance[] actions = _actions.ToArray();
			for (int i = 0; i < actions.Length; ++i)
			{
				onUpdate?.Invoke(actions[i]);
				actions[i].container.Update(dt);
			}
		}

		public ActionContainer Connect(T target)
		{
			return OnConnect(target);
		}

		public void Disconnect(T target)
		{
			OnDisconnect(target);
		}
		#endregion

		#region Procedures
		private ActionContainer OnConnect(T target)
		{
			if (this.actionInstanceMap.ContainsKey(target))
			{
				return this.actionInstanceMap[target].container;
			}

			ActionContainer owner = new ActionContainer();
			Instance container = new Instance(target, owner);

			this._actions.Add(container);
			this.actionInstanceMap.Add(target, container);
			onConnect?.Invoke(target);
			return owner;
		}

		private void OnDisconnect(T target)
		{
			// @TODO: Why is this an issue?
			if (target == null)
			{
				return;
			}

			Instance container = this.actionInstanceMap[target];
			onDisconnect.Invoke(target);
			this._actions.Remove(container);
			this.actionInstanceMap.Remove(target);
		}
		#endregion
	}

	public class ActionScheduler : ActionScheduler<object>
	{
	}

	public static class ActionSchedulerExtensions
	{
		public static TAction Action<TObject, TAction>(this ActionScheduler<TObject> scheduler, TObject target)
			where TAction : ActionBase, new()
		{
			TAction sequence = new TAction();
			var actions = scheduler.Connect(target);
			actions.Add(sequence);
			return sequence;
		}

		public static ActionSequence Sequence<TObject>(this ActionScheduler<TObject> scheduler, TObject target)
			=> Action<TObject, ActionSequence>(scheduler, target);

		public static ActionGroup Group<TObject>(this ActionScheduler<TObject> scheduler, TObject target)
			=> Action<TObject, ActionGroup>(scheduler, target);
	}
}