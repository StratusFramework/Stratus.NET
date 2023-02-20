using System;

namespace Stratus.Timers
{
	/// <summary>
	/// The base class for all timers
	/// </summary>
	public abstract class Timer
	{
		#region Properties
		/// <summary>
		/// Returns the maximum duration for this timer
		/// </summary>
		public float total { protected set; get; }
		/// <summary>
		/// Returns the currently elapsed time for this timer
		/// </summary>
		public float current { protected set; get; }
		/// <summary>
		/// The remaining duration on the timer before its complete
		/// </summary>
		public abstract float remaining { get; }
		/// <summary>
		/// Whether this timer has finished running
		/// </summary>
		public bool isFinished { protected set; get; }
		/// <summary>
		/// The current progress in this timer as a percentage value ranging from 0 to 1.
		/// </summary>
		public virtual float normalizedProgress { get { if (total == 0.0f) return 0.0f; return current / total; } }
		/// <summary>
		/// The inverse of the normalized progress, as a percentage value ranging from 0 to 1
		/// </summary>
		public virtual float inverseNormalizedProgress => 1f - normalizedProgress;
		/// <summary>
		/// The current progress in this timer as a percentage value ranging from 0 to 100.
		/// </summary>
		public virtual float progress { get { if (total == 0.0f) return 0.0f; return current / total * 100.0f; } }
		/// <summary>
		/// Whether this timer should automatically reset when it has finished
		/// </summary>
		public bool resetOnFinished { get; set; } = false;
		#endregion

		#region Events
		/// <summary>
		/// The callback function for when this timer finishes
		/// </summary>
		public event Action onFinished;
		#endregion

		#region Virtual
		public abstract void Set(float time);
		public abstract bool Update(float dt);
		protected abstract void OnReset();
		#endregion

		#region Interface
		/// <summary>
		/// Finishes the timer
		/// </summary>
		public void Finish()
		{
			if (!isFinished)
				this.onFinished?.Invoke();
			isFinished = true;
			if (resetOnFinished)
				Reset();
		}

		/// <summary>
		/// Sets a callback function to be called when this timer finishes
		/// </summary>
		/// <param name="onFinished"></param>
		public void WhenFinished(Action onFinished)
		{
			this.onFinished = onFinished;
		}

		/// <summary>
		/// Resets the timer
		/// </summary>
		public void Reset()
		{
			isFinished = false;
			this.OnReset();
		} 
		#endregion
	}
}
