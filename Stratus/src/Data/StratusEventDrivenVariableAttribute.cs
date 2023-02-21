using Stratus.Data;

namespace Stratus
{
	/// <summary>
	/// An event driven variable
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class StratusEventDrivenVariableAttribute<T> : StratusValue
	{
		public abstract class BaseEvent : StratusEvent
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

		public StratusEventDrivenVariableAttribute(float value,
			float floor = 0,
			float ceiling = float.MaxValue)
			: base(value, floor, ceiling)
		{
		}

		//public abstract void Initialize(object monoBehaviour)
		//{
		//	monoBehaviour.gameObject.Connect<IncreaseEvent>(this.OnIncreaseEvent);
		//	monoBehaviour.gameObject.Connect<DecreaseEvent>(this.OnDecreaseEvent);
		//	eventConnected = true;
		//}

		private void OnIncreaseEvent(IncreaseEvent e) => Increase(e.value);
		private void OnDecreaseEvent(DecreaseEvent e) => Decrease(e.value);

	}
}