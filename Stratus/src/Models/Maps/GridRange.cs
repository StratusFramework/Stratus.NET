using Stratus.Numerics;

using System.Collections.Generic;

namespace Stratus.Models
{
	public class GridRange : StratusSearchRange<Vector3Int, float>
	{
		public GridRange()
		{
		}

		public GridRange(IDictionary<Vector3Int, float> dictionary) : base(dictionary)
		{
		}

		public GridRange(IEqualityComparer<Vector3Int> comparer) : base(comparer)
		{
		}

		public GridRange(IEnumerable<KeyValuePair<Vector3Int, float>> collection) : base(collection)
		{
		}

		public GridRange(IDictionary<Vector3Int, float> dictionary, IEqualityComparer<Vector3Int> comparer) : base(dictionary, comparer)
		{
		}

		public GridRange(IEnumerable<KeyValuePair<Vector3Int, float>> collection, IEqualityComparer<Vector3Int> comparer) : base(collection, comparer)
		{
		}
	}
}
