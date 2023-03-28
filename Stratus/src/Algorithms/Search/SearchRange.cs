using Stratus.Extensions;

using System;
using System.Collections.Generic;

namespace Stratus.Search
{
	public class SearchRange<TPosition, TCost> : Dictionary<TPosition, TCost>
	{
		public SearchRange()
		{
		}

		public SearchRange(IDictionary<TPosition, TCost> dictionary) : base(dictionary)
		{
		}

		public SearchRange(IEqualityComparer<TPosition> comparer) : base(comparer)
		{
		}

		public SearchRange(IEnumerable<KeyValuePair<TPosition, TCost>> collection) : base(collection)
		{
		}

		public SearchRange(IDictionary<TPosition, TCost> dictionary, IEqualityComparer<TPosition> comparer) : base(dictionary, comparer)
		{
		}

		public SearchRange(IEnumerable<KeyValuePair<TPosition, TCost>> collection, IEqualityComparer<TPosition> comparer) : base(collection, comparer)
		{
		}

		public override string ToString() => this.ToStringForKeyValuePairs();
	}
}