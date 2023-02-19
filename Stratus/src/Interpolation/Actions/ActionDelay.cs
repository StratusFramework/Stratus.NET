namespace Stratus.Interpolation
{
	/// <summary>
	/// An ActionDelay is a type of action that blocks all actions behind it
	/// for a specified amount of time.
	/// </summary>
	public class ActionDelay : ActionBase
	{
		public ActionDelay(float duration)
		{
			this.duration = duration;
		}

		public override float Update(float dt)
		{
			this.elapsed += dt;
			float timeLeft = this.duration - this.elapsed;

			if (this.elapsed >= this.duration)
			{
				this.isFinished = true;
				if (logging)
				{
					StratusLog.Info("Finished!");
				}
			}
			else
			{
				if (logging)
				{
					StratusLog.Info("dt = '" + dt + "', timeLeft = '" + timeLeft + "'");
				}
			}

			// I don't remember why this works @_@
			if (timeLeft < dt)
			{
				return dt;
			}
			else
			{
				return timeLeft;
			}
		}
	}

}

