﻿using Stratus.Data;
using Stratus.Events;

namespace Stratus
{
	/// <summary>
	/// An event driven variable
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class EventDrivenVariableAttribute<T> : StratusValue
	{
		public abstract class BaseEvent : Event
		{
			public float value { get; set; }
		}

		public class IncreaseEvent : BaseEvent
		{
		}

		public class DecreaseEvent : BaseEvent
		{
		}

		public bool eventConnected { get; private set; }

		public abstract string defaultLabel { get; }

		public EventDrivenVariableAttribute(float value,
			float floor = 0,
			float ceiling = float.MaxValue)
			: base(value, floor, ceiling)
		{
		}

		private void OnIncreaseEvent(IncreaseEvent e) => Increase(e.value);
		private void OnDecreaseEvent(DecreaseEvent e) => Decrease(e.value);

	}
}