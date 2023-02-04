using Stratus.Models;

using System.Collections.Generic;

namespace Stratus.Utilities
{
	public class StratusGridRange : StratusSearchRange<StratusVector3Int, float>
	{
		public StratusGridRange()
		{
		}

		public StratusGridRange(IDictionary<StratusVector3Int, float> dictionary) : base(dictionary)
		{
		}

		public StratusGridRange(IEqualityComparer<StratusVector3Int> comparer) : base(comparer)
		{
		}

		public StratusGridRange(IEnumerable<KeyValuePair<StratusVector3Int, float>> collection) : base(collection)
		{
		}

		public StratusGridRange(IDictionary<StratusVector3Int, float> dictionary, IEqualityComparer<StratusVector3Int> comparer) : base(dictionary, comparer)
		{
		}

		public StratusGridRange(IEnumerable<KeyValuePair<StratusVector3Int, float>> collection, IEqualityComparer<StratusVector3Int> comparer) : base(collection, comparer)
		{
		}
	}
}
