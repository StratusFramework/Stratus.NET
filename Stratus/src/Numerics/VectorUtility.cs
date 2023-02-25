using System.Numerics;

namespace Stratus.Numerics
{
	public static class VectorUtility
	{
		/// <summary>
		/// Attempts to parse a Vector2 in either the format:
		/// (a,b) or a,b
		/// </summary>
		public static Vector2 ParseVector2(string value)
		{
			// Remove the parentheses
			if (value.StartsWith("(") && value.EndsWith(")"))
			{
				value = value.Substring(1, value.Length - 2);
			}

			// split the items
			string[] values = value.Split(',');

			// store as a Vector3
			Vector2 result = new Vector2(
				float.Parse(values[0]),
				float.Parse(values[1]));

			return result;
		}

		/// <summary>
		/// Attempts to parse a Vector3 in either the format:
		/// (a,b,c) or a,b,c
		/// </summary>
		public static Vector3 ParseVector3(string value)
		{
			// Remove the parentheses
			if (value.StartsWith("(") && value.EndsWith(")"))
			{
				value = value.Substring(1, value.Length - 2);
			}

			// split the items
			string[] values = value.Split(',');

			// store as a Vector3
			Vector3 result = new Vector3(
				float.Parse(values[0]),
				float.Parse(values[1]),
				float.Parse(values[2]));

			return result;
		}
	}
}
