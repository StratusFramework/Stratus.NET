using NUnit.Framework;

using Stratus.Models.Math;

namespace Stratus.Models.Tests
{
	public class StratusDiceTest
    {
        [TestCase(2)]
        [TestCase(4)]
        public void RollsDiceWithinExpectedRange(int die)
        {
            var roll = Dice.Roll(die);
            Assert.That(roll > 0 && roll <= die);
        }

        [Test]
        public void CanForceTheNextRoll([Values(6)] int die, [Range(1, 6)]  int nextRoll)
		{
            Dice.NextRoll(r => nextRoll);
            var roll = Dice.Roll(die);
            Assert.AreEqual(nextRoll, roll);
        }
        
        [TestCase(Die.d1, Die.d4)]
        [TestCase(Die.d12)]
        [TestCase(Die.d20, Die.d1)]
        public void CanRollMultipleDice(params Die[] dice)
		{
            var roll = Dice.Roll(null, dice);
            for(int i = 0; i < dice.Length; ++i)
			{
                Assert.AreEqual(dice[i], roll.dice[i].die);
            }
		}

        [Test]
        public void CanForceTheNextRollOfMultipleDice([Values] Die die, [Range(1, 3)] int n)
        {
            string label = "foobar";
            int value = die.ToInteger();
            Dice.NextRoll(r => value, label);
            var roll = Dice.Roll(label, Die.d6, n);
            Assert.AreEqual(value * n, roll.total);
        }
    }
}
