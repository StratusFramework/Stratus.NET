using System.Collections.Generic;
using System;
using Stratus.Types;

namespace Stratus.Interpolation
{
	/// <summary>
	/// The ActionSet is the base class from which all other sets derive.
	/// Sets such as Sequence, Group and the unique set used by entities.
	/// </summary>
	public abstract class ActionSet : ActionBase
	{
		#region Fields
		protected List<ActionBase> activeActions = new List<ActionBase>();
		protected List<ActionBase> recentlyAddedActions = new List<ActionBase>();
		#endregion

		#region Events
		public event Action<string> onLog;
		#endregion

		#region Interface
		/// <summary>
		/// Updates the set of actions. This will propagate the updates of all actions
		/// of this set all the way down. Each action itself consumes part of the given time passed.
		/// </summary>
		/// <param name="dt">The engine delta time</param>
		/// <returns>The amount of time that was consumed</returns>
		public abstract override float Update(float dt);

		/// <summary>
		/// Adds an action onto the set.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>The action that was just added</returns>
		public virtual T Add<T>(T action)
			where T : ActionBase
		{
			Log($"Add {action}");
			if (action is ActionSet set)
			{
				set.onLog = onLog;
			}
			recentlyAddedActions.Add(action);
			return action;
		}

		/// <summary>
		/// Adds an action onto the set after constructing it.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>The action that was just added</returns>
		public virtual T Add<T>() where T : ActionBase, new() => Add(ObjectUtility.Instantiate<T>());

		/// <summary>
		/// Adds a range of actions onto the set
		/// </summary>
		public void AddRange(params ActionBase[] actions)
		{
			recentlyAddedActions.AddRange(actions);
		}
		#endregion

		#region Internal
		protected void Log(string message)
		{
			onLog?.Invoke($"[{this}] {message}");
		}

		/// <summary>
		/// Migrates recently added actions
		/// </summary>
		protected void Migrate()
		{
			// Add the new actions (to prevent desync)
			foreach (ActionBase action in recentlyAddedActions)
			{
				activeActions.Add(action);
			}
			recentlyAddedActions.Clear();
		}

		/// <summary>
		/// Sweeps all inactive actions.
		/// </summary>
		protected void Sweep()
		{
			// No actions to clear
			if (activeActions.Count == 0)
			{
				isFinished = true;
				return;
			}

			// Remove all actions that are finished
			int removed = activeActions.RemoveAll(x => x.isFinished == true);
			if (removed > 0)
			{
				Log($"Removed {removed} actions");
			}
		}
		#endregion



	}
}
