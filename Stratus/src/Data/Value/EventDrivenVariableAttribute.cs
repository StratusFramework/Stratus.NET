using Stratus.Data;
using Stratus.Events;

namespace Stratus.Models.Values
{
	/// <summary>
	/// An event driven variable
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class EventDrivenVariableAttribute<T> : BoundedFloat
	{
		public abstract record BaseEvent : Event
		{
			public float value { get; set; }
		}

		public record IncreaseEvent : BaseEvent
		{
		}

		public record DecreaseEvent : BaseEvent
		{
		}

		public bool eventConnected { get; private set; }

		public abstract string defaultLabel { get; }

		public EventDrivenVariableAttribute(float value)
			: base(value)
		{
		}

		private void OnIncreaseEvent(IncreaseEvent e) => Increase(e.value);
		private void OnDecreaseEvent(DecreaseEvent e) => Decrease(e.value);

	}
}