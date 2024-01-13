using NUnit.Framework;

using Stratus.Events;

using System;

namespace Stratus.Tests
{
	public class StratusEventTest
    {
		private class MockEntity : IEventSubscriber
		{
            public MockEntity(string name)
            {
                this.name = name;
            }

            public string name { get; }
        }

        private class MockEventSystem : EventSystem<MockEntity>
        {
        }

		public record EntityEvent : Event
		{
		}

        private record MockEventFoo : EntityEvent
		{
            public int value;
        }

		private record MockEventBar : EntityEvent
		{
			public int value;
		}

		[SetUp]
		public void Setup()
		{
			MockEventSystem.Reset();
		}

		[Test]
        public void ConnectsToEventForObjectByGenericTypeParameter()
        {
            // A subscribes to the foo event
            MockEntity a = new MockEntity("a");
            MockEventSystem.Connect<MockEventFoo>(a, e =>
            {
                e.value++;
            });

            // Send an event to to A
            MockEventFoo e = new MockEventFoo();
            e.value = 5;
            MockEventSystem.Dispatch(a, e);

            // The value should now be += 1
            Assert.AreEqual(6, e.value);
		}

		[Test]
		public void DisconnectsFromAllEvents()
		{
			MockEntity a = new MockEntity("a");
			MockEventSystem.Connect<MockEventFoo>(a, e =>
			{
				e.value++;
			});

			MockEventFoo e = new MockEventFoo();
			e.value = 5;


			// The value should now be 6
			MockEventSystem.Dispatch(a, e);
			Assert.AreEqual(6, e.value);
			// Disconnect from all events
			MockEventSystem.Disconnect(a);
			// The value should still be  6
			MockEventSystem.Dispatch(a, e);
			Assert.AreEqual(6, e.value);
		}

		[Test]
		public void DisconnectsFromSpecificEvent()
		{
			MockEntity a = new MockEntity("a");

			MockEventSystem.Connect<MockEventFoo>(a, e =>
			{
				e.value++;
			});

			MockEventSystem.Connect<MockEventBar>(a, e =>
			{
				e.value++;
			});

			MockEventFoo foo = new MockEventFoo();
			foo.value = 1;
			MockEventBar bar = new MockEventBar();
			bar.value = 1;


			MockEventSystem.Disconnect<MockEventFoo>(a);
						
			MockEventSystem.Dispatch(a, foo);
			MockEventSystem.Dispatch(a, bar);

			Assert.AreEqual(1, foo.value);
			Assert.AreEqual(2, bar.value);
		}

		[Test]
		public void DispatchesEventWithTypeOverride()
		{
			// A subscribes to all events
			MockEntity a = new MockEntity("a");
			Type type = typeof(EntityEvent);
			MockEventSystem.Connect(a, type, e =>
			{
                if (e is MockEventFoo foo)
                {
				    foo.value++;
                }
			});

			// Send an event to to A
			MockEventFoo e = new MockEventFoo();
			e.value = 5;
			MockEventSystem.Dispatch(a, e, type);

			// The value should now be += 1
			Assert.AreEqual(6, e.value);
		}

		[Test]
		public void DispatchesEventWithGenericTypeOverride()
		{
			// A subscribes to all events
			MockEntity a = new MockEntity("a");
			MockEventSystem.Connect<EntityEvent>(a, e =>
			{
				if (e is MockEventFoo foo)
				{
					foo.value++;
				}
			});

			// Send an event to to A
			MockEventFoo e = new MockEventFoo();
			e.value = 5;
			MockEventSystem.Dispatch(a, e, typeof(EntityEvent));

			// The value should now be += 1
			Assert.AreEqual(6, e.value);
		}

		[Test]
		public void ConnectsToBroadcastEvent()
		{
			Action<MockEventFoo> callback = e =>
			{
				e.value++;
			};

			MockEventSystem.Connect(callback);

			MockEventFoo e = new MockEventFoo();
			e.value = 7;

			MockEventSystem.Broadcast(e);
			Assert.AreEqual(8, e.value);

			MockEventSystem.Broadcast(e);
			Assert.AreEqual(9, e.value);
		}
	}
}
