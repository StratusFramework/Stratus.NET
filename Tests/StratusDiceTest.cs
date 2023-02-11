using NUnit.Framework;

namespace Stratus.Models.Tests
{
    public class StratusDiceTest
    {
        [TestCase(2)]
        [TestCase(4)]
        public void RollsDiceWithinExpectedRange(int die)
        {
            var roll = StratusDice.Roll(die);
            Assert.That(roll > 0 && roll <= die);
        }

        [Test]
        public void CanForceTheNextRoll([Values(6)] int die, [Range(1, 6)]  int nextRoll)
		{
            StratusDice.NextRoll(r => nextRoll);
            var roll = StratusDice.Roll(die);
            Assert.AreEqual(nextRoll, roll);
        }
        
        [TestCase(StratusDie.d1, StratusDie.d4)]
        [TestCase(StratusDie.d12)]
        [TestCase(StratusDie.d20, StratusDie.d1)]
        public void CanRollMultipleDice(params StratusDie[] dice)
		{
            var roll = StratusDice.Roll(null, dice);
            for(int i = 0; i < dice.Length; ++i)
			{
                Assert.AreEqual(dice[i], roll.dice[i].die);
            }
		}

        [Test]
        public void CanForceTheNextRollOfMultipleDice([Values] StratusDie die, [Range(1, 3)] int n)
        {
            string label = "foobar";
            int value = die.ToInteger();
            StratusDice.NextRoll(r => value, label);
            var roll = StratusDice.Roll(label, StratusDie.d6, n);
            Assert.AreEqual(value * n, roll.total);
        }
    }
}
