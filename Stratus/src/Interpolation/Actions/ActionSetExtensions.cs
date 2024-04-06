using Stratus.Events;

namespace Stratus.Interpolation
{
	/// <summary>
	/// Common methods
	/// </summary>
	public static class ActionSetExtensions
	{
		public static ActionSequence Sequence(this ActionSet set)
			=> set.Add(new ActionSequence());

		public static ActionGroup Group(this ActionSet set)
			=> set.Add(new ActionGroup());
		
		/// <summary>
		/// Broadcasts an event onto the <see cref="EventSystem"/>
		/// </summary>
		public static ActionSet Event<TEvent>(this ActionSet set, TEvent e)
			where TEvent : Event
		{
			var call = new ActionCall(() => EventSystem.Broadcast(e));
			set.Add(call);
			return set;
		}
	}
}
