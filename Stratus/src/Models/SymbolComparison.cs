namespace Stratus.Models
{
	public enum SymbolComparison
	{
		IsEqualTo,
		IsNotEqualTo
	}

	public static class SymbolComparisonUtility
	{
		public static bool Compare(this SymbolComparison comparison, object firstValue, object secondValue)
		{
			bool match = false;
			switch (comparison)
			{
				case SymbolComparison.IsEqualTo:
					match = firstValue.Equals(secondValue);
					break;
				case SymbolComparison.IsNotEqualTo:
					match = !firstValue.Equals(secondValue);
					break;
			}
			return match;
		}
	}

}