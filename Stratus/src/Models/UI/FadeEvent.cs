using Stratus.Events;

namespace Stratus.Models.UI
{
	public class FadeEvent : Event
	{
		public float duration { get; }

		public FadeEvent(float duration)
		{
			this.duration = duration;
		}
	}

	public class FadeInEvent : FadeEvent
	{
		public FadeInEvent(float duration) : base(duration)
		{
		}
	}

	public class FadeOutEvent : FadeEvent
	{
		public FadeOutEvent(float duration) : base(duration)
		{
		}
	}
}
