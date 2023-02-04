using System;
using System.Collections.Generic;

namespace Stratus
{
	public class StratusSearchRange<TPosition, TCost> : Dictionary<TPosition, TCost>
	{
		public StratusSearchRange()
		{
		}

		public StratusSearchRange(IDictionary<TPosition, TCost> dictionary) : base(dictionary)
		{
		}

		public StratusSearchRange(IEqualityComparer<TPosition> comparer) : base(comparer)
		{
		}

		public StratusSearchRange(IEnumerable<KeyValuePair<TPosition, TCost>> collection) : base(collection)
		{
		}

		public StratusSearchRange(IDictionary<TPosition, TCost> dictionary, IEqualityComparer<TPosition> comparer) : base(dictionary, comparer)
		{
		}

		public StratusSearchRange(IEnumerable<KeyValuePair<TPosition, TCost>> collection, IEqualityComparer<TPosition> comparer) : base(collection, comparer)
		{
		}
	}
}