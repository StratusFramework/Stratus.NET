using Stratus.Serialization;

using System;

namespace Stratus.Data
{
	public abstract class BoundedValueBase<T>
	{
		[SerializeField]
		private T _value;

		/// <summary>
		/// The base value
		/// </summary>
		public T value
		{
			get => _value;
			set
			{
				T previous = _value;
				_value = value;
				onValueChanged?.Invoke(previous, value);
			}
		}

		public event Action<T, T> onValueChanged;

		protected BoundedValueBase(T baseValue)
		{
			this._value = baseValue;
		}
	}

	/// <summary>
	/// A set of parameters which can be used to constrain the modification of a value
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ValueConstraint<T>
		// TODO: Replace with INumber<T> in NET 7 "someday"
		// https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/
		where T : struct, IConvertible
	{
		/// <summary>
		/// The lowest value allowed
		/// </summary>
		public T floor { get; set; }
		/// <summary>
		/// The highest value allowed
		/// </summary>
		public T ceiling { get; set; }
	}

	public class ValueModifier<T>
	{
		public string name;
		public T modifier;
	}

	/// <summary>
	/// A numeric value, with a set floor and a ceiling that 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BoundedValue<T> : BoundedValueBase<T>
		// TODO: Replace with INumber<T> in NET 7 "someday"
		// https://devblogs.microsoft.com/dotnet/dotnet-7-generic-math/
		where T : struct, IConvertible
	{
		#region Fields
		[SerializeField]
		private T _modifier;
		[SerializeField]
		private T _increment;
		#endregion

		#region Properties
		/// <summary>
		/// The maximum value of this parameter. Base + modifiers.
		/// </summary>
		public T maximum => Add(value, modifier);
		/// <summary>
		/// The current value
		/// </summary>
		public T current => Add(value, _increment);
		/// <summary>
		/// The modifier, which adjusts the maximum value.
		/// </summary>
		public T modifier
		{
			get => _modifier;
			set
			{
				_modifier = value;
				onModified?.Invoke();
			}
		}

		/// <summary>
		/// Whether this parameter's current value is at its minimum value
		/// </summary>
		public bool minimal => Substract(current, modifier).Equals(zero);
		/// <summary>
		/// Whether this parameter's current value is at its maximum value
		/// </summary>
		public bool maximal => current.Equals(maximum);

		/// <summary>
		/// If set, constrains how the value is updated
		/// </summary>
		public ValueConstraint<T> constraints { get; set; }
		/// <summary>
		/// Whether constraints for the value has been set
		/// </summary>
		public bool hasConstraints => constraints != null;
		#endregion

		#region Events
		/// <summary>
		/// Invoked when this attribute reaches its maximum value
		/// </summary>
		public event Action onMaximum;
		/// <summary>
		/// Invoked when this attribute reaches its minimum value
		/// </summary>
		public event Action onMinimum;
		/// <summary>
		/// Invoked whenever the modifier changes
		/// </summary>
		public event Action onModified;
		/// <summary>
		/// Emits the current percentage change
		/// </summary>
		public event Action<float> onPercentageChanged;
		#endregion

		#region Constants
		public T zero => default;
		#endregion

		#region Virtual
		/// <summary>
		/// The current ratio of the parameter when compared to its maximum as a percentage
		/// </summary>
		public abstract float percentage { get; }
		protected abstract T Add(T a, T b);
		protected abstract T Substract(T a, T b);
		protected abstract bool LessThan(T a, T b);
		protected abstract bool GreaterThan(T a, T b);
		protected abstract T Negate(T a);
		protected abstract T Abs(T a);
		#endregion

		#region Constructors
		protected BoundedValue(T baseValue = default) : base(baseValue)
		{
		}
		#endregion

		#region Operators
		public static implicit operator float(BoundedValue<T> bounded) => (float)(object)bounded.current;
		public static implicit operator int(BoundedValue<T> bounded) => (int)(object)bounded.current;
		#endregion

		#region Strings
		public override string ToString()
		{
			return $"({current}/{maximum})";
		}

		public string ToPercentageString()
		{
			return $"{percentage}%";
		}
		#endregion

		#region Interface
		/// <summary>
		/// Adds to the increment of this parameter, up to the maximum value
		/// </summary>
		/// <param name="value"></param>
		public Result Increase(T value)
		{
			if (maximal)
			{
				return new Result(false, $"Value {value} already at maximum.");
			}

			float previousPercentage = percentage;

			_increment = Add(_increment, value);

			if (GreaterThan(current, maximum))
			{
				_increment = Substract(_increment, Substract(current, maximum));
			}

			if (hasConstraints)
			{
				if (GreaterThan(this.value, constraints.ceiling))
				{
					_increment = Substract(constraints.ceiling, _increment);
				}
			}

			float percentageGained = percentage - previousPercentage;

			onPercentageChanged?.Invoke(percentageGained);
			return true;
		}

		/// <summary>
		/// Reduces the increment of this parameter, up to its minimum value
		/// </summary>
		/// <param name="value"></param>
		/// <returns>How much was lost, as a percentage of the total value of this parameter</returns>
		public Result Decrease(T value)
		{
			if (minimal)
			{
				return new Result(false, $"Value {value} already at minimum");
			}

			if (LessThan(value, zero))
			{
				throw new ArgumentException($"The input value for decrease '{value}' must be positive!");
			}

			float previousPercentage = percentage;

			_increment = Substract(_increment, value);

			if (hasConstraints)
			{
				if (LessThan(_increment, constraints.floor))
				{
					_increment = Negate(maximum);
				}
			}

			float percentageLost = previousPercentage - percentage;
			onPercentageChanged?.Invoke(percentageLost);

			if (minimal)
			{
				onMinimum?.Invoke();
			}
			return true;
		}

		/// <summary>
		/// Sets the current value forcefully, ignoring modifiers
		/// </summary>
		/// <param name="value"></param>
		public void DecreaseToFloor()
		{
			T value = Substract(maximum, Abs(_modifier));
			Decrease(value);
		}

		/// <summary>
		/// Resets the current value of this parameter back to maximum
		/// </summary>
		public void Reset()
		{
			_increment = default;
		}

		public void Constrain(Action<ValueConstraint<T>> configure)
		{
			constraints = new ValueConstraint<T>();
			configure(constraints);
		}
		#endregion

	}

	/// <summary>
	/// A numeric value that is constrained by a floor and a ceiling
	/// </summary>
	public class BoundedFloat : BoundedValue<float>
	{
		#region Properties

		public override float percentage => value / maximum * 100.0f;
		#endregion

		#region Virtual
		protected override float Add(float a, float b) => a + b;
		protected override bool LessThan(float a, float b) => a < b;
		protected override bool GreaterThan(float a, float b) => a > b;
		protected override float Substract(float a, float b) => a - b;
		protected override float Negate(float a) => -a;
		protected override float Abs(float a) => MathF.Abs(a);
		#endregion

		#region Constructor
		public BoundedFloat(float baseValue = 0) : base(baseValue)
		{
		}
		#endregion
	}

	/// <summary>
	/// An integer that is constrained by a floor and a ceiling
	/// </summary>
	public class BoundedInteger : BoundedValue<int>
	{
		public override float percentage => value / maximum * 100.0f;

		protected override int Abs(int a) => Math.Abs(a);
		protected override int Add(int a, int b) => a + b;
		protected override bool GreaterThan(int a, int b) => a > b;
		protected override bool LessThan(int a, int b) => a < b;
		protected override int Negate(int a) => -a;
		protected override int Substract(int a, int b) => a - b;
	}


}