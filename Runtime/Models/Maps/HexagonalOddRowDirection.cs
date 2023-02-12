namespace Stratus.Models.Maps
{
	/// <summary>
	/// Clock-wise rotation around a hex cell with pointy top
	/// </summary>
	public enum HexagonalOddRowDirection
	{
		Right,
		UpperRight,
		UpperLeft,
		Left,
		DownLeft,
		DownRight
	}

	public enum HexOffsetCoordinates
	{
		OddRow,
		EvenRow,
		OddColumn,
		EvenColumn
	}
}