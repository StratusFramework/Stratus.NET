namespace Stratus.Interpolation
{
	/// <summary>
	/// Invokes a function immediately
	/// </summary>
	public class ActionCall : ActionBase
	{
		public delegate void Delegate();

		private Delegate action;

		public ActionCall(Delegate action)
		{
			this.action = action;
		}

		/// <summary>
		/// Updates the action
		/// </summary>
		/// <param name="dt">The delta time.</param>
		/// <returns>How much time was consumed during this action.</returns>
		public override float Update(float dt)
		{
			if (logging)
			{
				StratusLog.Info("#" + this.id + ": Calling function '" + this.action.Method.Name + "'");
			}

			this.action.Invoke();
			this.isFinished = true;

			if (logging)
			{
				StratusLog.Info("#" + this.id + ": Finished!");
			}

			return 0.0f;
		}
	}


	/// <summary>
	/// Invokes a function immediately
	/// </summary>
	public class ActionCall<T> : ActionBase
	{
		public delegate void Delegate<T>();

		private Delegate<T> action;

		public ActionCall(Delegate<T> action)
		{
			this.action = action;
		}

		/// <summary>
		/// Updates the action
		/// </summary>
		/// <param name="dt">The delta time.</param>
		/// <returns>How much time was consumed during this action.</returns>
		public override float Update(float dt)
		{
			if (logging)
			{
				StratusLog.Info("#" + this.id + ": Calling function '" + this.action.Method.Name + "'");
			}

			// If the target was destroyed in the meantime...
			if (this.action.Target == null)
			{
				return 0f;
			}

			this.action.Invoke();
			this.isFinished = true;

			if (logging)
			{
				StratusLog.Info("#" + this.id + ": Finished!");
			}

			return 0.0f;
		}
	}
}