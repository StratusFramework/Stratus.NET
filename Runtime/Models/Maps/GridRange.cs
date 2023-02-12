using System.Collections.Generic;

namespace Stratus.Models
{
	public class GridRange : StratusSearchRange<StratusVector3Int, float>
	{
		public GridRange()
		{
		}

		public GridRange(IDictionary<StratusVector3Int, float> dictionary) : base(dictionary)
		{
		}

		public GridRange(IEqualityComparer<StratusVector3Int> comparer) : base(comparer)
		{
		}

		public GridRange(IEnumerable<KeyValuePair<StratusVector3Int, float>> collection) : base(collection)
		{
		}

		public GridRange(IDictionary<StratusVector3Int, float> dictionary, IEqualityComparer<StratusVector3Int> comparer) : base(dictionary, comparer)
		{
		}

		public GridRange(IEnumerable<KeyValuePair<StratusVector3Int, float>> collection, IEqualityComparer<StratusVector3Int> comparer) : base(collection, comparer)
		{
		}
	}
}
