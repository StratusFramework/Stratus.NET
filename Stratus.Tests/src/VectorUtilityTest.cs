using NUnit.Framework;

using Stratus.Numerics;

using System.Numerics;

namespace Stratus.Tests
{
	public class VectorUtilityTest
	{
		[TestCase("1,2,3", 1, 2, 3)]
		[TestCase("12,24,36", 12, 24, 36)]
		[TestCase("1.25,2.55,3.66", 1.25f, 2.55f, 3.66f)]
		[TestCase("(1,2,3)", 1, 2, 3)]
		[TestCase("<1,2,3>", 1, 2, 3)]
		[TestCase("<1, 2, 3>", 1, 2, 3)]
		public void ParsesVector3(string input, float x, float y, float z)
		{
			var actual = VectorUtility.ParseVector3(input);
			Assert.AreEqual(x, actual.X);
			Assert.AreEqual(y, actual.Y);
			Assert.AreEqual(z, actual.Z);
		}

		[Test]
		public void ParsesVector3FromToString()
		{
			Vector3 vec = new Vector3(1, 2, 3);
			var input = vec.ToString();
			ParsesVector3(input, 1, 2, 3);
		}
	}
}

