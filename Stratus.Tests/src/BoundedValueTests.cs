using NUnit.Framework;

using Stratus.Data;
using Stratus.Editor.Tests;

namespace Stratus.Tests
{
	public class BoundedValueTests : StratusTest
    {
		[Test]
		public void ValueCannotBeIncreasedPastMaximum()
        {
            const float original = 10;
			BoundedFloat value  = new BoundedFloat(original);
            value.Decrease(3);
            Assert.AreEqual(7, value.current);
            value.Increase(4);
            Assert.AreEqual(original, value.current);
			Assert.True(value.maximal);
		}

		[Test]
		public void ValueCannotBeDecreaseddPastMinimum()
		{
			const float original = 10;
			BoundedFloat value = new BoundedFloat(original);
			value.Decrease(3);
			Assert.AreEqual(7, value.current);
			value.Decrease(7);
			Assert.AreEqual(0, value.current);
			Assert.True(value.minimal, $"Value not minimal {value}");
			value.Decrease(1).Assert(false);
		}

		[Test]
        public void ValueCanBeModified()
        {
            const float original = 100f;
			BoundedFloat hitpoints = new BoundedFloat(original);
            Assert.True(hitpoints.maximal);

            const float bonus  = 50f;
            hitpoints.modifier += bonus;
            Assert.False(hitpoints.maximal);
            AssertSuccess(hitpoints.Increase(bonus));
			Assert.True(hitpoints.maximal, $"Hitpoints not at maximum, {hitpoints}");
            Assert.AreEqual(original + bonus, hitpoints.current);
		}

		[Test]
		public void ValueCanDecreased()
		{
			const float original = 100f;
			BoundedFloat hitpoints = new BoundedFloat(original);
			Assert.True(hitpoints.maximal);

			hitpoints.Decrease(50);
			Assert.False(hitpoints.maximal);
			Assert.AreEqual(50, hitpoints.current);
		}

		[Test]
        public void ValueCanBeReset()
        {
			const float original = 100f;
			BoundedFloat hitpoints = new BoundedFloat(original);
			Assert.True(hitpoints.maximal);

            hitpoints.Decrease(50);
            Assert.False(hitpoints.maximal);
            Assert.AreEqual(50, hitpoints.current);

            hitpoints.Reset();
			Assert.True(hitpoints.maximal);
			Assert.AreEqual(original, hitpoints.current);
		}
    }
}
