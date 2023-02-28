using System;
using System.Collections.Generic;

using Stratus.Serialization;
using Stratus.Types;

namespace Stratus.Events
{
	/// <summary>
	/// Base event class for all events that use the Stratus Event System. 
	/// Derive from it in order to implement your own custom events.
	/// your own custom events.
	/// </summary>
	[Serializable]
	public class Event
	{
		#region Declarations
		/// <summary>
		/// Whether the event is dispatched is being dispatched to a single target or to all
		/// </summary>
		public enum Scope
		{
			Target,
			All
		}

		public delegate void ConnectFunction(Action<Event> connectFunc);
		public delegate void EventCallbackFunction(Event eventObj);
		public delegate void GenericEventCallback<in T>(T eventObj);
		#endregion

		#region Static Fields
		private static Dictionary<Type, Event> eventCache = new Dictionary<Type, Event>(); 
		#endregion

		#region Properties
		public bool handled { get; set; }
		public static Type[] eventTypes { get; private set; } = TypeUtility.SubclassesOf<Event>(false);
		public static string[] eventTypeNames { get; private set; } = TypeUtility.SubclassNames<Event>(false);
		#endregion

		#region Static Methods
		/// <summary>
		/// Returns an instance of the given event object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reset">Whether the event should be reset to its default values (by constructing it anew)</param>
		/// <returns></returns>
		public static T Get<T>(bool reset = false) where T : Event, new()
		{
			Type type = typeof(T);

			if (!HasCached<T>(type))
			{
				Cache<T>(type);
			}
			else if (reset)
			{
				if (HasCached<T>(type))
				{
					ResetCache<T>(type);
				}
				else
				{
					Cache<T>(type);
				}
			}

			T eventObject = (T)eventCache[type];
			return eventObject;
		}

		public static Event Instantiate(Type type) => (Event)ObjectUtility.Instantiate(type);

		public static Event Instantiate(Type type, string data)
		{
			return (Event)JsonSerializationUtility.Deserialize(data, type);
		}


		private static void Cache<T>(Type type) where T : Event, new() => eventCache.Add(type, (T)ObjectUtility.Instantiate(type));
		private static bool HasCached<T>(Type type) where T : Event, new() => eventCache.ContainsKey(type);
		private static void ResetCache<T>(Type type) where T : Event, new() => eventCache[type] = (T)ObjectUtility.Instantiate(type); 
		#endregion
	}
}
