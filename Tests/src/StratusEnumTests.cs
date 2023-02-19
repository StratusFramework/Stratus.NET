using NUnit.Framework;

using Stratus.Utilities;

using System.Linq;

namespace Stratus.Editor.Tests
{
	public class StratusEnumTests
	{
		public enum MockEnum
		{
			A,
			B,
			C
		}

		[Test]
		public void GetsValues()
		{
			MockEnum[] values = EnumUtility.Values<MockEnum>();
			Assert.True(values.Length == 3);
			Assert.AreEqual(values[0], MockEnum.A);
			Assert.AreEqual(values[1], MockEnum.B);
			Assert.AreEqual(values[2], MockEnum.C);
		}

		[Test]
		public void GetsValueByIndex()
		{
			MockEnum[] values = EnumUtility.Values<MockEnum>();
			for (int i = 0; i < values.Length; ++i)
			{
				Assert.AreEqual(EnumUtility.Value<MockEnum>(i), values[i]);
			}
		}

		[Test]
		public void GetsNames()
		{
			MockEnum[] values = EnumUtility.Values<MockEnum>();
			string[] names = EnumUtility.Names<MockEnum>();
			for (int i = 0; i < values.Length; ++i)
			{
				Assert.AreEqual(names[i], values[i].ToString());
			}
		}

		public enum MockFlags
		{
			None = 0,
			A = 1,
			B = 2,
			C = 4,
			All = A | B | C
		}

		[TestCase(MockFlags.None)]
		[TestCase(MockFlags.A, MockFlags.A)]
		[TestCase(MockFlags.B, MockFlags.B)]
		[TestCase(MockFlags.C, MockFlags.C)]
		[TestCase(MockFlags.A | MockFlags.B, MockFlags.A, MockFlags.B)]
		[TestCase(MockFlags.B | MockFlags.A, MockFlags.A, MockFlags.B)]
		[TestCase(MockFlags.C | MockFlags.A, MockFlags.A, MockFlags.C)]
		[TestCase(MockFlags.All, MockFlags.A, MockFlags.B, MockFlags.C)]
		public void GetsFlags(MockFlags value, params MockFlags[] flags)
		{
			MockFlags[] expected = EnumUtility.Flags(value).ToArray();
			Assert.AreEqual(expected.Length, flags.Length);
			Assert.AreEqual(expected, flags);
		}

		public enum MockDegree
		{
			ReallyBad,
			Bad,
			Good,
			ReallyGood
		}

		[TestCase(MockDegree.ReallyBad, MockDegree.Bad)]
		[TestCase(MockDegree.Bad, MockDegree.Good)]
		[TestCase(MockDegree.Good, MockDegree.ReallyGood)]
		[TestCase(MockDegree.ReallyGood, MockDegree.ReallyGood)]
		public void IncreasesDegree(MockDegree value, MockDegree expected)
		{
			Assert.AreEqual(expected, EnumUtility.Increase(value));
		}

		[TestCase(MockDegree.ReallyGood, MockDegree.Good)]
		[TestCase(MockDegree.Good, MockDegree.Bad)]
		[TestCase(MockDegree.Bad, MockDegree.ReallyBad)]
		[TestCase(MockDegree.ReallyBad, MockDegree.ReallyBad)]
		public void DecreasesDegree(MockDegree value, MockDegree expected)
		{
			Assert.AreEqual(expected, EnumUtility.Decrease(value));
		}
	}

}