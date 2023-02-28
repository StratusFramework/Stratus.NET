using Stratus.Timers;

namespace Stratus.Interpolation
{
	/// <summary>
	/// A general-purpose utility class for interpolation of specific types
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Interpolator<T>
	{
		#region Fields
		protected T _startingValue;
		protected T _currentValue;
		protected T _EndingValue;
		bool _Active;
		Stopwatch Timer;
		#endregion

		#region Virtual
		protected abstract void Interpolate(float t);
		#endregion

		#region Properties
		/// <summary>
		/// The starting value
		/// </summary>
		public T startingValue
		{
			set
			{
				_startingValue = value;
			}
		}

		/// <summary>
		/// The current value
		/// </summary>
		public T currentValue
		{
			get
			{
				if (Active)
					return _currentValue;
				return _EndingValue;
			}
		}

		/// <summary>
		/// The ending value
		/// </summary>
		public T endingValue
		{
			set
			{
				_EndingValue = value;
			}
		}

		/// <summary>
		/// Whether it is currently interpolating
		/// </summary>
		public bool Active { get { return _Active; } }
		#endregion

		#region Interface
		/// <summary>
		/// Begins interpolation
		/// </summary>
		/// <param name="time"></param>
		public void Start(float time)
		{
			Timer = new Stopwatch(time);
			_Active = true;
		}

		/// <summary>
		/// Updates the interpolator
		/// </summary>
		/// <param name="dt"></param>
		public void Update(float dt)
		{
			if (Active)
			{
				if (Timer.Update(dt))
				{
					Timer.Reset();
					_Active = false;
				}

				float t = Timer.normalizedProgress;
				Interpolate(t);
			}
		}
		#endregion
	}
}
