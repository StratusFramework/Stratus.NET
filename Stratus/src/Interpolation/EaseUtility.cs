using Stratus.Utilities;

using System;
using System.Collections.Generic;

namespace Stratus.Interpolation
{
	/// <summary>
	/// Common interpolation algorithms
	/// </summary>
	public enum Ease
	{
		/// <summary>
		/// Linear interpolation
		/// </summary>
		Linear,
		QuadraticIn,
		QuadraticInOut,
		QuadraticOut,
		CubicIn,
		CubicOut,
		CubicInOut,
		ExponentialIn,
		ExponentialOut,
		ExponentialInOut,
		SineIn,
		SineOut,
		SineInOut,
		ElasticIn,
		ElasticOut,
		ElasticInOut,
		Smoothstep
	}

	/// <summary>
	/// Provides methods for common interpolation algorithms,
	/// and common interpolation functions
	/// </summary>
	public static class EaseUtility
	{
		#region Declarations
		/// <summary>
		/// The easing function to be used
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public delegate float EaseFunction(float t);

		/// <summary>
		/// An abstract interpolator for all supported easing types
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public abstract class Interpolator<T>
		{
			public Action<T> setter;
			public EaseFunction function;
			public T difference { get; private set; }
			public T initialValue;
			public T finalValue;

			public Interpolator(T initialValue, T finalValue, EaseFunction function)
			{
				this.initialValue = initialValue;
				this.finalValue = finalValue;
				this.difference = ComputeDifference();
			}

			public abstract T ComputeDifference();
			public abstract T ComputeCurrentValue(float t);
		}
		#endregion

		#region Properties
		private static Dictionary<Ease, EaseFunction> easingFunctions { get; set; } = new Dictionary<Ease, EaseFunction>();
		public static float tMinimum { get; } = 0f;
		public static float tMax { get; } = 1f;
		private static Dictionary<float, Dictionary<float, float>> exponentCache { get; set; } = new Dictionary<float, Dictionary<float, float>>();

		#endregion

		#region Initializer
		static EaseUtility()
		{
			easingFunctions.Add(Ease.Linear, Linear);
			easingFunctions.Add(Ease.CubicIn, CubicIn);
			easingFunctions.Add(Ease.CubicInOut, CubicInOut);
			easingFunctions.Add(Ease.CubicOut, CubicOut);
			easingFunctions.Add(Ease.QuadraticIn, QuadIn);
			easingFunctions.Add(Ease.QuadraticOut, QuadOut);
			easingFunctions.Add(Ease.QuadraticInOut, QuadInOut);
			easingFunctions.Add(Ease.ExponentialIn, ExponentialIn);
			easingFunctions.Add(Ease.ExponentialOut, ExponentialOut);
			easingFunctions.Add(Ease.ExponentialInOut, ExponentialInOut);
			easingFunctions.Add(Ease.ElasticIn, ElasticIn);
			easingFunctions.Add(Ease.ElasticOut, ElasticOut);
			easingFunctions.Add(Ease.ElasticInOut, ElasticInOut);
			easingFunctions.Add(Ease.SineIn, SineIn);
			easingFunctions.Add(Ease.SineOut, SineOut);
			easingFunctions.Add(Ease.SineInOut, SineInOut);
			easingFunctions.Add(Ease.Smoothstep, Smoothstep);
		}
		#endregion

		#region Interpolation Functions
		// Many of these were found on this repository, by Fonserbc
		// https://gist.github.com/Fonserbc/3d31a25e87fdaa541ddf

		// Linear
		public static float Linear(float t)
		{
			return t;
		}

		// - Powers
		// General Power
		public static float PowerIn(float t, float exponent)
		{
			return Power(t, exponent);
		}

		public static float PowerOut(float t, float exponent)
		{
			return 1f - Power(1 - t, exponent);
		}

		public static float PowerInOut(float t, float exponent)
		{
			if (t < 0.5f)
				return PowerIn(t * 2, exponent) / 2;
			return PowerOut(t * 2 - 1, exponent) / 2 + 0.5f;
		}

		// Quadratic
		public static float QuadIn(float t)
		{
			return t * t;
		}

		public static float QuadOut(float t)
		{
			return t * (2f - t);
		}

		public static float QuadInOut(float t)
		{
			if ((t *= 2f) < 1f)
				return 0.5f * t * t;

			return -0.5f * ((t -= 1f) * (t - 2f) - 1f);
		}

		// Cubic
		public static float CubicIn(float t)
		{
			return t * t * t;
		}

		public static float CubicOut(float t)
		{
			return 1f + (t -= 1f) * t * t;
		}

		public static float CubicInOut(float t)
		{
			if ((t *= 2f) < 1f)
				return 0.5f * t * t * t;
			return 0.5f * ((t -= 2f) * t * t + 2f);
		}

		// Sine
		public static float SineIn(float t)
		{
			return 1f - MathUtility.Cos(t * MathUtility.PI / 2f);
		}

		public static float SineOut(float t)
		{
			return MathUtility.Sin(t * MathUtility.PI / 2f);
		}

		public static float SineInOut(float t)
		{
			return 0.5f * (1f - MathUtility.Cos(MathUtility.PI * t));
		}

		// Exponential
		public static float ExponentialIn(float t)
		{
			return t == 0f ? 0f : Power(1024f, t - 1f);
		}

		public static float ExponentialOut(float t)
		{
			return t == 1f ? 1f : 1f - Power(2f, -10f * t);
		}

		public static float ExponentialInOut(float t)
		{
			if (t == 0f) return 0f;
			if (t == 1f) return 1f;
			if ((t *= 2f) < 1f) return 0.5f * Power(1024f, t - 1f);
			return 0.5f * (-Power(2f, -10f * (t - 1f)) + 2f);
		}

		// Elastic
		public static float ElasticIn(float t)
		{
			if (t == 0) return 0;
			if (t == 1) return 1;
			return -Power(2f, 10f * (t -= 1f)) * MathUtility.Sin((t - 0.1f) * (2f * MathUtility.PI) / 0.4f);
		}

		public static float ElasticOut(float t)
		{
			if (t == 0) return 0;
			if (t == 1) return 1;
			return Power(2f, -10f * t) * MathUtility.Sin((t - 0.1f) * (2f * MathUtility.PI) / 0.4f) + 1f;
		}

		public static float ElasticInOut(float t)
		{
			if ((t *= 2f) < 1f) return -0.5f * MathUtility.Pow(2f, 10f * (t -= 1f)) * MathUtility.Sin((t - 0.1f) * (2f * MathUtility.PI) / 0.4f);
			return Power(2f, -10f * (t -= 1f)) * MathUtility.Sin((t - 0.1f) * (2f * MathUtility.PI) / 0.4f) * 0.5f + 1f;
		}

		// Smoothstep
		public static float Smoothstep(float t)
		{
			return t * t * (3 - 2 * t);
		}
		#endregion

		#region Calculations
		/// <summary>
		/// Recalculates the given t value based on the ease selected
		/// </summary>
		/// <param name="t"></param>
		/// <param name="ease"></param>
		/// <returns></returns>
		public static float Calculate(Ease ease, float t) => easingFunctions[ease](t);

		/// <summary>
		/// Returns the function used for this ease
		/// </summary>
		/// <param name="ease"></param>
		/// <returns></returns>
		public static EaseFunction ToFunction(this Ease ease)
		{
			switch (ease)
			{
				case Ease.Linear:
					return Linear;

				case Ease.QuadraticIn:
					return QuadIn;
				case Ease.QuadraticInOut:
					return QuadInOut;
				case Ease.QuadraticOut:
					return QuadOut;

				case Ease.CubicIn:
					return CubicIn;
				case Ease.CubicOut:
					return CubicOut;
				case Ease.CubicInOut:
					return CubicInOut;

				case Ease.ElasticIn:
					return ElasticIn;
				case Ease.ElasticOut:
					return ElasticOut;
				case Ease.ElasticInOut:
					return ElasticInOut;

				case Ease.ExponentialIn:
					return ExponentialIn;
				case Ease.ExponentialOut:
					return ExponentialOut;
				case Ease.ExponentialInOut:
					return ExponentialInOut;

				case Ease.SineIn:
					return SineIn;
				case Ease.SineOut:
					return SineOut;
				case Ease.SineInOut:
					return SineInOut;

				case Ease.Smoothstep:
					return Smoothstep;
			}

			throw new Exception($"No function found for the ease {ease}");
		}

		public static float Evaluate(this Ease ease, float t) => easingFunctions[ease](t);

		/// <summary>
		/// Returns the specified number raised to a specified power
		/// </summary>
		/// <param name="value"></param>
		/// <param name="exponent"></param>
		/// <returns></returns>
		public static float Power(float value, float exponent)
		{
			if (!exponentCache.ContainsKey(value))
			{
				exponentCache.Add(value, new Dictionary<float, float>());
			}

			if (!exponentCache[value].ContainsKey(exponent))
			{
				exponentCache[value].Add(exponent, MathUtility.Pow(value, exponent));
			}

			return exponentCache[value][exponent];

		} 
		#endregion

	}

}