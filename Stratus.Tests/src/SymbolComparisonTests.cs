using NUnit.Framework;

using Stratus.Data;
using Stratus.Models;

namespace Stratus.Tests
{
	public class SymbolComparisonTests
	{
		[TestCase(7, 7, SymbolComparison.IsEqualTo, true)]
		[TestCase(5, 7, SymbolComparison.IsEqualTo, false)]
		[TestCase(5, 7, SymbolComparison.IsNotEqualTo, true)]
		public void ComparesSymbols(object a, object b, SymbolComparison comparison, bool expected)
		{
			Assert.That(expected, Is.EqualTo(comparison.Compare(a, b)));
		}
	}
}

