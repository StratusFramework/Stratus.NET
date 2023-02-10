using NUnit.Framework;

using Stratus.Extensions;

namespace Stratus.Editor.Tests
{
	public class StratusFloatExtensionsTests
	{
		[Test]
		public static void TestLerp()
		{
			float a = 0f, b = 1f;

			// Lerp To
			Assert.AreEqual(0f, a.LerpTo(b, 0f));
			Assert.AreEqual(0.25f, a.LerpTo(b, 0.25f));
			Assert.AreEqual(0.5f, a.LerpTo(b, 0.5f));
			Assert.AreEqual(0.75f, a.LerpTo(b, 0.75f));
			Assert.AreEqual(1f, a.LerpTo(b, 1f));

			// Lerp From
			Assert.AreEqual(0f, b.LerpFrom(a, 0f));
			Assert.AreEqual(0.25f, b.LerpFrom(a, 0.25f));
			Assert.AreEqual(0.5f, b.LerpFrom(a, 0.5f));
			Assert.AreEqual(0.75f, b.LerpFrom(a, 0.75f));
			Assert.AreEqual(1f, b.LerpFrom(a, 1f));
		}

		[TestCase(5.555f, 5.55f)]
		[TestCase(5.4333f, 5.43f)]
		[TestCase(8.21111111f, 8.21f)]
		[TestCase(-5.5111f, -5.51f)]
		[TestCase(5.5111f, 5.51f)]
		[TestCase(5.54444f, 5.54f)]
		[TestCase(5.55555f, 5.56f)]
		[TestCase(5.58888f, 5.59f)]
		public void RoundsByDecimalPlaces(float value, float expected,  int decimalPlaces = 2)
		{
			Assert.AreEqual(expected, value.Round(decimalPlaces));
		}

		[TestCase(455f, 4.55f)]
		[TestCase(350f, 3.5f)]
		[TestCase(150f, 1.5f)]
		[TestCase(75f, 0.75f)]
		[TestCase(0f, 0f)]
		[TestCase(-75f, -0.75f)]
		[TestCase(-150f, -1.5f)]
		[TestCase(-350f, -3.5f)]
		public void ConvertsValueFromPercentage(float value, float expected)
		{
			float valueToPercent = value.ToPercent();
			Assert.AreEqual(expected, valueToPercent);
			Assert.AreEqual(valueToPercent.FromPercent(), value);
		}

		[Test]
		public void TestToPercent()
		{
			const float value = 350f;
			float valueAsPercent = value.ToPercent();
			Assert.AreEqual("350.00%", valueAsPercent.FromPercentString());
			Assert.AreEqual("350%", valueAsPercent.FromPercentRoundedString());
		}

		[TestCase(1f, 1)]
		[TestCase(1.3f, 1)]
		[TestCase(1.5f, 2)]
		[TestCase(-1.5f, -2)]
		[TestCase(1.6f, 2)]
		[TestCase(-1.6f, -2)]
		public void RoundsIntegerSymmetrically(float value, int expected)
		{
			Assert.AreEqual(expected, value.RoundToInt(StratusRoundingMethod.Symmetrical));
		}

		[Test]
		public void TestIterate()
		{
			int n = 3;

			int result = 0;
			n.Iterate(() => result += 1);
			Assert.AreEqual(n, result);

			result = 0;
			n.Iterate((i) => result += i * 2);
			Assert.AreEqual(0 * 2 + 1 * 2 + 2 * 2, result);

			result = 3;
			n.IterateReverse((i) =>
			{
				Assert.AreEqual(result - 1, i);
				result--;
			});
		}
	}
}