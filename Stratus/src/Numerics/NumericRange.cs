using Stratus.Models.Math;

using System;

namespace Stratus.Numerics
{
	/// <summary>
	/// Base class for numeric range fields
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract record NumericRange<T>(T minimum, T maximum)
		where T : struct
	{
		public abstract T randomInRange { get; }
	}

	/// <summary>
	/// Represents a range consisting of two floating point values
	/// </summary>
	[Serializable]
	public record FloatRange : NumericRange<float>
	{
		public FloatRange(float minimum, float maximum) 
			: base(minimum, maximum)
		{
		}

		public override float randomInRange => RandomUtility.Range(minimum, maximum);
	}

	/// <summary>
	/// Represents a range consisting of two integer values
	/// </summary>
	[Serializable]
	public record IntegerRange : NumericRange<int>
	{
		public IntegerRange(int minimum, int maximum) 
			: base(minimum, maximum)
		{
		}

		public override int randomInRange => RandomUtility.Range(minimum, maximum);
	}

}