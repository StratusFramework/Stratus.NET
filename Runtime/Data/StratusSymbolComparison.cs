namespace Stratus
{
	public enum StratusSymbolComparisonType
	{
		IsEqualTo,
		IsNotEqualTo
	}

	public static class StratusSymbolComparison
	{
		public static bool Compare(this StratusSymbolComparisonType comparison, object firstValue, object secondValue)
		{
			bool match = false;
			switch (comparison)
			{
				case StratusSymbolComparisonType.IsEqualTo:
					match = firstValue.Equals(secondValue);
					break;
				case StratusSymbolComparisonType.IsNotEqualTo:
					match = !firstValue.Equals(secondValue);
					break;
			}
			return match;
		}
	}

}