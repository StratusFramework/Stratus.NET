using NUnit.Framework;
using System.Collections.Generic;
using Stratus.Interpolation;
using System.Linq;
using Stratus.Extensions;
using System;
using System.Numerics;

namespace Stratus.Tests
{
	public class ActionTests
	{
		public class MockEntity
		{
			public string id;
			public int intValue;
			public float floatValue;
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

		[TestCase(typeof(int), typeof(ActionPropertyInteger))]
		[TestCase(typeof(float), typeof(ActionPropertyFloat))]
		[TestCase(typeof(bool), typeof(ActionPropertyBoolean))]
		[TestCase(typeof(Vector2), typeof(ActionPropertyVector2))]
		[TestCase(typeof(Vector3), typeof(ActionPropertyVector3))]
		public void HasPropertyImplementationForType(Type propertyType, Type implType)
		{
			Assert.AreEqual(implType, ActionProperty.GetImplementation(propertyType));
		}

		[Test]
		public void InstantiatesIntegerProperty()
		{
			const float duration = 1f;
			const Ease ease = Ease.Linear;
			ActionProperty property = ActionProperty.Instantiate(() => target.intValue, 7, duration, ease);
			Assert.NotNull(property);
			Assert.AreEqual(typeof(ActionPropertyInteger), property.GetType());
		}

		[Test]
		public void InstantiatesFloatProperty()
		{
			const float duration = 1f;
			const Ease ease = Ease.Linear;
			ActionProperty property = ActionProperty.Instantiate(() => target.floatValue, 7f, duration, ease);
			Assert.NotNull(property);
			Assert.AreEqual(typeof(ActionPropertyFloat), property.GetType());
		}

		[Test]
		public void InterpolatesInteger()
		{
			const int initial = 0;
			const int finalValue = 5;

			float duration = 1f;
			int ticks = 5;
			float dt = duration / ticks;

			var seq = scheduler.Sequence(target);
			Actions.Property(seq, () => target.intValue, finalValue, 1f, Ease.Linear);

			List<int> values = new List<int>();

			for (int t = 1; t <= ticks; t++)
			{
				scheduler.Update(dt);
				var current = target.intValue;
				values.Add(current);
				Assert.AreEqual(t, current, $"Failed to update values properly. ({values.ToStringJoin()})");
			}

		}
	}
}
