using Stratus.Models.Math;

using System;

namespace Stratus.Data
{
	/// <summary>
	/// Base class for numeric range fields
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusNumericRange<T> where T : struct
	{
		public T minimum, maximum;
		public abstract T randomInRange { get; }
	}

	/// <summary>
	/// Represents a range consisting of two floating point values
	/// </summary>
	[Serializable]
	public class StratusFloatRange : StratusNumericRange<float>
	{
		public override float randomInRange => RandomUtility.Range(minimum, maximum);
	}

	/// <summary>
	/// Represents a range consisting of two integer values
	/// </summary>
	[Serializable]
	public class StratusIntegerRange : StratusNumericRange<int>
	{
		public override int randomInRange => RandomUtility.Range(minimum, maximum);
	}

}