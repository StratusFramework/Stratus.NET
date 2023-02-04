namespace Stratus
{
	/// <summary>
	/// Clock-wise rotation around a hex cell with pointy top
	/// </summary>
	public enum StratusHexagonalOddRowDirection
    {
        Right,
        UpperRight,
        UpperLeft,
        Left,
        DownLeft,
        DownRight
    }

	public enum StratusHexOffsetCoordinates
	{
		OddRow,
		EvenRow,
		OddColumn,
		EvenColumn
	}
}