using System.Collections.Generic;
using System;

namespace Stratus.Interpolation
{
	/// <summary>
	/// The ActionSet is the base class from which all other sets derive.
	/// Sets such as Sequence, Group and the unique set used by entities.
	/// </summary>
	public abstract class ActionSet : ActionBase
	{
		//---------------------------------------------------------------------/
		// Fields
		//---------------------------------------------------------------------/
		public StratusTimeScale timescale = StratusTimeScale.Delta;
		protected List<ActionBase> activeActions = new List<ActionBase>();
		protected List<ActionBase> recentlyAddedActions = new List<ActionBase>();

		//---------------------------------------------------------------------/
		// Messages
		//---------------------------------------------------------------------/
		public ActionSet(StratusTimeScale mode)
		{
			this.timescale = mode;
		}
		public abstract override float Update(float dt);

		//---------------------------------------------------------------------/
		// Methods
		//---------------------------------------------------------------------/
		/// <summary>
		/// Add an action to this set
		/// </summary>
		/// <param name="action">The specified action.</param>
		public virtual void Add(ActionBase action)
		{
			this.recentlyAddedActions.Add(action);
		}

		/// <summary>
		/// Migrates new actions over.
		/// </summary>
		protected void Migrate()
		{
			// Add the new actions (to prevent desync)
			foreach (ActionBase action in this.recentlyAddedActions)
			{
				this.activeActions.Add(action);
			}
			this.recentlyAddedActions.Clear();
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
	}
}
