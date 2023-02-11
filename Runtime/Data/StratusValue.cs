using Stratus.Serialization;

using System;

namespace Stratus.Data
{
	public abstract class StratusValueBase
	{
		private float _baseValue;

		/// <summary>
		/// The base value of this parameter
		/// </summary>
		public float baseValue
		{
			get => _baseValue;
			set => _baseValue = value;
		}

		protected StratusValueBase(float baseValue)
		{
			this._baseValue = baseValue;
		}
	}

	[Serializable]
	public class StratusValue : StratusValueBase
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private float _increment;
		[SerializeField]
		private float _bonusValue;

		[SerializeField]
		private float floor;
		[SerializeField]
		private float ceiling;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public float bonusValue => _bonusValue;
		public float increment => _increment;

		/// <summary>
		/// The maximum value of this parameter. Base + modifiers.
		/// </summary>
		public float maximum => baseValue + _bonusValue;
		/// <summary>
		/// The current, total value of this parameter
		/// </summary>
		public float total => maximum + _increment;

		/// <summary>
		/// The current ratio of the parameter when compared to its maximum as a percentage
		/// </summary>
		public float percentage => total / maximum * 100.0f;
		/// <summary>
		/// Whether this parameter's current value is at its maximum value
		/// </summary>
		public bool isAtMaximum => total == maximum;
		/// <summary>
		/// Whether this parameter's current value is at its minimum value
		/// </summary>
		public bool isAtMinimum => total == floor;
		/// <summary>
		/// Returns an instance with a value of 1
		/// </summary>
		public static StratusValue one => new StratusValue(1f);
		/// <summary>
		/// If locked, prevents modifications
		/// </summary>
		public bool locked { get; set; }
		/// <summary>
		/// The last increment (from an increase/decrease call)
		/// </summary>
		public float lastIncrement { get; private set; }

		//------------------------------------------------------------------------/
		// Functions
		//------------------------------------------------------------------------/
		public Func<float, float> increaseModifier { get; set; }
		public Func<float, float> decreaseModifier { get; set; }

		//------------------------------------------------------------------------/
		// Events
		//------------------------------------------------------------------------/
		/// <summary>
		/// Invoked when this attribute reaches its maximum value
		/// </summary>
		public event Action onMaximum;
		/// <summary>
		/// Emits the current percentage change of this attribute
		/// </summary>
		public event Action<float> onModified;
		/// <summary>
		/// Invoked when this attribute reaches its minimum value
		/// </summary>
		public event Action onMinimum;

		//------------------------------------------------------------------------/
		// CTOR
		//------------------------------------------------------------------------/
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseVaue">The base value of the parameter</param>
		/// <param name="floor">The minimum value for this parameter</param>
		public StratusValue(float baseValue, float floor = 0.0f, float ceiling = float.MaxValue)
			: base(baseValue)
		{
			this.floor = floor;
			this.ceiling = ceiling;
			this._bonusValue = 0.0f;
			Reset();
		}

		public StratusValue() : this(0f)
		{
		}

		public override string ToString()
		{
			return $"({baseValue}, {total}, {bonusValue})";
		}

		public string ToPercentageString()
		{
			return $"{total}/{maximum}";
		}

		public static implicit operator float(StratusValue attribute) => attribute.total;
		public static implicit operator int(StratusValue attribute) => (int)attribute.total;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Resets the current value of this parameter back to maximum
		/// </summary>
		public void Reset()
		{
			_increment = 0;
		}

		/// <summary>
		/// Adds to the current value of this parameter, up to the maximum value
		/// </summary>
		/// <param name="value"></param>
		public bool Increase(float value)
		{
			if (locked)
			{
				return false;
			}

			if (isAtMaximum)
			{
				return false;
			}

			if (increaseModifier != null)
			{
				value = increaseModifier(value);
			}

			float previousPercentage = percentage;

			lastIncrement = value;
			_increment += value;
			if (total > maximum) _increment = maximum - total;
			if (total > ceiling) _increment = ceiling - total;
			float percentageGained = percentage - previousPercentage;

			onModified?.Invoke(percentageGained);
			return true;
		}

		/// <summary>
		/// Reduces the current value of this parameter, up to its minimum value
		/// </summary>
		/// <param name="value"></param>
		/// <returns>How much was lost, as a percentage of the total value of this parameter</returns>
		public bool Decrease(float value)
		{
			if (locked)
			{
				return false;
			}

			if (value < 0f)
				throw new ArgumentException($"The input value for decrease '{value}' must be positive!");

			if (decreaseModifier != null)
			{
				value = decreaseModifier(value);
			}

			float previousPercentage = percentage;

			lastIncrement = value;
			_increment -= value;
			if (total < floor) _increment = -maximum;
			float percentageLost = previousPercentage - percentage;
			onModified?.Invoke(percentageLost);

			if (isAtMinimum)
			{
				onMinimum?.Invoke();
			}
			return true;
		}

		/// <summary>
		/// Adds a positive temporary modifier to this parameter
		/// </summary>
		/// <param name="bonus"></param>
		public void AddBonus(float bonus)
		{
			this._bonusValue += bonus;
		}

		/// <summary>
		/// Sets the modifier of this parameter to a flat value
		/// </summary>
		/// <param name="modifier"></param>
		public void SetBonus(float modifier)
		{
			this._bonusValue = modifier;
		}

		/// <summary>
		/// Clears all temporary modifiers for this parameter
		/// </summary>
		public void ClearBonus()
		{
			_bonusValue = 0.0f;
		}

		/// <summary>
		/// Sets the current value forcefully, ignoring modifiers
		/// </summary>
		/// <param name="value"></param>
		public void DecreaseToFloor()
		{
			float value = maximum - Math.Abs(_increment);
			Decrease(value);
		}
	}
}