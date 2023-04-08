using NUnit.Framework;

namespace Stratus.Models.Tests
{
	public class StratusObjectModificationTest
	{
		internal class MockObject
		{
			internal string name;
			internal int points;
		}

		internal interface IMockObjectModification : IObjectModification<MockObject>
		{
		}

		internal abstract class MockModification<TValue> : ObjectModification<MockObject, TValue>,
			IMockObjectModification
		{
			protected MockModification(MockObject TObject) : base(TObject)
			{
			}

			protected MockModification(MockObject target, params TValue[] values) : base(target, values)
			{
			}
		}

		internal class MockPointsModification : MockModification<int>
		{
			public MockPointsModification(MockObject target, params int[] values) : base(target, values)
			{
			}

			protected override bool Apply(int value)
			{
				target.points += value;
				return true;
			}

			protected override bool Revert(int value)
			{
				target.points -= value;
				return true;
			}
		}

		internal class MockObjectModificationCollector : ObjectModificationCollector<MockObject,
			IMockObjectModification>
		{
			public MockObjectModificationCollector(MockObject target) : base(target)
			{
			}

			protected override void OnModificationAdded(IMockObjectModification modification)
			{
			}
		}

		[Test]
		public void AppliesModification()
		{
			var target = new MockObject()
			{
				name = "Bob"
			};

			var collector = new MockObjectModificationCollector(target);

			// Add 1 modification, should be applied immediately
			var mod1 = new MockPointsModification(target, 7);
			string mod1Label = "a";
			collector.Add(mod1Label, mod1);
			Assert.AreEqual(7, target.points);
			Assert.AreEqual(collector.modificationsByLabel[mod1Label][0], collector.modificationsByType[mod1.GetType()][0]);

			// Now remove it, reverting the points change
			collector.Remove(mod1Label);
			Assert.AreEqual(0, target.points);
		}
	}
}
