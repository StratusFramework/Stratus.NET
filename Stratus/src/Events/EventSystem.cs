﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stratus.Events
{

	public delegate void BroadcastEventFunction(Event e, Type typeOverride = null);

	public interface IEventBroadcaster
	{
		event BroadcastEventFunction onBroadcast;
	}

	public interface IEventSystem
	{
	}

	/// <summary>
	/// A list of <see cref="Delegate"/>
	/// </summary>
	internal class DelegateList : List<Delegate>
	{
	}

	/// <summary>
	/// The key being the <see cref="Event"/> type name
	/// </summary>
	internal class DelegateMap : Dictionary<string, DelegateList>
	{
	}

	/// <summary>
	/// Represents the subscription of a delegate with a subscriber
	/// </summary>
	public record DelegateBinding(object subscriber, Delegate action);

	// TODO: Add interface methods somehow..
	/// <summary>
	/// The class which manages an event system for entities of type <typeparamref name="TObject"/>
	/// </summary>
	public class EventSystem<TObject> : IEventSystem
	{
		#region Declarations
		/// <summary>
		/// The key being the subscriber
		/// </summary>
		internal class ObjectDispatchMap : Dictionary<TObject, DelegateMap>
		{
		}

		public class Configuration
		{
			public bool logRegister { get; set; } = false;
			public bool logConnect { get; set; } = true;
			public bool logDispatch { get; set; } = false;
			public bool logBroadcast { get; set; } = false;
			public bool logAll
			{
				set
				{
					logBroadcast = logConnect = logRegister = logDispatch = value;
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// The system instance
		/// </summary>
		internal static EventSystem<TObject> instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new EventSystem<TObject>();
				}
				return _instance;
			}
		}
		private static EventSystem<TObject> _instance;
		/// <summary>
		/// Whether we are doing tracing for debugging purposes.
		/// </summary>
		public static Configuration configuration { get; } = new Configuration();
		/// <summary>
		/// A map of all targets connected to the event system and a map of delegates that are connected to them.
		/// Whenever an event of a given type is sent to the GameObject, we invoke it on all delegates for a given type
		/// (essentially a list of delegates for each type)
		/// </summary>
		private ObjectDispatchMap objectDispatchMap { get; set; } = new ObjectDispatchMap();
		/// <summary>
		/// Events that are broadcasted are invoked here
		/// </summary>
		private DelegateMap broadcastMap { get; set; } = new DelegateMap();
		/// <summary>
		/// A list of all event types that are being watched for at the moment.
		/// </summary>
		private List<string> eventWatchList { get; set; } = new List<string>();
		#endregion

		#region Virtual
		protected virtual bool IsNull(object obj) => obj == null;
		protected virtual string GetKey(Type type) => type.ToString();
		protected virtual void OnConnect(TObject obj)
		{
			instance.objectDispatchMap.Add(obj, new DelegateMap());
		}
		#endregion

		#region Interface
		/// <summary>
		/// Resets the system
		/// </summary>
		public static void Reset()
		{
			_instance = null;
		}

		/// <summary>
		/// Connects to the event of a given object.
		/// </summary>
		/// <typeparam name="TEvent">The event class. </typeparam>
		/// <param name="subscriber">The subscriber we are connecting to whose events we are connecting to. </param>
		/// <param name="memFunc">The member function to connect to. </param>
		public static void Connect<TEvent>(TObject subscriber, Action<TEvent> memFunc)
			where TEvent : Event
		{
			Connect(subscriber, typeof(TEvent), e => memFunc((TEvent)e));
		}

		/// <summary>
		/// Connects to the events on the given target of the given type
		/// </summary>
		/// <param name="subscriber"></param>
		/// <param name="callback"></param>
		/// <param name="type"></param>
		public static void Connect(TObject subscriber, Type type, Action<Event> callback)
		{
			string key = instance.GetKey(type);

			// If the object hasn't been registered yet, add its key
			CheckRegistration(subscriber);

			// If this objects dispatch map has no delegates for this event type yet, create it
			if (!instance.objectDispatchMap[subscriber].ContainsKey(key))
			{
				instance.objectDispatchMap[subscriber].Add(key, new DelegateList());
			}

			// If the delegate is already present, do not add it
			if (instance.objectDispatchMap[subscriber][key].Contains(callback))
			{
				return;
			}

			if (configuration.logConnect)
			{
				StratusLog.Info($"Connecting to dispatch of event of type {type} onto {subscriber}");
			}

			// Add the component's delegate onto the gameobject
			instance.objectDispatchMap[subscriber][key].Add(callback);
		}

		/// <summary>
		/// Connects to broadcast events 
		/// </summary>
		public static void Connect<TEvent>(Action<TEvent> callback)
			where TEvent : Event
		{
			Connect(typeof(TEvent), e => callback((TEvent)e));
		}

		/// <summary>
		/// Connects to broadcast events 
		/// </summary>
		public static void Connect(Type type, Action<Event> callback)
		{
			string key = instance.GetKey(type);
			if (!instance.broadcastMap.ContainsKey(key))
			{
				instance.broadcastMap.Add(key, new DelegateList());
			}
			if (instance.broadcastMap[key].Contains(callback))
			{
				return;
			}
			if (configuration.logConnect)
			{
				StratusLog.Info($"Connecting to broadcast of event of type {type}");
			}
			instance.broadcastMap[key].Add(callback);
		}

		/// <summary>
		/// Broadcast the given event
		/// </summary>
		public static void Broadcast(Event e, Type typeOverride = null)
		{
			Type type = typeOverride ?? e.GetType();
			string key = instance.GetKey(type);
			bool watching = false;
			if (instance.eventWatchList.Contains(key))
			{
				watching = true;
			}

			if (!instance.broadcastMap.ContainsKey(key))
			{
				StratusLog.Warning($"No one has connected to broadcast for events of type {type}");
				return;
			}

			DelegateList delegateList = instance.broadcastMap[key];
			if (configuration.logBroadcast)
			{
				StratusLog.Info($"Broadcasting event of type {e}");
			}
			Invoke(e, watching, delegateList);
		}

		/// <summary>
		/// Broadcast the given event of type <typeparamref name="TEvent"/>
		/// </summary>
		public static void Broadcast<TEvent>() where TEvent : Event, new()
			=> Broadcast(new TEvent());

		/// <summary>
		/// Dispatches the given event of the specified type onto the object.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="target">The object to which to connect to.</param>
		/// <param name="e">The event to which to listen for.</param>
		/// <param name="nextFrame">Whether to send this event on the next frame.</param>
		public static void Dispatch<TEvent>(TObject target, TEvent e)
			where TEvent : Event
		{
			Dispatch(target, e, null);
		}

		/// <summary>
		/// Dispatches the given event of the specified type onto the object.
		/// </summary>
		/// <typeparam name="T">The event class.</typeparam>
		/// <param name="target">The object to which to connect to.</param>
		/// <param name="e">The name of the event to which to listen for.</param>
		/// <param name="typeOverride">If set, will override the type to which to send the event to</param>
		public static void Dispatch(TObject target, Event e, Type typeOverride = null)
		{
			Type type = typeOverride ?? e.GetType();
			string key = instance.GetKey(type);

			// Check if the object has been registered onto the event system.
			// If not, it will be.
			CheckRegistration(target);

			// If there is no delegate registered to this object, do nothing.
			if (!HasDelegate(target, key))
			{
				return;
			}

			// If we are watching events of this type
			bool watching = false;
			if (instance.eventWatchList.Contains(key))
			{
				watching = true;
			}

			if (configuration.logDispatch)
			{
				StratusLog.Info($"Dispatching event of type {e} to {target}");
			}

			// Invoke the method for every delegate
			DelegateList delegateList = instance.objectDispatchMap[target][key];
			Invoke(e, watching, delegateList);
		}

		/// <summary>
		/// Registers the object to the event system.
		/// </summary>
		/// <param name="obj">The GameObject which is being registered. </param>
		public static void Connect(TObject obj)
		{
			instance.OnConnect(obj);
		}

		/// <summary>
		/// Returns true if this object is connected to any events
		/// </summary>
		/// <param name="gameObject"></param>
		/// <returns></returns>
		public static bool IsConnected(TObject gameObject)
		{
			return instance.objectDispatchMap.ContainsKey(gameObject);
		}

		/// <summary>
		/// Disconnects the object from all events
		/// </summary>
		/// <param name="obj"></param>
		public static void Disconnect(TObject obj)
		{
			if (instance.objectDispatchMap.ContainsKey(obj))
			{
				instance.objectDispatchMap.Remove(obj);
			}
		}

		/// <summary>
		/// Disconnects the object from events of the given type
		/// </summary>
		/// <param name="obj"></param>
		public static void Disconnect<TEvent>(TObject obj)
			where TEvent : Event
		{
			Disconnect(obj, typeof(TEvent));
		}

		/// <summary>
		/// Disconnects the object from events of the given type
		/// </summary>
		/// <param name="obj"></param>
		public static void Disconnect(TObject obj, Type type)
		{
			if (instance.objectDispatchMap.ContainsKey(obj))
			{
				string key = instance.GetKey(type);
				instance.objectDispatchMap[obj].Remove(key);
			}
		}

		/// <summary>
		/// Adds the specified event to watch list, informing the user whenever
		/// the event is being dispatched.
		/// </summary>
		/// <typeparam name="T">The event type.</typeparam>
		public static void Watch<T>()
		{
			string type = typeof(T).ToString();

			if (!instance.eventWatchList.Contains(type))
			{
				instance.eventWatchList.Add(type);
			}
		}
		#endregion

		#region Implementation
		private static void Invoke(Event e, bool watching, DelegateList delegateList)
		{
			DelegateList delegatesToRemove = null;
			foreach (Delegate deleg in delegateList)
			{
				// If we are watching events of this type
				if (watching)
				{
					//StratusDebug.Log("Invoking member function on " + deleg.Target.ToString());
				}

				// Do a lazy delete if it has been nulled out?
				if (instance.IsNull(deleg.Method) || instance.IsNull(deleg.Target))
				{
					if (delegatesToRemove == null)
					{
						delegatesToRemove = new DelegateList();
					}

					delegatesToRemove.Add(deleg);
					continue;
				}

				deleg.DynamicInvoke(e);
				e.handled = true;
			}

			// If any delegates were found to be null, remove them (lazy delete)
			if (delegatesToRemove != null)
			{
				foreach (Delegate deleg in delegatesToRemove)
				{
					delegateList.Remove(deleg);
				}
			}
		}

		private static bool HasDelegate(TObject obj, string key)
		{
			if (instance.objectDispatchMap[obj] != null
				&& instance.objectDispatchMap[obj].ContainsKey(key))
			{
				return true;
			}

			return false;
		}

		private static void CheckRegistration(TObject gameObj)
		{
			if (!instance.objectDispatchMap.ContainsKey(gameObj))
			{
				Connect(gameObj);
			}
		}
		#endregion
	}

	/// <summary>
	/// The default event system supported by the framework
	/// </summary>
	public sealed class EventSystem : EventSystem<object>
	{
	}
}
