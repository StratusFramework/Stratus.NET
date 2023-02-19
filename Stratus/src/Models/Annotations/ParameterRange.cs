using System;

namespace Stratus.Models
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public class ParameterRange : Attribute
	{
		public float min;
		public float max;

		public ParameterRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}