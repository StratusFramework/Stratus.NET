using System;

namespace Stratus.Events
{
	/// <summary>
	/// Works with events from the default <see cref="EventSystem"/>.
	/// </summary>
	/// <remarks>
	/// By implementing this interface, you will get access to convenience functions for working with the event system.
	/// </remarks>
	public interface IEventSubscriber
	{
	}

	public static class EventSubscriberExtensions
	{
		public static void Connect<TEvent>(this IEventSubscriber subscriber, Action<TEvent> onEvent)
			where TEvent : Event
		{
			EventSystem.Connect(subscriber, onEvent);
		}
	}

	public static class EventDispatcherExtensions
	{
		public static void Broadcast<TEvent>(this IEventSubscriber _, TEvent e)
			where TEvent : Event
		{
			EventSystem.Broadcast(e);
		}
	}
}
