namespace Stratus.Models
{
	public enum StratusOrientation
	{
		Horizontal,
		Vertical
	}

	/// <summary>
	/// Type of comparison
	/// </summary>
	public enum ComparisonType
	{
		Equals = 1,
		NotEqual = 2,
		Greater = 3,
		Lesser = 4,
		GreaterOrEqual = 5,
		LesserOrEqual = 6
	}

	/// <summary>
	/// Whether to use the dimensions as an absolute value in pixels, or a relative percentage based on screen size
	/// </summary>
	public enum Dimensions
	{
		/// <summary>
		/// The provided size is to be taken in respect to the size of the containing rect (the screen)
		/// </summary>
		Relative,
		/// <summary>
		/// The provided size is to be used directly in pixels
		/// </summary>
		Absolute
	}
}