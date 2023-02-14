using NUnit.Framework;

using Stratus.Interpolation;

namespace Stratus.Tests
{
	public class ActionTests
	{
		public class MockEntity
		{
			public string id;
		}

		ActionScheduler scheduler;
		MockEntity target;
		const float dt = 0.16f;

		[SetUp]
		public void Setup()
		{
			scheduler = new();
			target = new MockEntity();
			scheduler.Connect(target);
		}

		[Test]
		public void RunsSequenceWithCallsForTarget()
		{
			int value = 0;
			var seq = scheduler.Sequence(target);

			var a = new ActionCall(() => value++);
			var b = new ActionCall(() => value++);
			var c = new ActionCall(() => value++);

			seq.Add(a);
			seq.Add(b);
			seq.Add(c);

			// All 3 actions are executed immediately
			scheduler.Update(dt);
			Assert.AreEqual(3, value);
		}

		[Test]
		public void RunsSequenceWithDelay()
		{
			int value = 0;
			var seq = scheduler.Sequence(target);

			var a = new ActionCall(() => value++);
			var b = new ActionCall(() => value++);
			var c = new ActionCall(() => value++);

			seq.Add(a);
			seq.Add(b);
			var delay = seq.Add(new ActionDelay(dt));
			seq.Add(c);

			scheduler.Update(dt);
			Assert.AreEqual(2, value);
			scheduler.Update(dt);
			Assert.AreEqual(3, value);
		}
	}
}
