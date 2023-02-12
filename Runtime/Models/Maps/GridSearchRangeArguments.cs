using System;

namespace Stratus.Models
{
	/// <summary>
	/// Arguments to perform a range search
	/// </summary>
	public class GridSearchRangeArguments
	{
		public GridSearchRangeArguments(int minimum, int maximum)
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		public GridSearchRangeArguments(int maximum)
		{
			this.minimum = 0;
			this.maximum = maximum;
		}

		public int minimum { get; }
		public int maximum { get; }
		public Func<StratusVector3Int, float> traversalCostFunction { get; set; }
		public StratusTraversalPredicate<StratusVector3Int> traversableFunction { get; set; }
	}
}
