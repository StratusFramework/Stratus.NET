using Stratus.Utilities;

using System.Collections.Generic;

namespace Stratus.Interpolation
{
	/// <summary>
	/// A container of all actions a particular GameObject has.
	/// They propagate updates to all actions attached to it.
	/// </summary>
	public class ActionContainer : ActionGroup
	{
		public ActionContainer(StratusTimeScale mode = StratusTimeScale.Delta)
			: base(mode)
		{
		}

		/// <summary>
		/// Updates an entity's actions, updating all the actions one tier below in parallel.
		/// </summary>
		/// <param name="dt">The time to be updated.</param>
		/// <returns>How much time was consumed while updating.</returns>
		public override float Update(float dt)
		{
			this.Migrate();
			return base.Update(dt);
		}
	}

	/// <summary>
	/// Manages the update of each <see cref="ActionBase"/> of the interpolation system.
	/// It must be manually updated by an external entity within the context of a game engine.
	/// </summary>
	public class ActionScheduler<T>
	{
		#region Declaration
		public class Instance
		{
			public T target;
			public ActionContainer owner;

			public Instance(T target, ActionContainer owner)
			{
				this.target = target;
				this.owner = owner;
			}
		}
		#endregion

		#region Properties
		private List<Instance> activeActions { get; } = new List<Instance>();
		private Dictionary<T, Instance> actionsOwnerMap { get; } = new Dictionary<T, Instance>();
		#endregion

		#region Events
		public event System.Action<T> onConnect;
		public event System.Action<T> onDisconnect;
		#endregion

		#region Interface
		/// <summary>
		/// Propagates an update to all active actions through ActionOwners.
		/// </summary>
		public void Update(float dt)
		{
			Instance[] currentActions = activeActions.ToArray();
			for (int i = 0; i < currentActions.Length; ++i)
			{
				currentActions[i].owner.Update(dt);
			}
		}

		public ActionContainer Connect(T gameObj)
		{
			return SubscribeToActions(gameObj);
		}

		/// <summary>
		/// Unsubscribes the specified GameObject from the ActionSpace.
		/// </summary>
		/// <param name="gameObj">A reference to the gameobject.</param>
		public void Disconnect(T gameObj)
		{
			UnsubscribeFromActions(gameObj);
		}
		#endregion

		#region Procedures
		/// <summary>
		/// Subscribes this gameobject to the action space
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		private ActionContainer SubscribeToActions(T target)
		{
			if (this.actionsOwnerMap.ContainsKey(target))
			{
				return this.actionsOwnerMap[target].owner;
			}

			ActionContainer owner = new ActionContainer();
			Instance container = new Instance(target, owner);

			this.activeActions.Add(container);
			this.actionsOwnerMap.Add(target, container);
			onConnect?.Invoke(target);
			return owner;
		}

		/// <summary>
		/// Unsubscribe this gameobject from the action system
		/// </summary>
		/// <param name="target"></param>
		private void UnsubscribeFromActions(T target)
		{
			// @TODO: Why is this an issue?
			if (target == null)
			{
				return;
			}

			Instance container = this.actionsOwnerMap[target];
			onDisconnect.Invoke(target);
			this.activeActions.Remove(container);
			this.actionsOwnerMap.Remove(target);
		}
		#endregion
	}

	public class ActionScheduler : ActionScheduler<object>
	{
	}
}