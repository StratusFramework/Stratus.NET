using System;
using System.Collections.Generic;

using Stratus.Serialization;
using Stratus.Utilities;

namespace Stratus
{
	/// <summary>
	/// Base event class for all events that use the Stratus Event System. 
	/// Derive from it in order to implement your own custom events.
	/// your own custom events.
	/// </summary>
	[Serializable]
	public class StratusEvent
	{
		//--------------------------------------------------------------------------------------------/
		// Declarations
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Whether the event is dispatched is being dispatched to a single target or to all
		/// </summary>
		public enum Scope
		{
			Target,
			All
		}

		/// <summary>
		/// A delegate for a connect function
		/// </summary>
		/// <param name="connectFunc"></param>
		public delegate void ConnectFunction(Action<StratusEvent> connectFunc);
		public delegate void EventCallbackFunction(StratusEvent eventObj);
		public delegate void GenericEventCallback<in T>(T eventObj);

		//--------------------------------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------------------------------/
		private static Dictionary<Type, StratusEvent> eventCache = new Dictionary<Type, StratusEvent>();

		//--------------------------------------------------------------------------------------------/
		// Properties
		//--------------------------------------------------------------------------------------------/
		public bool handled { get; set; }
		public static Type[] eventTypes { get; private set; } = StratusTypeUtility.SubclassesOf<StratusEvent>(false);
		public static string[] eventTypeNames { get; private set; } = StratusTypeUtility.SubclassNames<StratusEvent>(false);

		//--------------------------------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------------------------------/
		/// <summary>
		/// Returns an instance of the given event object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reset">Whether the event should be reset to its default values (by constructing it anew)</param>
		/// <returns></returns>
		public static T Get<T>(bool reset = false) where T : StratusEvent, new()
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

		public static StratusEvent Instantiate(Type type) => (StratusEvent)StratusObjectUtility.Instantiate(type);

		public static StratusEvent Instantiate(Type type, string data)
		{
			return (StratusEvent)StratusJSONSerializerUtility.Deserialize(data, type);
		}


		private static void Cache<T>(Type type) where T : StratusEvent, new() => eventCache.Add(type, (T)StratusObjectUtility.Instantiate(type));
		private static bool HasCached<T>(Type type) where T : StratusEvent, new() => eventCache.ContainsKey(type);
		private static void ResetCache<T>(Type type) where T : StratusEvent, new() => eventCache[type] = (T)StratusObjectUtility.Instantiate(type);
	}
}
