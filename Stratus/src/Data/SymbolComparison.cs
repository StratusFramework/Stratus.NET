namespace Stratus.Data
{
	public enum SymbolComparisonType
	{
		IsEqualTo,
		IsNotEqualTo
	}

	public static class SymbolComparison
	{
		public static bool Compare(this SymbolComparisonType comparison, object firstValue, object secondValue)
		{
			bool match = false;
			switch (comparison)
			{
				case SymbolComparisonType.IsEqualTo:
					match = firstValue.Equals(secondValue);
					break;
				case SymbolComparisonType.IsNotEqualTo:
					match = !firstValue.Equals(secondValue);
					break;
			}
			return match;
		}
	}

}