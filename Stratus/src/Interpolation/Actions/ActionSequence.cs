namespace Stratus.Interpolation
{
	/// <summary>
	/// An ActionSequence is a type of set that updates all its actions
	/// and children in sequence, depleting its time slice as it updates
	/// each.
	/// </summary>
	public class ActionSequence : ActionSet
	{
		/// <summary>
		/// Updates an ActionSequence, by updating the actions in the sequence
		/// sequentially.
		/// </summary>
		/// <param name="dt">The time to be updated</param>
		/// <returns>How much time was consumed while updating</returns>
		public override float Update(float dt)
		{
			Migrate();

			var timeLeft = dt;
			foreach (var action in activeActions)
			{
				// If an action is inactive, stop the sequence (since its blocking)
				if (action.isActive)
				{
					break;
				}

				Log($"Updating {action}");
				// Every action consumes time from the time slice given (dt)
				timeLeft -= action.Update(dt);
				// If the action was completed (Meaning there is time remaining
				// after it was updated, then it will be cleared on the next frame!
				if (timeLeft <= 0)
				{
					break;
				}
			}

			// Sweep all inactive actions
			Sweep();

			return dt - timeLeft;
		}
	}
}
