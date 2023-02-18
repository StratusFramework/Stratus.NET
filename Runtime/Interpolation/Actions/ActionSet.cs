using System.Collections.Generic;
using System;
using Stratus.Utilities;

namespace Stratus.Interpolation
{
	/// <summary>
	/// The ActionSet is the base class from which all other sets derive.
	/// Sets such as Sequence, Group and the unique set used by entities.
	/// </summary>
	public abstract class ActionSet : ActionBase
	{
		protected List<ActionBase> activeActions = new List<ActionBase>();
		protected List<ActionBase> recentlyAddedActions = new List<ActionBase>();

		public abstract override float Update(float dt);

		#region Interface
		public virtual T Add<T>(T action)
			where T : ActionBase
		{
			recentlyAddedActions.Add(action);
			return action;
		}

		public virtual T Add<T>() where T : ActionBase, new()  => Add(ObjectUtility.Instantiate<T>());			

		public void AddRange(params ActionBase[] actions)
		{
			recentlyAddedActions.AddRange(actions);
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
		public void Sweep()
		{
			// No actions to clear
			if (this.activeActions.Count == 0)
			{
				return;
			}

			// Remove all actions that are finished
			this.activeActions.RemoveAll(x => x.isFinished == true);
		}

		/// <summary>
		/// Clears all actions.
		/// </summary>
		public void Clear()
		{
			this.activeActions.Clear();
			this.recentlyAddedActions.Clear();
		} 
		#endregion
	}
}
