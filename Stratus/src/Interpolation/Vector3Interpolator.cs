using Stratus.Interpolation;

using System.Numerics;

namespace Stratus.Interpolation
{
	/// <summary>
	/// Interpolates a Vector3
	/// </summary>
	public class Vector3Interpolator : Interpolator<Vector3>
	{
		protected override void Interpolate(float t)
		{
			_currentValue = Vector3.Lerp(_startingValue, _EndingValue, t);
		}
	}

	/// <summary>
	/// Interpolates a Vector2
	/// </summary>
	public class Vector2Interpolator : Interpolator<Vector2>
	{
		protected override void Interpolate(float t)
		{
			_currentValue = Vector2.Lerp(_startingValue, _EndingValue, t);
		}
	}
}
